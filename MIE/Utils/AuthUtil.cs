using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MIE.Entity;

namespace MIE.Utils
{
    public class AuthUtil : IAuthUtil
    {
        private readonly MySQLDbContext context;
        private readonly IConfiguration config;
        private readonly IHttpContextAccessor httpContextAccessor;

        public AuthUtil(MySQLDbContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            this.config = configuration;
            this.httpContextAccessor = httpContextAccessor;
        }

        public string GetToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:SecretKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            //创建用户身份标识，可按需要添加更多信息
            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("id", user.UserId.ToString(), ClaimValueTypes.Integer32), // 用户id
                new Claim("name", user.Username) // 用户名
            };

            //创建令牌
            var token = new JwtSecurityToken(
              issuer: config["jwt:Issuer"],
              audience: config["jwt:Audience"],
              signingCredentials: credentials,
              claims: claims,
              notBefore: DateTime.Now,
              expires: DateTime.Now.AddDays(30)
            );

            string jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            return jwtToken;
        }

        public int GetIdFromToken()
        {
            var nameId = httpContextAccessor.HttpContext.User.FindFirst("id");
            return nameId != null ? Convert.ToInt32(nameId.Value) : 0;
        }
    }
}
