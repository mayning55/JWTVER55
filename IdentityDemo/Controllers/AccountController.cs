using ClassLibrary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityDemo.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<UserExtend> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public AccountController(UserManager<UserExtend> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        [HttpPost]
        [NoCheckJWTVerAttribute]
        public async Task<ActionResult> ManagerUserRoleExists()
        {
            if(await roleManager.RoleExistsAsync("Admin")==false)
            {
                IdentityRole adminRole = new IdentityRole();
                adminRole.Name = "Admin";
                var result =await roleManager.CreateAsync(adminRole);
                if (!result.Succeeded)
                {
                    return BadRequest();
                }
            }
            if(await roleManager.RoleExistsAsync("NormalUser")==false)
            {
                IdentityRole normalUserRole = new IdentityRole();
                normalUserRole.Name = "NormalUser";
                var result =await roleManager.CreateAsync(normalUserRole);
                if (!result.Succeeded)
                {
                    return BadRequest();
                }
            }
            var userAdmin = await userManager.FindByNameAsync("Admin");
            if (userAdmin == null)
            {
                userAdmin = new UserExtend();
                userAdmin.UserName = "Admin";
                var result = await userManager.CreateAsync(userAdmin,"123123");
                if (!result.Succeeded)
                {
                    return BadRequest();
                }
            }
            if (!await userManager.IsInRoleAsync(userAdmin,"Admin"))
            {
                var result = await userManager.AddToRoleAsync(userAdmin,"Admin");
                if (!result.Succeeded)
                {
                    return BadRequest();
                }
            }
            return Ok();
        }
        [HttpPost]
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult> AddUser(string userName, string passWord)
        {
            var item = await userManager.FindByNameAsync(userName);
            if (item == null)
            {
                item = new UserExtend();
                item.UserName=userName;
                var result = await userManager.CreateAsync(item,passWord);
                if (!result.Succeeded)
                {
                    return BadRequest();
                }
            }
            else
            {
                return Ok("user exisit");
            }
            if (!await userManager.IsInRoleAsync(item,"NormalUser"))
            {
                var result = await userManager.AddToRoleAsync(item,"NormalUser");
                if (!result.Succeeded)
                {
                    return BadRequest();
                }
            }
            return Ok();
        }
        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DisabledUser(string userName)
        {
            var item = await userManager.FindByNameAsync(userName);
            if(item == null)
            {
                return NotFound();
            }
            item.IsDisabled = true;
            item.JWTVer++;
            await userManager.UpdateAsync(item);
            return Ok();
        }
        [HttpPut]
        [Authorize(Roles="Admin")]
        public async Task<ActionResult> ChangeUserPassword(string userName,string newPassword)
        {
            var item = await userManager.FindByNameAsync(userName);
            if(item == null)
            {
                return NotFound();
            }
            if(newPassword == null)
            {
                newPassword = "123123";
            }
            var result = await userManager.RemovePasswordAsync(item);
            if(result.Succeeded)
            {
                await userManager.AddPasswordAsync(item,newPassword);
            }
            return Ok();
        }






    }
}
