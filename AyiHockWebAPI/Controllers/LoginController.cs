using AyiHockWebAPI.Dtos;
using AyiHockWebAPI.Filters;
using AyiHockWebAPI.Helpers;
using AyiHockWebAPI.Models;
using AyiHockWebAPI.Services;
using Google.Apis.Auth;
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
                               LoginService loginService,
                               CustomerService customerService)
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
                return Ok(_jwtHelper.GenerateToken(loginDtoWithRole.Email, loginDtoWithRole.Role, loginDtoWithRole.Name, (int)LoginPlatform.Original));
            }
            else
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// 前台Google帳號登入(ApplyRole: anonymous)
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("signin/google")]
        public async Task<ActionResult<string>> SignInWithSocial([FromBody] SocialUser user)
        {
            //驗證GoogleToken並取得Payload
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new List<string>() { "730644139443-1s6dsft5mu2kf7l4bg5jscggjs2lmr18.apps.googleusercontent.com" }
            };
           
            var payload = await GoogleJsonWebSignature.ValidateAsync(user.Token, settings);
            if (payload == null)
                return BadRequest("Google驗證失敗!");

            //檢查帳號是否存在 (Yes:取回相關資訊 ; No:將此帳號加入DB)
            //最後回傳Jwt
            LoginDtoForSocial loginDtoForSocial = await _loginService.ValidateSocialUser(payload.Email, LoginPlatform.Google);

            if (loginDtoForSocial != null)
            {
                if (!loginDtoForSocial.IsBlack && loginDtoForSocial.Enable)
                {
                    return Ok(_jwtHelper.GenerateToken(loginDtoForSocial.Email, loginDtoForSocial.Role, loginDtoForSocial.Name, (int)LoginPlatform.Google));
                }
                else
                {
                    return BadRequest("使用者為黑名單成員，無法操作!");
                }
            }
            else
            {
                var customer = await _loginService.AddSocialUser(payload.Name, payload.Email, LoginPlatform.Google);

                return Ok(_jwtHelper.GenerateToken(customer.Email, customer.Role, customer.Name, (int)LoginPlatform.Google));
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
                return Ok(_jwtHelper.GenerateToken(loginDtoWithRole.Email, loginDtoWithRole.Role, loginDtoWithRole.Name, (int)LoginPlatform.Original));
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost("logout")]
        [Authorize]
        [Authorize("JtiRestraint")]
        [Authorize(Roles = "admin, staff, normal, golden, platinum, diamond")]
        public async Task<ActionResult> LogoutToken()
        {
            //var principal = _jwtHelper.GetPrincipalByAccessToken(token.Access);

            //if (principal == null)
            //    return Ok("false");

            var jti = User.Claims.FirstOrDefault(p => p.Type == JwtRegisteredClaimNames.Jti).Value.ToString();
            var expire = Convert.ToInt32(User.Claims.FirstOrDefault(p => p.Type == JwtRegisteredClaimNames.Exp).Value.ToString());

            await _loginService.SetJtiToBlackList(jti, expire);

            return Ok();
        }

    }
}
