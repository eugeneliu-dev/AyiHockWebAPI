using AyiHockWebAPI.Dtos;
using AyiHockWebAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AyiHockWebAPI.Services
{
    public class LoginService
    {

        private readonly d5qp1l4f2lmt76Context _ayihockDbContext;
        public LoginService(d5qp1l4f2lmt76Context ayihockDbContext)
        {
            _ayihockDbContext = ayihockDbContext;
        }
        public async Task<bool> DeleteExpiredJti(int expiredTime)
        {
            try
            {
                var expiredJtis = _ayihockDbContext.Jtiblacklists.Where(x => x.Expire.AddMinutes(expiredTime) < DateTime.Now);
                _ayihockDbContext.Jtiblacklists.RemoveRange(expiredJtis);
                await _ayihockDbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
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

        public async Task<bool> SetJtiToBlackList(string jti, int expire )
        {
            if (string.IsNullOrWhiteSpace(jti) || expire <= 0)
                return false;

            if (await IsJtiInBlackList(jti))
                return true;

            try
            {
                DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                dateTime = dateTime.AddSeconds(expire).ToLocalTime();

                _ayihockDbContext.Jtiblacklists.Add(new Jtiblacklist { Jti = jti, Expire = dateTime });
                await _ayihockDbContext.SaveChangesAsync();
            }
            catch
            {
                //TODO: Catch Exception
                return false;
            }

            return true;
        }

        public async Task<bool> IsJtiInBlackList(string jti)
        {
            return await _ayihockDbContext.Jtiblacklists.Where(x => x.Jti == jti).CountAsync() > 0;
        }

    }
}
