using AyiHockWebAPI.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace AyiHockWebAPI.Middleware
{
    public class CatchExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CatchExceptionMiddleware> _logger;
        public CatchExceptionMiddleware(RequestDelegate next, ILogger<CatchExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleException(context, ex);
            }
        }

        private Task HandleException(HttpContext context, Exception ex)
        {
            if (!context.Response.HasStarted)
            {

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";
                var message = ex.GetType() != null ? ex.Message : "InternalServerError";

                _logger.LogError(ex, $"Ex massage: {message}, StackTrace: {ex.StackTrace}", ex);
                var result = JsonConvert.SerializeObject(new ResultDto
                {
                    Success = false,
                    Message = string.Format("({0}) - {1}", ((int)HttpStatusCode.InternalServerError).ToString(), ex.Message),
                    Data = null
                });


                return context.Response.WriteAsync(result);
            }
            else
            {
                return context.Response.WriteAsync(string.Empty);
            }

        }
    }
}
