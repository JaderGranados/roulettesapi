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
            return _redisClient.Remove(token);
        }

        public async Task<string> SignIn(LoginDto loginDto)
        {
            var response = await _dbContext.
                WithFunctionName(_dbLoginFunctionName).
                ExecuteFunction<DefaultResponseDto>(JsonSerializer.Serialize(loginDto));
            var token = Guid.NewGuid().ToString();

            return _redisClient.Set<string>(key: token, redisObject: response.Token) ? 
                token : throw new Exception(message: "Error al procesar su solicitud");
        }

        public bool IsAuthorized(string token)
        {
            return string.IsNullOrEmpty(token) ? false : 
                !string.IsNullOrEmpty(_redisClient.Get<string>(key: token));
        }

        public string GetSession(string token)
        {
            return !string.IsNullOrEmpty(token) ? 
                _redisClient.Get<string>(token) :
                throw new Exception(message: "Debe enviar un token de verificación");
        }
    }
}