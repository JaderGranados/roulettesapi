using System;
using System.Text.Json;
using System.Threading.Tasks;
using RoulettesAPI.Dtos;
using RoulettesAPI.Persistence;

namespace RoulettesAPI.Managers.Implementations
{
    public class SignUpManager : ISignUpManager
    {
        private readonly IDbContext _dbContext;
        private readonly IRedisClient _redisClient;
        private readonly string _dbLoginFunctionName;
        private readonly string _dbCreateUserFunctionName;

        public SignUpManager(IDbContext dbContext,
            IRedisClient redisClient)
        {
            _dbContext = dbContext;
            _dbLoginFunctionName = "signup.sp_login";
            _dbCreateUserFunctionName = "signup.sp_create_user";
            _redisClient = redisClient;
        }

        public async Task<DefaultResponseDto> SignUp(CreateUserDto createUserDto)
        {
            var stringifyObject = JsonSerializer.Serialize(createUserDto);
            return await _dbContext
                .WithFunctionName(_dbCreateUserFunctionName)
                .ExecuteFunction<DefaultResponseDto>(stringifyObject);
        }

        public bool SignOut(string token)
        {
            return this._redisClient.Remove(token);
        }

        public async Task<DefaultResponseDto> SignIn(LoginDto loginDto)
        {
            var response = await this._dbContext.
                WithFunctionName(_dbLoginFunctionName).
                ExecuteFunction<DefaultResponseDto>(JsonSerializer.Serialize(loginDto));
            response.Token = Guid.NewGuid().ToString();
            
            return this._redisClient.Set<LoginDto>(response.Token, loginDto) ? 
                response : throw new Exception(message: "Error al procesar su solicitud");
        }

        public bool IsAuthorized(string token)
        {
            return !string.IsNullOrEmpty(this._redisClient.Get<string>(key: token));
        }
    }
}