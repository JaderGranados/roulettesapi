using System.Threading.Tasks;
using RoulettesAPI.Dtos;

namespace RoulettesAPI.Managers
{
    public interface ISignUpManager
    {
        Task<DefaultResponseDto> SignIn(LoginDto loginDto);

        bool SignOut(string token);

        Task<DefaultResponseDto> SignUp(CreateUserDto createUserDto);

        bool IsAuthorized(string token);
    }
}