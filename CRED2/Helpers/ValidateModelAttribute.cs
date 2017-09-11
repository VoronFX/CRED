using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRED2.GitBridge;
using CRED2.Model;
using CRED2.Model.DTOs;
using LiteDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace CRED2
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                context.Result = new BadRequestObjectResult(context.ModelState);
            }
        }
    }
}