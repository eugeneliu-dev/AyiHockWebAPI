using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AyiHockWebAPI.Helpers
{
    public class JwtHelper
    {
        private readonly IConfiguration Configuration;

        public JwtHelper(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }
        public string GenerateToken(string account, int role, string name, int platform)
        {
            var issuer = Configuration.GetValue<string>("JwtSettings:Issuer");
            var signKey = Configuration.GetValue<string>("JwtSettings:SignKey");
            var expireMinutes = Configuration.GetValue<int>("JwtSettings:ExpireMinutes");

            // 設定要加入到 JWT Token 中的聲明資訊(Claims)
            var claims = new List<Claim>();

            // 在 RFC 7519 規格中(Section#4)，總共定義了 7 個預設的 Claims，我們應該只用的到兩種！
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, account)); // User.Identity.Name
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())); // JWT ID

            // 你可以自行擴充 "roles" 加入登入者該有的角色
            claims.Add(new Claim(ClaimTypes.Role, _GetRoleName(role)));
            claims.Add(new Claim(ClaimTypes.Name, name));
            claims.Add(new Claim("platform", platform.ToString()));

            var userClaimsIdentity = new ClaimsIdentity(claims);

            // 建立一組對稱式加密的金鑰，主要用於 JWT 簽章之用
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signKey));

            // HmacSha256 有要求必須要大於 128 bits，所以 key 不能太短，至少要 16 字元以上
            // https://stackoverflow.com/questions/47279947/idx10603-the-algorithm-hs256-requires-the-securitykey-keysize-to-be-greater
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            // 建立 SecurityTokenDescriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = issuer,
                Subject = userClaimsIdentity,
                Expires = DateTime.Now.AddMinutes(expireMinutes),
                SigningCredentials = signingCredentials
            };

            // 產出所需要的 JWT securityToken 物件，並取得序列化後的 Token 結果(字串格式)
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var serializeToken = tokenHandler.WriteToken(securityToken);

            return serializeToken;
        }

        public string GenerateRefreshToken(string account, string userId)
        {
            var issuer = Configuration.GetValue<string>("JwtSettings:RefreshIssuer");
            var signKey = Configuration.GetValue<string>("JwtSettings:SignKey");
            var expireMinutes = Configuration.GetValue<int>("JwtSettings:RefreshExpireMinutes");

            // 設定要加入到 JWT Token 中的聲明資訊(Claims)
            var claims = new List<Claim>();

            // 在 RFC 7519 規格中(Section#4)，總共定義了 7 個預設的 Claims，我們應該只用的到兩種！
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, account)); // User.Identity.Name
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())); // JWT ID

            var userClaimsIdentity = new ClaimsIdentity(claims);

            // 建立一組對稱式加密的金鑰，主要用於 JWT 簽章之用
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signKey));

            // HmacSha256 有要求必須要大於 128 bits，所以 key 不能太短，至少要 16 字元以上
            // https://stackoverflow.com/questions/47279947/idx10603-the-algorithm-hs256-requires-the-securitykey-keysize-to-be-greater
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            // 建立 SecurityTokenDescriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = issuer,
                Subject = userClaimsIdentity,
                Expires = DateTime.Now.AddMinutes(expireMinutes),
                SigningCredentials = signingCredentials
            };

            // 產出所需要的 JWT securityToken 物件，並取得序列化後的 Token 結果(字串格式)
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var serializeToken = tokenHandler.WriteToken(securityToken);

            return serializeToken;
        }

        public ClaimsPrincipal GetPrincipalByAccessToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var signKey = Configuration.GetValue<string>("JwtSettings:SignKey");

            try
            {
                return handler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = true,
                    ValidIssuer = Configuration.GetValue<string>("JwtSettings:Issuer"),
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signKey)),
                    ValidateLifetime = true
                }, out SecurityToken validatedToken);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private string _GetRoleName(int role)
        {
            string retStr = "normal";
            switch (role)
            {
                case 0:
                    retStr = "normal";
                    break;
                case 1:
                    retStr = "golden";
                    break;
                case 2:
                    retStr = "platinum";
                    break;
                case 3:
                    retStr = "diamond";
                    break;
                case 10:
                    retStr = "staff";
                    break;
                case 11:
                    retStr = "admin";
                    break;
                default:
                    break;
            }

            return retStr;
        }

    }
}
