using System;
using System.Threading.Tasks;
using RoulettesAPI.Dtos;
using RoulettesAPI.Managers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RoulettesAPI.Controllers
{
    [ApiController]
    [Route("roulette/api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ISignUpManager _signUpManager;

        public AuthController(ISignUpManager signUpManager)
        {
            _signUpManager = signUpManager;
        }

        [HttpPost("signin", Name = "LogIn")]
        public async Task<ActionResult<string>> LogIn(LoginDto loginDto)
        {
            try
            {
                var signUpResponse = await this._signUpManager.SignIn(loginDto);

                return signUpResponse.Success ? Ok(signUpResponse) :
                    BadRequest(signUpResponse);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                return Problem("Error en la consulta", statusCode: 500);
            }
        }

        [HttpGet("signout")]
        public ActionResult LogOut()
        {
            var token = GetAuthorization();
            if (string.IsNullOrEmpty(token))
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }
            
            return this._signUpManager.SignOut(token: token) ? 
                NoContent() : 
                Unauthorized();
        }

        [HttpPost("signup")]
        public async Task<ActionResult<DefaultResponseDto>> Register(CreateUserDto createUserDto)
        {
            try
            {
                var response = await _signUpManager.SignUp(createUserDto);

                return CreatedAtRoute("LogIn", response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        private string GetAuthorization()
        {
            return Request.Headers["Authorization"];
        }
    }
}