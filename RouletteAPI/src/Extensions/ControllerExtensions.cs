using Microsoft.AspNetCore.Mvc;

namespace RoulettesAPI.Extensions
{
    public static class ControllerExtensions
    {
        public static string GetAuthorization(this ControllerBase controller)
        {
            return controller.Request.Headers["Authorization"];
        }
    }
}