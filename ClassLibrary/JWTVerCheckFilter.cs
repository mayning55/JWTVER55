using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ClassLibrary;

public class JWTVerCheckFilter : IAsyncActionFilter
{
    private readonly UserManager<UserExtend> userManager;

    public JWTVerCheckFilter(UserManager<UserExtend> userManager)
    {
        this.userManager = userManager;
    }
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        ControllerActionDescriptor controllerActionDescripor = (ControllerActionDescriptor)context.ActionDescriptor;
        if (controllerActionDescripor == null)
        {
            await next();
            return;
        }
        if(controllerActionDescripor.MethodInfo.GetCustomAttributes(typeof(NoCheckJWTVerAttribute),true).Any())    
        {
            await next();
            return;
        }

        var userName = context.HttpContext.User.FindFirst(ClaimTypes.Name);
        string u = userName.Value.ToString();
        var userid = userManager.FindByNameAsync(u).Result;
        System.Console.WriteLine(userid);
        var claimJWTVer  = context.HttpContext.User.FindFirst("JWTVer");
        long claimJWTVerFromClient = Convert.ToInt64(claimJWTVer.Value);
        System.Console.WriteLine(claimJWTVerFromClient);
        if (claimJWTVer == null)
        {
            context.Result = new ObjectResult("nonejwtver"){StatusCode = 400};
            return ;
        }
        var user = await userManager.FindByNameAsync(u.ToUpper());
        System.Console.WriteLine(user.UserName+user.JWTVer);
        if (user == null)
        {
            context.Result = new ObjectResult("noneuser"){StatusCode = 400};
            return ;
        }
        if(user.JWTVer >claimJWTVerFromClient)
        {
            context.Result = new ObjectResult("jwtmorethan"){StatusCode = 400};
            return ;
        }
        await next();
    }
}
