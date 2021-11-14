using System.Threading.Tasks;
using RoulettesAPI.Dtos;

namespace RoulettesAPI.Managers
{
    public interface IRoulettesManager
    {
        Task<DefaultResponseDto> Create(CreateRouletteDto createRouletteDto);

        Task<bool> OpenRoulette(string rouletteId);

        Task<DefaultResponseDto> Bet(CreateRouletteBetDto createRouletteBetDto);
    }
}