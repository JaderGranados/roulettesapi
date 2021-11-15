using System.Threading.Tasks;
using RoulettesAPI.Dtos;

namespace RoulettesAPI.Managers
{
    public interface IRoulettesManager
    {
        Task<DefaultResponseDto> CreateRoulette(CreateRouletteDto createRouletteDto);

        Task<bool> OpenRoulette(string rouletteId);

        Task<DefaultResponseDto> BetOnRoulette(CreateRouletteBetDto createRouletteBetDto);

        Task<CloseRouletteDto> CloseRoulette(string rouletteId);

        Task<PaginatedRoulettesListDto> GetRoulettes(RouletteFiltersDto rouletteFiltersDto);
    }
}