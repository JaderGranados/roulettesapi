using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RoulettesAPI.Dtos;
using RoulettesAPI.Managers;
using RoulettesAPI.Extensions;

namespace RoulettesAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoulettesController : ControllerBase
    {
        private readonly IRoulettesManager _rouletteManager;
        private readonly ISignUpManager _signUpManager;

        public RoulettesController(IRoulettesManager rouletteManager, ISignUpManager signUpManager)
        {
            _rouletteManager = rouletteManager;
            _signUpManager = signUpManager;
        }

        [HttpPost]
        public async Task<ActionResult<string>> Post(CreateRouletteDto readRouletteDto)
        {
            try
            {
                var token = this.GetAuthorization();
                var response = _signUpManager.IsAuthorized(token) ? 
                    (await _rouletteManager.CreateRoulette(readRouletteDto)) : 
                    throw new UnauthorizedAccessException(message: "Sin autorización");

                return response.Success ? Ok(response.Token) : throw new Exception(message: response.Message);
            }
            catch(UnauthorizedAccessException e)
            {
                return Unauthorized(e.Message);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("open/{id}")]
        public async Task<ActionResult<bool>> Open(string id)
        {
            try
            {
                var token = this.GetAuthorization();
                var response = _signUpManager.IsAuthorized(token) ? 
                    (await _rouletteManager.OpenRoulette(id)) : 
                    throw new UnauthorizedAccessException(message: "Sin autorización");

                return Ok(response);
            }
            catch(UnauthorizedAccessException e)
            {
                return Unauthorized(e.Message);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("bet/{id}")]
        public async Task<ActionResult<bool>> Bet(string id, [FromBody] CreateRouletteBetDto createRouletteBetDto)
        {
            try
            {
                var token = this.GetAuthorization();
                var userId = _signUpManager.GetSession(token);
                createRouletteBetDto.UserId = userId;
                createRouletteBetDto.RouletteId ??= id;
                var response = _signUpManager.IsAuthorized(token) ? 
                    (await _rouletteManager.BetOnRoulette(createRouletteBetDto)) : 
                    throw new UnauthorizedAccessException(message: "Sin autorización");

                return Ok(response);
            }
            catch(UnauthorizedAccessException e)
            {
                return Unauthorized(e.Message);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("close/{id}")]
        public async Task<ActionResult<bool>> Close(string id)
        {
            try
            {
                var token = this.GetAuthorization();
                var response = _signUpManager.IsAuthorized(token) ? 
                    (await _rouletteManager.CloseRoulette(id)) : 
                    throw new UnauthorizedAccessException(message: "Sin autorización");

                return Ok(response);
            }
            catch(UnauthorizedAccessException e)
            {
                return Unauthorized(e.Message);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("list")]
        public async Task<ActionResult<PaginatedRoulettesListDto>> GetRoulettes([FromBody] RouletteFiltersDto rouletteFiltersDto)
        {
            try
            {
                var token = this.GetAuthorization();
                var response = _signUpManager.IsAuthorized(token) ? 
                    (await _rouletteManager.GetRoulettes(rouletteFiltersDto)) : 
                    throw new UnauthorizedAccessException(message: "Sin autorización");

                return Ok(response);
            }
            catch(UnauthorizedAccessException e)
            {
                return Unauthorized(e.Message);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}