using AyiHockWebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AyiHockWebAPI.ValidationAttributes
{
    public class ValidJtiRequirement: IAuthorizationRequirement
    {
        public class ValidJtiHandler:AuthorizationHandler<ValidJtiRequirement>
        {
            private readonly d5qp1l4f2lmt76Context _ayihockDbContext;
            public ValidJtiHandler(d5qp1l4f2lmt76Context ayihockDbContext)
            {
                _ayihockDbContext = ayihockDbContext;
            }

            protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ValidJtiRequirement requirement)
            {
                //檢查Jti是否存在
                var jti = context.User.FindFirst("jti")?.Value; 
                if (jti == null)
                {
                    context.Fail(); //驗證失敗
                    return Task.CompletedTask;
                }

                //檢查Jti是否在黑名單 _ayihockDbContext.Jtiblacklists.Where(x => x.Jti == jti).Count() > 0;
                var tokenExists = _ayihockDbContext.Jtiblacklists.Any(r => r.Jti == jti);
                if (tokenExists)
                    context.Fail();
                else
                    context.Succeed(requirement); //驗證成功

                return Task.CompletedTask;
            }
        }
    }
}
