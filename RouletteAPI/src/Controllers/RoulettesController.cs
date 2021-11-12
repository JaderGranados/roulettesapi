using Microsoft.AspNetCore.Mvc;
using RoulettesAPI.Dtos;

namespace RoulettesAPI.Controllers
{
    public class RoulettesController : ControllerBase
    {
        [HttpPost]
        public ActionResult<string> Post(RouletteDto rouletteDto)
        {
            return "";
        }
    }
}