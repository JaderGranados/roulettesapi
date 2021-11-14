using System;
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
        public RoulettesManager(IDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbCreateRouletteFunctionName = "bets.sp_create_roulette";
            _dbOpenRouletteFunctionName = "bets.sp_open_roulette";
            _dbCreateRouletteBetFunctionName = "bets.sp_bet";
        }

        public async Task<DefaultResponseDto> Bet(CreateRouletteBetDto createRouletteBetDto)
        {
            return await _dbContext
                .WithFunctionName(_dbCreateRouletteBetFunctionName)
                .ExecuteFunction<DefaultResponseDto>(JsonSerializer.Serialize(createRouletteBetDto));
        }

        public async Task<DefaultResponseDto> Create(CreateRouletteDto createRouletteDto)
        {
            return await _dbContext
                .WithFunctionName(_dbCreateRouletteFunctionName)
                .ExecuteFunction<DefaultResponseDto>(JsonSerializer.Serialize(createRouletteDto));
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
            Console.WriteLine(response.Message);

            return response.Success;
        }
    }
}