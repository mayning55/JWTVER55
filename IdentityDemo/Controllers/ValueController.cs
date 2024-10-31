using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IdentityDemo.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class ValueController : ControllerBase
    {
        
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public string AdminValue()
        {
            var name = this.User.FindFirst(ClaimTypes.Name);
            var role = this.User.FindFirst(ClaimTypes.Role);
            return name.Value +" "+ role.Value +"ok";
        }

        [HttpGet]
        [Authorize(Roles ="NormalUser")]
        public string UserValue()
        {
            var result = this.User.FindFirst(ClaimTypes.Name);
            return result.Value + "ok";
        }

        [HttpGet]
        [Authorize(Roles ="Admin,NormalUser")]
        public string MulitValue()
        {
            var result = this.User.FindFirst(ClaimTypes.Name);
            return result.Value + "ok";
        }

        [HttpGet]
        [AllowAnonymous]
        public string AllowAnonymous()
        {
            return "AllowAnonymous";
        }
    }
}
