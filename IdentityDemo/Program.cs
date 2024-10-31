using ClassLibrary;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt=>
{
        var scheme = new OpenApiSecurityScheme()
        {
                Description = "Authorization header Add bearer+space+token",
                Reference = new OpenApiReference{Type= ReferenceType.SecurityScheme,Id="Authorization"},
                Scheme = "oauth2",Name = "Authorization",
                In = ParameterLocation.Header,Type=SecuritySchemeType.ApiKey,
        };
        opt.AddSecurityDefinition("Authorization",scheme);
        var requirement = new OpenApiSecurityRequirement();
        requirement[scheme] = new List<string>();
        opt.AddSecurityRequirement(requirement);
});//在Swagger界面添加Header值属性。

builder.Services.Configure<JWTSetting>(builder.Configuration.GetSection("JWT"));//JWTSetting配置
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt=>{
        var jwtSetting = builder.Configuration.GetSection("JWT").Get<JWTSetting>();
        byte[] keyBytes = Encoding.UTF8.GetBytes(jwtSetting.SigningKey);//密钥字符串编码格式化
        var signingKey = new SymmetricSecurityKey(keyBytes);
        opt.TokenValidationParameters=new TokenValidationParameters()
        {
                ValidateIssuer = false,//一个标志，指示是否应验证发行者。默认值为 true。
                ValidateAudience = false,//一个标志，指示是否应验证接收者。默认值为 true。
                ValidateLifetime = true,//验证Token有效期
                ValidateIssuerSigningKey = true,//验证签署的Token
                IssuerSigningKey = signingKey, //签名密钥
        };
});//配置JWT

builder.Services.Configure<MvcOptions>(opt=>{
        opt.Filters.Add<JWTVerCheckFilter>();
});//添加过滤器

var conStrBuilder = new SqlConnectionStringBuilder(
        builder.Configuration.GetConnectionString("SQLServerConnection"));
conStrBuilder.Password = builder.Configuration["Password"];
var connection = conStrBuilder.ConnectionString;
builder.Services.AddDbContext<EFCoreDBContext>(opt =>opt.UseSqlServer(connection));//配置数据库连接信息，Password值在UserSecrets里

builder.Services.AddDataProtection();//数据保护
builder.Services.AddIdentityCore<UserExtend>(options =>{
        options.Password.RequireDigit = true;//必须包含数字
        options.Password.RequireLowercase = false;//必须包含小写字母
        options.Password.RequireNonAlphanumeric = false;//必须包含非字母数字
        options.Password.RequireUppercase = false;//必须大写字母
        options.Password.RequiredLength = 3;//长度
        options.Password.RequiredUniqueChars = 3;//可重复唯一
        //options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultAuthenticatorProvider;//根据Token重置密码
        //options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;//邮件确认用户Token
        });
IdentityBuilder identityBuilder = new IdentityBuilder(typeof(UserExtend),typeof(IdentityRole),builder.Services);
identityBuilder.AddEntityFrameworkStores<EFCoreDBContext>()
        //.AddDefaultTokenProviders()重置密码生成双重验证的Token
        .AddUserManager<UserManager<UserExtend>>()
        .AddRoleManager<RoleManager<IdentityRole>>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();//启用身份验证。在CROS（跨域访问）后
app.UseAuthorization();//授权

app.MapControllers();

app.Run();
