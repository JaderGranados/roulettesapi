-- DROP SCHEMA bets;

CREATE SCHEMA bets AUTHORIZATION postgres;
-- bets.bet_type definition

-- Drop table

-- DROP TABLE bets.bet_type;

CREATE TABLE bets.bet_type (
	bet_type_id int4 NOT NULL,
	description varchar NOT NULL,
	"enum" varchar NOT NULL,
	is_active bool NOT NULL DEFAULT true,
	CONSTRAINT bet_type_pk PRIMARY KEY (bet_type_id)
);


-- bets.roulette_status_type definition

-- Drop table

-- DROP TABLE bets.roulette_status_type;

CREATE TABLE bets.roulette_status_type (
	roulette_status_type_id int4 NOT NULL,
	description varchar NOT NULL,
	"enum" varchar NOT NULL,
	CONSTRAINT roulette_status_type_pk PRIMARY KEY (roulette_status_type_id)
);


-- bets.turns definition

-- Drop table

-- DROP TABLE bets.turns;

CREATE TABLE bets.turns (
	turn_id uuid NOT NULL DEFAULT gen_random_uuid(),
	game_id uuid NOT NULL,
	"number" int4 NOT NULL,
	opening_date timestamp NOT NULL DEFAULT now()::timestamp without time zone,
	max_amount numeric NOT NULL DEFAULT 10000,
	CONSTRAINT turns_pk PRIMARY KEY (turn_id)
);


-- bets.roulette_bets definition

-- Drop table

-- DROP TABLE bets.roulette_bets;

CREATE TABLE bets.roulette_bets (
	user_id uuid NOT NULL,
	roulette_id uuid NOT NULL,
	turn_id uuid NOT NULL,
	amount numeric NOT NULL,
	created_date timestamp NOT NULL DEFAULT now()::timestamp without time zone,
	is_winner bool NOT NULL DEFAULT false,
	value int4 NOT NULL,
	bet_type_id int4 NOT NULL,
	CONSTRAINT roulette_bets_pk PRIMARY KEY (user_id, roulette_id, turn_id),
	CONSTRAINT roulette_bets_fk FOREIGN KEY (bet_type_id) REFERENCES bets.bet_type(bet_type_id)
);


-- bets.roulettes definition

-- Drop table

-- DROP TABLE bets.roulettes;

CREATE TABLE bets.roulettes (
	roulette_id uuid NOT NULL DEFAULT gen_random_uuid(),
	"name" varchar NOT NULL,
	roulette_status_type_id int4 NOT NULL,
	created_date date NOT NULL DEFAULT now()::timestamp without time zone,
	CONSTRAINT roulettes_pk PRIMARY KEY (roulette_id),
	CONSTRAINT roulettes_un UNIQUE (name),
	CONSTRAINT roulettes_fk FOREIGN KEY (roulette_status_type_id) REFERENCES bets.roulette_status_type(roulette_status_type_id)
);

-- DROP TYPE bets.beting_type;

CREATE TYPE bets.beting_type AS (
	"user" text,
	value int4,
	amount numeric(1000,999),
	is_winner bool,
	bet_type text);

-- DROP TYPE bets.roulette_bet_type;

CREATE TYPE bets.roulette_bet_type AS (
	roulette_id uuid,
	user_id uuid,
	amount numeric(1000,999),
	value int4,
	bet_type_id int4);

-- DROP TYPE bets.roulette_filters;

CREATE TYPE bets.roulette_filters AS (
	roulette_id uuid,
	status_id int4,
	items_per_page int4,
	current_page int4);

-- DROP TYPE bets.roulette_item_type;

CREATE TYPE bets.roulette_item_type AS (
	roulette_id uuid,
	"name" text,
	status text,
	last_turn_number int4,
	created_date timestamp);

-- DROP TYPE bets.roulette_results;

CREATE TYPE bets.roulette_results AS (
	roulette_id uuid,
	"name" text,
	opening_date timestamp,
	closing_date timestamp,
	bets _beting_type,
	winner_number int4);

-- DROP TYPE bets.roulette_type;

CREATE TYPE bets.roulette_type AS (
	"name" text,
	status_id int4);


CREATE OR REPLACE FUNCTION bets.sp_bet(in_data text)
 RETURNS text
 LANGUAGE plpgsql
AS $function$
DECLARE roulette_bet bets.roulette_bet_type;
		opened_status_id int4;
		out_data public.response;
		is_opened bool;
		last_turn uuid;
		max_bet_amount_exceded bool;
