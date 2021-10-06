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
    public class CustomerService
    {
        private readonly d5qp1l4f2lmt76Context _ayihockDbContext;
        private readonly IConfiguration _configuration;
        private readonly EncryptDecryptHelper _encryptDecryptHelper;
        private readonly AutoSendEmailHelper _autoSendEmailHelper;

        public CustomerService(d5qp1l4f2lmt76Context ayihockDbContext, IConfiguration configuration, EncryptDecryptHelper encryptDecryptHelper, AutoSendEmailHelper autoSendEmailHelper)
        {
            _ayihockDbContext = ayihockDbContext;
            _configuration = configuration;
            _encryptDecryptHelper = encryptDecryptHelper;
            _autoSendEmailHelper = autoSendEmailHelper;
        }

        public Manager GetManagerInfo(string sub)
        {
            return _ayihockDbContext.Managers.Where(mgr => sub == mgr.Email).Select(mgr => mgr).FirstOrDefault();
        }

        public Customer GetCustomerFullInfoByMail(string mail)
        {
            var customer = (from a in _ayihockDbContext.Customers
                            where a.Email == mail && a.Platform == (int)LoginPlatform.Original
                            select a).SingleOrDefault();
            return customer;
        }

        public Customer GetCustomerFullInfoById(Guid id)
        {
            var customer = (from a in _ayihockDbContext.Customers
                            where a.CustomerId == id
                            select a).SingleOrDefault();
            return customer;
        }

        public Customer GetCustomerFullInfoByOldPassword(string mail, string oldPassword)
        {
            var customer = (from a in _ayihockDbContext.Customers
                            where a.Email == mail && a.Platform == (int)LoginPlatform.Original && a.Password == _encryptDecryptHelper.AESDecrypt(oldPassword).Replace("\"", "")
                            select a).SingleOrDefault();
            return customer;
        }

        public Customer GetCustomerFullInfoByPrePassword(string mail, string prePassword)
        {
            int randomCodeLength = _configuration.GetValue<int>("Encrypt:RandomCodeLength");
            var customer = (from a in _ayihockDbContext.Customers
                            where a.Email == mail && a.Platform == (int)LoginPlatform.Original && a.PrePassword.Length == randomCodeLength && a.PrePassword == prePassword
                            select a).SingleOrDefault();
            return customer;
        }

        public async Task<List<CustomerGetDto>> GetCustomerList()
        {
            var customers = await (from a in _ayihockDbContext.Customers
                                   select new CustomerGetDto
                                   {
                                       CustomerId = a.CustomerId,
                                       Name = a.Name,
                                       Email = a.Email,
                                       Phone = a.Phone,
                                       Enable = a.Enable,
                                       Role = a.Role,
                                       Isblack = a.Isblack,
                                       Money = a.Money,
                                       Modifier = a.Modifier,
                                       CreateTime = a.CreateTime,
                                       ModifyTime = a.ModifyTime,
                                       Platform = a.Platform
                                   }).OrderBy(a => a.CreateTime).ToListAsync();
            return customers;
        }

        public async Task<CustomerGetDto> GetCustomerFromManager(Guid id)
        {
            var customerById = await (from a in _ayihockDbContext.Customers
                                      where a.CustomerId == id
                                      select new CustomerGetDto
                                      {
                                          CustomerId = a.CustomerId,
                                          Name = a.Name,
                                          Email = a.Email,
                                          Phone = a.Phone,
                                          Enable = a.Enable,
                                          Role = a.Role,
                                          Isblack = a.Isblack,
                                          Money = a.Money,
                                          Modifier = a.Modifier,
                                          CreateTime = a.CreateTime,
                                          ModifyTime = a.ModifyTime,
                                          Platform = a.Platform
                                      }).SingleOrDefaultAsync();

            return customerById;
        }

        public async Task<CustomerGetByUserDto> GetCustomerFromUser(string sub)
        {
            var customerByMail = await (from a in _ayihockDbContext.Customers
                                        where a.Email == sub && a.Platform == (int)LoginPlatform.Original
                                        select new CustomerGetByUserDto
                                        {
                                            Name = a.Name,
                                            Email = a.Email,
                                            Phone = a.Phone,
                                            Enable = a.Enable,
                                            Role = a.Role,
                                            ModifyTime = a.ModifyTime
                                        }).SingleOrDefaultAsync();

            return customerByMail;
        }

        public async Task PostCustomer(CustomerPostDto value)
        {
            Guid guid = Guid.NewGuid();

            Customer customer = new Customer
            {
                CustomerId = guid,
                Name = value.Name,
                Email = value.Email,
                Password = _encryptDecryptHelper.AESDecrypt(value.Password).Replace("\"", ""), //需前端加密處理,後端解密
                Phone = value.Phone,
                Enable = false,
                Isblack = false,
                Modifier = guid,
                CreateTime = DateTime.Now,
                ModifyTime = DateTime.Now
            };

            _ayihockDbContext.Customers.Add(customer);
            await _ayihockDbContext.SaveChangesAsync();

            string apiRoot = _configuration.GetValue<string>("URL:ApiRoot");
            string authLink = "連結如下:\n" + apiRoot + "/api/customer/auth?varify=" + _encryptDecryptHelper.AESEncrypt(guid.ToString());
            _autoSendEmailHelper.SendAuthEmail(value.Email, authLink);
        }

        public async Task<Customer> AuthCustomer(string varify)
        {
            //trans varifyStr to Guid
            Guid guid = Guid.Parse(_encryptDecryptHelper.AESDecrypt(varify));

            //check Guid is exist?
            var customer = await (from a in _ayihockDbContext.Customers
                                  where a.CustomerId == guid && a.Enable == false
                                  select a).SingleOrDefaultAsync();

            if (customer == null)
            {
                return null;
            }
            else
            {
                //modify Enable to 'true'
                customer.Enable = true;
                await _ayihockDbContext.SaveChangesAsync();

                string url = _configuration.GetValue<string>("URL:Login");
                System.Diagnostics.Process.Start("explorer", url);

                return customer;
            }
        }

        public async Task PutCustomerFromManager(Guid modifierId, CustomerPutDto value, Customer update)
        {
            update.CustomerId = value.CustomerId;
            update.Enable = value.Enable;
            update.Role = value.Role;
            update.Isblack = value.Isblack;
            update.ModifyTime = value.ModifyTime;
            update.Modifier = modifierId;

            await _ayihockDbContext.SaveChangesAsync();
        }

        public async Task PutCustomerFromUser(CustomerPutByUserDto value, Customer update)
        {
            update.Name = value.Name;
            update.Phone = value.Phone;
            update.ModifyTime = DateTime.Now;
            update.Modifier = update.CustomerId;

            await _ayihockDbContext.SaveChangesAsync();
        }

        public async Task PutCustomerNewPassword(string newPassword, Customer update)
        {
            update.Password = _encryptDecryptHelper.AESDecrypt(newPassword).Replace("\"", "");  //需前端加密處理,後端解密
            update.PrePassword = "";

            await _ayihockDbContext.SaveChangesAsync();
        }

        public async Task PutCustomerResetPassword(string userMail, Customer update)
        {
            string prePwd = _encryptDecryptHelper.GetRandomStr();
            update.PrePassword = prePwd;

            await _ayihockDbContext.SaveChangesAsync();
            string resetPwdUri = _configuration.GetValue<string>("URL:ResetPwd");
            string authLink = string.Format("驗證碼: {0}，請至以下連結更改密碼\n，{1}", prePwd, resetPwdUri);
            _autoSendEmailHelper.SendAuthEmail(userMail, authLink);
        }

        public async Task<Customer> DeleteCustomer(Guid id)
        {
            var delete = GetCustomerFullInfoById(id);
            if (delete == null)
                return null;

            _ayihockDbContext.Customers.Remove(delete);
            await _ayihockDbContext.SaveChangesAsync();
            return delete;
        }

    }
}
