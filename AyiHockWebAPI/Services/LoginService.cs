using AyiHockWebAPI.Dtos;
using AyiHockWebAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AyiHockWebAPI.Services
{
    public class LoginService
    {

        private readonly d5qp1l4f2lmt76Context _ayihockDbContext;
        private readonly IConfiguration _configuration;
        private readonly IConnectionMultiplexer _redis;

        public LoginService(d5qp1l4f2lmt76Context ayihockDbContext, IConfiguration config, IConnectionMultiplexer radis)
        {
            _ayihockDbContext = ayihockDbContext;
            _configuration = config;
            _redis = radis;
        }

        public async Task<LoginDtoWithRole> ValidateUser(LoginDto login)
        {
            if (!string.IsNullOrWhiteSpace(login.Email) && !string.IsNullOrWhiteSpace(login.Password))
            {
                var res = await (from a in _ayihockDbContext.Customers
                                 where a.Email == login.Email && a.Password == login.Password && a.Enable == true
                                 select new LoginDtoWithRole
                                 {
                                      Email = a.Email,
                                      Password = a.Password,
                                      Role = a.Role,
                                      Name = a.Name
                                 }).SingleOrDefaultAsync();
                return res;
            }
            else
            {
                return null;
            }
        }

        public async Task<LoginDtoWithRole> ValidateAdmin(LoginDto login)
        {
            if (!string.IsNullOrWhiteSpace(login.Email) && !string.IsNullOrWhiteSpace(login.Password))
            {
                var res = await (from a in _ayihockDbContext.Managers
                                 where a.Email == login.Email && a.Password == login.Password && a.Enable == true
                                 select new LoginDtoWithRole
                                 {
                                     Email = a.Email,
                                     Password = a.Password,
                                     Role = (a.IsAdmin == true) ? 11 : 10, //11:Admin, 10:Staff
                                     Name = a.Name
                                 }).SingleOrDefaultAsync();
                return res;
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> SetJtiToBlackList(string jti, int expire )
        {
            if (string.IsNullOrWhiteSpace(jti) || expire <= 0)
                return false;

            try
            {
                IDatabase cache = _redis.GetDatabase(0);
                cache.StringSet(jti, expire, TimeSpan.FromSeconds(expire - DateTimeOffset.Now.ToUnixTimeSeconds()), When.NotExists);
            }
            catch
            {
                //TODO: Catch Exception
                return false;
            }

            return true;
        }

    }
}