BEGIN
	SELECT	*
	INTO	roulette_bet
	FROM 	jsonb_populate_record(NULL::bets.roulette_bet_type, in_data::jsonb);

	opened_status_id := (SELECT rst.roulette_status_type_id
		FROM	bets.roulette_status_type rst
		WHERE	rst."enum" = 'OPENED');
	
	is_opened := EXISTS(SELECT 1
		FROM	bets.roulettes r
		WHERE	r.roulette_id = roulette_bet.roulette_id
			AND r.roulette_status_type_id = opened_status_id);	
	
	IF (NOT is_opened) THEN
		RAISE 'La ruleta no está abierta';
	END IF;

	IF (NOT int4range(0, 36) @> roulette_bet.value) THEN
		RAISE 'El número debe estar entre 0 y 36';		
	END IF;
		
	last_turn := (SELECT t.turn_id 
		FROM	bets.turns t
		WHERE	t.game_id = roulette_bet.roulette_id
		ORDER BY t.opening_date DESC);
	
	max_bet_amount_exceded := COALESCE((SELECT t.max_amount - t."sum"
		FROM  (
			SELECT t.turn_id,
					t.max_amount,
					sum(rb.amount)
			FROM	bets.roulette_bets rb
				LEFT JOIN bets.turns t ON t.turn_id = rb.turn_id
			WHERE	rb.turn_id = last_turn
			GROUP  BY t.turn_id, t.max_amount) t), roulette_bet.amount) < roulette_bet.amount;
	
	IF (max_bet_amount_exceded) THEN
		RAISE 'Máximo monto de ruleta superado. Intente con un monto menor';
	END IF;
	
	WITH inserting AS (
		INSERT 	INTO bets.roulette_bets (user_id, roulette_id, turn_id, amount, value, bet_type_id)
		SELECT	roulette_bet.user_id,
				roulette_bet.roulette_id,
				(SELECT	t.turn_id
				FROM	bets.turns t
				WHERE	t.game_id = roulette_bet.roulette_id
				ORDER BY t.opening_date DESC
				LIMIT	1),
				roulette_bet.amount,
				roulette_bet.value,
				roulette_bet.bet_type_id
		ON CONFLICT DO NOTHING 
		RETURNING roulette_id AS RESULT
	)
	SELECT 	'Apuesta realizada con éxito',
			TRUE,
			i.*
	INTO	out_data
	FROM 	inserting i;

	IF (out_data.success IS NULL) THEN
		RAISE 'Un usuario no puede apostar dos veces en el mismo turno';
	END IF;
	
	RETURN to_jsonb(out_data)::TEXT;
END;
$function$
;

CREATE OR REPLACE FUNCTION bets.sp_close_roulette(in_data text)
 RETURNS text
 LANGUAGE plpgsql
AS $function$
DECLARE filters bets.roulette_filters;
		closed_status_id int4;
		exists_status bool;
		out_data bets.roulette_results;
		random_number int4;
		is_odd bool;
BEGIN
	SELECT 	*
	INTO	filters
	FROM 	jsonb_populate_record(NULL::bets.roulette_filters, in_data::jsonb);

	closed_status_id := (SELECT rst.roulette_status_type_id
		FROM 	bets.roulette_status_type rst
		WHERE 	rst."enum" = 'CLOSED');

	exists_status := (SELECT count(*) 
		FROM 	bets.roulettes r
		WHERE 	r.roulette_id = filters.roulette_id
			AND r.roulette_status_type_id = closed_status_id);
		
	IF (exists_status) THEN
		RAISE 'La ruleta ya está cerrada';
	END IF;

	random_number := (SELECT floor(random()*(37)));

	is_odd := (SELECT MOD(random_number, 2) = 1);

	WITH updating_bets AS (
		UPDATE 	bets.roulette_bets 
		SET		is_winner = TRUE 
		WHERE 	value = random_number
			AND roulette_id = filters.roulette_id
			OR 	(MOD(value, 2) = 1 AND is_odd)
		RETURNING user_id  AS RESULT 
	),
	updating_roulette AS (
		UPDATE	bets.roulettes 
		SET		roulette_status_type_id = closed_status_id
		WHERE 	roulette_id = filters.roulette_id
	),
	getting_data AS (
		SELECT 	r.roulette_id AS roulette_id,
				r."name" AS "name",
				t.opening_date AS opening_date,
				now()::timestamp AS closing_date,
				COALESCE(array_agg((
					u.names,
					rb.value,
					rb.amount,
					(rb.value = random_number OR  (MOD(rb.value, 2) = 1 AND is_odd)),
					bt.description 
				)::bets.beting_type), ARRAY[]::bets.beting_type[]) AS bets,
				random_number AS winner_number
		FROM 	bets.roulettes r 
			LEFT JOIN (SELECT DISTINCT ON(t.game_id) * FROM bets.turns t ORDER BY t.game_id, t.opening_date DESC) t ON t.game_id = r.roulette_id
			LEFT JOIN bets.roulette_bets rb USING (roulette_id)
			LEFT JOIN signup.users u ON u.user_id = rb.user_id
			LEFT JOIN bets.bet_type bt ON bt.bet_type_id = rb.bet_type_id 
		WHERE 	r.roulette_id = filters.roulette_id
		GROUP BY roulette_id, "name", opening_date, closing_date 
	)
	SELECT	*
	INTO	out_data
	FROM 	getting_data;
	
	RETURN to_jsonb(out_data)::TEXT;
