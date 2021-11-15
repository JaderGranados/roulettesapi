-- DROP SCHEMA public;

CREATE SCHEMA public AUTHORIZATION postgres;

COMMENT ON SCHEMA public IS 'standard public schema';


-- DROP TYPE paginated_list;

CREATE TYPE paginated_list AS (
	current_page int4,
	items_per_page int4,
	total_items int4,
	items _jsonb);

-- DROP TYPE response;

CREATE TYPE response AS (
	message text,
	success bool,
	"token" text);
