using System.Text.Json;
using System.Threading.Tasks;
using RoulettesAPI.Dtos;
using RoulettesAPI.Persistence;
using RoulettesAPI.Types;

namespace RoulettesAPI.Managers.Implementations
{
    public class RoulettesManager : IRoulettesManager
    {
        private readonly IDbContext _dbContext;
        private readonly string _dbCreateRouletteFunctionName;
        private readonly string _dbOpenRouletteFunctionName;
        private readonly string _dbCreateRouletteBetFunctionName;
        private readonly string _dbCloseRouletteFunctionName;
        private readonly string _dbGetRoulettesFunctionName;

        public RoulettesManager(IDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbCreateRouletteFunctionName = "bets.sp_create_roulette";
            _dbOpenRouletteFunctionName = "bets.sp_open_roulette";
            _dbCreateRouletteBetFunctionName = "bets.sp_bet";
            _dbCloseRouletteFunctionName = "bets.sp_close_roulette";
            _dbGetRoulettesFunctionName = "bets.sp_get_roulettes";
        }

        public async Task<DefaultResponseDto> BetOnRoulette(CreateRouletteBetDto createRouletteBetDto)
        {
            return await _dbContext
                .WithFunctionName(_dbCreateRouletteBetFunctionName)
                .ExecuteFunction<DefaultResponseDto>(JsonSerializer.Serialize(createRouletteBetDto));
        }

        public async Task<CloseRouletteDto> CloseRoulette(string rouletteId)
        {
            var request = new RouletteFiltersDto
            {
                RouletteId = rouletteId
            };

            return await _dbContext
                .WithFunctionName(_dbCloseRouletteFunctionName)
                .ExecuteFunction<CloseRouletteDto>(JsonSerializer.Serialize(request));
        }

        public async Task<DefaultResponseDto> CreateRoulette(CreateRouletteDto createRouletteDto)
        {
            return await _dbContext
                .WithFunctionName(_dbCreateRouletteFunctionName)
                .ExecuteFunction<DefaultResponseDto>(JsonSerializer.Serialize(createRouletteDto));
        }

        public async Task<PaginatedRoulettesListDto> GetRoulettes(RouletteFiltersDto rouletteFiltersDto)
        {
            return await _dbContext
                .WithFunctionName(_dbGetRoulettesFunctionName)
                .ExecuteFunction<PaginatedRoulettesListDto>(JsonSerializer.Serialize(rouletteFiltersDto));
        }

        public async Task<bool> OpenRoulette(string rouletteId)
        {
            var request = new RouletteFiltersDto
            {
                RouletteId = rouletteId,
                StatusId = RouletteStatus.OPENED
            };         
            var response = await _dbContext
                .WithFunctionName(_dbOpenRouletteFunctionName)
                .ExecuteFunction<DefaultResponseDto>(JsonSerializer.Serialize(request));

            return response.Success;
        }
    }
}