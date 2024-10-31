using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using ClassLibrary;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;

namespace IdentityDemo.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly UserManager<UserExtend> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IOptionsSnapshot<JWTSetting> jwtSetting;


        public LoginController(UserManager<UserExtend> userManager, RoleManager<IdentityRole> roleManager,IOptionsSnapshot<JWTSetting> jwtSetting)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.jwtSetting = jwtSetting;
        }
        [HttpPost]
        [NoCheckJWTVerAttribute]//此标识过滤掉检验JWTVer
        public async Task<ActionResult> Login(string userName, string passWord)
        {
            var user = await userManager.FindByNameAsync(userName);
            var result = await userManager.CheckPasswordAsync(user, passWord);
            if (result == true)
            {
                user.JWTVer++;
                await userManager.UpdateAsync(user);
                var item = new CreateToken(jwtSetting,userManager);
                var token = item.CreateLoginToken(userName);
                return Ok(new { token });
            }
            else
            {
                return BadRequest();
            }
        }

    }
}
