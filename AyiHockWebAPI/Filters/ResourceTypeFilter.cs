using AyiHockWebAPI.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AyiHockWebAPI.Filters
{
    public class ResourceTypeFilter : Attribute, IResourceFilter
    {
        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            //throw new NotImplementedException();
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            var files = context.HttpContext.Request.Form.Files;

            foreach (var file in files)
            {
                if (Path.GetExtension(file.FileName) != ".jpg" &&
                    Path.GetExtension(file.FileName) != ".bmp" &&
                    Path.GetExtension(file.FileName) != ".png"  )
                {
                    context.Result = new JsonResult(new ResultDto
                    {
                        Success = false,
                        Message = "400-檔案格式錯誤",
                        Data = null
                    });
                }
            }
        }
    }
}