END;
$function$
;

CREATE OR REPLACE FUNCTION bets.sp_create_roulette(in_data text)
 RETURNS text
 LANGUAGE plpgsql
AS $function$
DECLARE roulette bets.roulette_type;
		out_data public.response;
BEGIN
	SELECT	*
	INTO	roulette
	FROM 	jsonb_populate_record(NULL::bets.roulette_type, in_data::jsonb);

	WITH upserting AS (
		INSERT 	INTO bets.roulettes ("name", roulette_status_type_id)
		SELECT	roulette."name",
				roulette.status_id
		ON CONFLICT ("name") DO NOTHING
		RETURNING roulette_id AS RESULT
	)
	SELECT 	'Ruleta creada con éxito',
			u.* IS NOT NULL,
			u.*
	INTO	out_data
	FROM 	upserting u;

	IF (out_data.success IS NULL) THEN
		RAISE 'Ya existe una ruleta con este nombre';
	END IF;
	
	RETURN to_jsonb(out_data)::TEXT;
END;
$function$
;

CREATE OR REPLACE FUNCTION bets.sp_get_roulettes(in_data text)
 RETURNS text
 LANGUAGE plpgsql
AS $function$
DECLARE out_data public.paginated_list;
		filters bets.roulette_filters;
BEGIN
	SELECT 	*
	INTO	filters
	FROM 	jsonb_populate_record(NULL::bets.roulette_filters, in_data::jsonb);

	WITH filtered_data AS (
		SELECT	DISTINCT ON (r.roulette_id)
				r.roulette_id,
				r."name",
				rst.description,
				t."number",
				r.created_date
		FROM 	bets.roulettes r 
			LEFT JOIN bets.roulette_status_type rst ON rst.roulette_status_type_id = r.roulette_status_type_id 
			LEFT JOIN (SELECT DISTINCT ON (t.turn_id) * FROM bets.turns t ORDER BY t.turn_id, t.opening_date DESC) t ON t.game_id = r.roulette_id 
			LEFT JOIN bets.roulette_bets rb ON rb.turn_id = t.turn_id
		WHERE 	(r.roulette_id = filters.roulette_id OR filters.roulette_id IS NULL)
			AND	(r.roulette_status_type_id = filters.status_id OR filters.status_id IS NULL)
	),
	limited_data AS (
		SELECT	fd.*
		FROM 	filtered_data fd
		ORDER BY fd.created_date
		LIMIT	filters.items_per_page
		OFFSET	((filters.current_page - 1) * filters.items_per_page) 
	)
	SELECT	filters.current_page,
			filters.items_per_page,
			(SELECT count(*) FROM filtered_data),
			COALESCE(array_agg(to_jsonb((ld.*)::bets.roulette_item_type)), ARRAY[]::jsonb[])
	INTO	out_data
	FROM 	limited_data ld;

	RETURN to_jsonb(out_data)::TEXT;
END;
$function$
;

CREATE OR REPLACE FUNCTION bets.sp_open_roulette(in_data text)
 RETURNS text
 LANGUAGE plpgsql
AS $function$
DECLARE filters bets.roulette_filters;
		opened_status_id int4;
		exists_status bool;
		out_data public.response;
BEGIN
	SELECT 	*
	INTO	filters
	FROM 	jsonb_populate_record(NULL::bets.roulette_filters, in_data::jsonb);

	opened_status_id := (SELECT rst.roulette_status_type_id
		FROM 	bets.roulette_status_type rst
		WHERE 	rst."enum" = 'OPENED');

	exists_status := (SELECT count(*) 
		FROM 	bets.roulettes r
		WHERE 	r.roulette_id = filters.roulette_id
			AND r.roulette_status_type_id = opened_status_id);
		
	IF (exists_status) THEN
		SELECT 	'La ruleta ya está abierta',
				FALSE,
				NULL
		INTO	out_data;
		RETURN	to_jsonb(out_data)::TEXT;
	END IF;

	WITH updating AS (
		UPDATE 	bets.roulettes 
		SET		roulette_status_type_id = opened_status_id
		WHERE 	roulette_id = filters.roulette_id
		RETURNING roulette_id AS RESULT
	),
	inserting_turn AS (
		INSERT 	INTO bets.turns (game_id, "number")
		SELECT 	filters.roulette_id,
				COALESCE((SELECT t."number" 
				FROM 	bets.turns t
				WHERE	t.game_id = filters.roulette_id
				ORDER BY t.opening_date DESC
				LIMIT	1), 0) + 1
		RETURNING turn_id AS RESULT
	)
	SELECT 	'Se ha abiert esta ruleta con éxito',
			TRUE,
			it.*
	INTO	out_data
	FROM 	updating it;

	RETURN to_jsonb(out_data)::TEXT;
END;
$function$
;
