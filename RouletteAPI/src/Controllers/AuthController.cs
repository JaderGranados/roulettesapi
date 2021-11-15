using System;
using System.Threading.Tasks;
using RoulettesAPI.Dtos;
using RoulettesAPI.Managers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RoulettesAPI.Extensions;

namespace RoulettesAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
                var signUpResponse = await _signUpManager.SignIn(loginDto);

                return Ok(signUpResponse);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("signout")]
        public ActionResult LogOut()
        {
            var token = this.GetAuthorization();
            if (string.IsNullOrEmpty(token))
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }
            
            return _signUpManager.SignOut(token: token) ? 
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
    }
}