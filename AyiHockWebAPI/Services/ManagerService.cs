using AyiHockWebAPI.Dtos;
using AyiHockWebAPI.Helpers;
using AyiHockWebAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AyiHockWebAPI.Services
{
    public class ManagerService
    {
        private readonly d5qp1l4f2lmt76Context _ayihockDbContext;
        private readonly EncryptDecryptHelper _encryptDecryptHelper;
        private readonly AutoSendEmailHelper _autoSendEmailHelper;

        public ManagerService(d5qp1l4f2lmt76Context ayihockDbContext, IConfiguration configuration, EncryptDecryptHelper encryptDecryptHelper, AutoSendEmailHelper autoSendEmailHelper)
        {
            _ayihockDbContext = ayihockDbContext;
            _encryptDecryptHelper = encryptDecryptHelper;
            _autoSendEmailHelper = autoSendEmailHelper;
        }

        public Manager GetManagerFullInfoByMail(string mail)
        {
            var manager = (from a in _ayihockDbContext.Managers
                            where a.Email == mail
                            select a).SingleOrDefault();
            return manager;
        }

        public Manager GetManagerFullInfoById(Guid id)
        {
            var manager = (from a in _ayihockDbContext.Managers
                            where a.ManagerId == id
                            select a).SingleOrDefault();
            return manager;
        }

        public Manager GetManagerFullInfoByOldPassword(string mail, string oldPassword)
        {
            var manager = (from a in _ayihockDbContext.Managers
                            where a.Email == mail && a.Password == _encryptDecryptHelper.AESDecrypt(oldPassword).Replace("\"", "")
                            select a).SingleOrDefault();
            return manager;
        }

        public async Task<List<ManagerGetTotalInfoDto>> GetManagersListTotalInfo()
        {
            var managers = await (from a in _ayihockDbContext.Managers
                                   where a.IsAdmin == false
                                   select new ManagerGetTotalInfoDto
                                   {
                                       ManagerId = a.ManagerId,
                                       Name = a.Name,
                                       Email = a.Email,
                                       Phone = a.Phone,
                                       Enable = a.Enable,
                                       IsAdmin = a.IsAdmin
                                   }).OrderBy(a => a.Enable).ToListAsync();
            return managers;
        }

        public async Task<ManagerGetTotalInfoDto> GetManagerTotalInfo(Guid id)
        {
            var manager = await (from a in _ayihockDbContext.Managers
                                 where a.IsAdmin == false && a.ManagerId == id
                                 select new ManagerGetTotalInfoDto
                                 {
                                     ManagerId = a.ManagerId,
                                     Name = a.Name,
                                     Email = a.Email,
                                     Phone = a.Phone,
                                     Enable = a.Enable,
                                     IsAdmin = a.IsAdmin
                                 }).SingleOrDefaultAsync();
            return manager;
        }

        public async Task<ManagerGetBasicInfoDto> GetManagerBasicInfo(string userEmail)
        {
            var manager = await (from a in _ayihockDbContext.Managers
                                 where a.Email == userEmail
                                 select new ManagerGetBasicInfoDto
                                 {
                                     Name = a.Name,
                                     Email = a.Email,
                                     Phone = a.Phone,
                                     IsAdmin = a.IsAdmin
                                 }).SingleOrDefaultAsync();
            return manager;
        }

        public async Task PostManager(ManagerPostDto value)
        {
            Guid guid = Guid.NewGuid();

            Manager manager = new Manager
            {
                ManagerId = guid,
                Name = value.Name,
                Email = value.Email,
                Password = _encryptDecryptHelper.AESDecrypt(value.Password).Replace("\"", ""), //需前端加密處理,後端解密
                Phone = value.Phone,
                Enable = true,
                IsAdmin = false
            };

            _ayihockDbContext.Managers.Add(manager);
            await _ayihockDbContext.SaveChangesAsync();
        }

        public async Task PutManager(Manager update, ManagerPutDto value)
        {
            update.Name = value.Name;
            update.Email = value.Email;
            update.Phone = value.Phone;
            update.Enable = value.Enable;

            await _ayihockDbContext.SaveChangesAsync();
        }

        public async Task PutManagerNewPassword(ManagerPutPwdDto value, Manager update)
        {
            update.Password = _encryptDecryptHelper.AESDecrypt(value.NewPassword).Replace("\"", "");  //需前端加密處理,後端解密
            await _ayihockDbContext.SaveChangesAsync();
        }

        public async Task PutManagerResetPassword(string managerMail, Manager update)
        {
            string newPwd = _encryptDecryptHelper.GetRandomStr();
            update.Password = newPwd;

            await _ayihockDbContext.SaveChangesAsync();
            _autoSendEmailHelper.SendAuthEmail(managerMail, "新密碼:" + newPwd);
        }

        public async Task<Manager> DeleteManager(Guid id)
        {
            var delete = GetManagerFullInfoById(id);
            if (delete == null)
                return null;

            _ayihockDbContext.Managers.Remove(delete);
            await _ayihockDbContext.SaveChangesAsync();
            return delete;
        }





    }
}
