using AyiHockWebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using StackExchange.Redis;
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
            private readonly IConnectionMultiplexer _redis;
            public ValidJtiHandler(d5qp1l4f2lmt76Context ayihockDbContext, IConnectionMultiplexer radis)
            {
                _ayihockDbContext = ayihockDbContext;
                _redis = radis;
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

                IDatabase cache = _redis.GetDatabase(0);
                var tokenExists = cache.KeyExists(jti);

                if (tokenExists)
                    context.Fail();
                else
                    context.Succeed(requirement); //驗證成功

                return Task.CompletedTask;
            }
        }
    }
}
