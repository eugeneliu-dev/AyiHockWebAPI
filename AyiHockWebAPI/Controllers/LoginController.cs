using AyiHockWebAPI.Dtos;
using AyiHockWebAPI.Filters;
using AyiHockWebAPI.Helpers;
using AyiHockWebAPI.Models;
using AyiHockWebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AyiHockWebAPI.Controllers
{
    [Route("api/[controller]")]
    [TypeFilter(typeof(ResultFormatFilter))]
    [ApiController]
    public class LoginController : ControllerBase
    {
        //DbContext
        private readonly d5qp1l4f2lmt76Context _ayihockDbContext;
        
        // Services/Helpers
        private readonly JwtHelper _jwtHelper;
        private readonly LoginService _loginService;

        public LoginController(d5qp1l4f2lmt76Context ayihockDbContext, 
                               JwtHelper jwt,
                               LoginService loginService)
        {
            _ayihockDbContext = ayihockDbContext;
            _jwtHelper = jwt;
            _loginService = loginService;
        }

        /// <summary>
        /// 前台帳號登入(ApplyRole: anonymous)
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("signin")]
        public async Task<ActionResult<string>> SignIn([FromBody] LoginDto login)
        {
            LoginDtoWithRole loginDtoWithRole = await _loginService.ValidateUser(login);

            if (loginDtoWithRole != null)
            {
                //整理jti黑名單資料
                //bool ret = await _loginService.DeleteExpiredJti(30);

                //回傳JwtToken
                return Ok(_jwtHelper.GenerateToken(loginDtoWithRole.Email, loginDtoWithRole.Role, loginDtoWithRole.Name));
            }
            else
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// 後台帳號登入(ApplyRole: anonymous)
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("adminsignin")]
        public async Task<ActionResult<string>> AdminSignIn([FromBody] LoginDto login)
        {
            LoginDtoWithRole loginDtoWithRole = await _loginService.ValidateAdmin(login);

            if (loginDtoWithRole != null)
            {
                //整理jti黑名單資料
                //bool ret = await _loginService.DeleteExpiredJti(30);

                //回傳JwtToken
                return Ok(_jwtHelper.GenerateToken(loginDtoWithRole.Email, loginDtoWithRole.Role, loginDtoWithRole.Name));
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost("logout")]
        [Authorize]
        [Authorize("JtiRestraint")]
        [Authorize(Roles = "admin, staff")]
        public async Task<ActionResult> LogoutToken()
        {
            //var principal = _jwtHelper.GetPrincipalByAccessToken(token.Access);

            //if (principal == null)
            //    return Ok("false");

            var jti = User.Claims.FirstOrDefault(p => p.Type == JwtRegisteredClaimNames.Jti).Value.ToString();
            var expire = Convert.ToInt32(User.Claims.FirstOrDefault(p => p.Type == JwtRegisteredClaimNames.Exp).Value.ToString());

            await _loginService.SetJtiToBlackList(jti, expire);

            return NoContent();
        }

    }
}
