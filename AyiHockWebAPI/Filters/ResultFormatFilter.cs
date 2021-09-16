﻿using AyiHockWebAPI.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AyiHockWebAPI.Filters
{
    public class ResultFormatFilter : Attribute, IAsyncResultFilter
    {
        
        private readonly ILogger<ResultFormatFilter> _logger;

        public ResultFormatFilter(ILogger<ResultFormatFilter> logger)
        {
            _logger = logger;
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (!(context.Result is EmptyResult))
            {
                context.HttpContext.Response.ContentType = "application/json";

                ResultDto result = new ResultDto();
                var statusCode = (context.Result as ObjectResult)?.StatusCode;

                if (statusCode == null)
                {
                    StatusCodeResult statusResult = context.Result as StatusCodeResult;
                    statusCode = statusResult?.StatusCode;
                }

                if (Is200SeriousStatusCode((int)statusCode))
                {
                    result.Success = true;
                    result.Message = statusCode.ToString();
                    result.Data = (context.Result as ObjectResult)?.Value;
                }
                else
                {
                    result.Success = false;
                    result.Message = statusCode.ToString();
                    result.Data = null;
                }

                await context.HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(result));
                
                await next();

                _logger.LogInformation("StatusCode:" + statusCode.ToString());
            }
            else
            {
                context.Cancel = true;
            }
        }

        private bool Is200SeriousStatusCode(int code)
        {
            if (code >= 200 && code < 300)
                return true;
            else
                return false;
        }

    }
}
