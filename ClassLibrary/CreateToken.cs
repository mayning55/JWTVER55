using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ClassLibrary;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace IdentityDemo;

public class CreateToken
{
    private readonly IOptionsSnapshot<JWTSetting> jwtSetting;
    private readonly UserManager<UserExtend> userManager;

    public CreateToken(IOptionsSnapshot<JWTSetting> jwtSetting, UserManager<UserExtend> userManager)
    {
        this.jwtSetting = jwtSetting;
        this.userManager = userManager;
    }

    public async Task<string> CreateLoginToken(string userName)
    {
        var user = await userManager.FindByNameAsync(userName);
        var claims = new List<Claim>();
        claims.Add(new Claim(ClaimTypes.Name, userName));
        var role = await userManager.GetRolesAsync(user);
        foreach (var r in role)
        {
            claims.Add(new Claim(ClaimTypes.Role, r));
        }
        claims.Add(new Claim("JWTVer", user.JWTVer.ToString()));
        //claims.Add(new Claim(ClaimTypes.Name,userName));
        //claims.Add(new Claim("其它属性","其它属性值"));
        //string issuer = jwtSetting.Value.Issuer;
        //string audience = jwtSetting.Value.Audience;
        string key = jwtSetting.Value.SigningKey;//读取SigningKey
        DateTime expire = DateTime.UtcNow.AddSeconds(jwtSetting.Value.ExpireSeconds);//有效期
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);//编码
        var signingKey = new SymmetricSecurityKey(keyBytes);//对称算法
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256Signature);//加密生成Credentials
        var tokenDescripor = new JwtSecurityToken(//issuer: issuer, audience: audience, 
        claims: claims, expires: expire, signingCredentials: credentials);//生成Token
        string token = new JwtSecurityTokenHandler().WriteToken(tokenDescripor);
        return token;
    }

}
