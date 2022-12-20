using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Marketplace.SaaS.Accelerator.Services.Utilities;
public class RequestLoggerActionFilter : IAsyncActionFilter
{
    private readonly ILogger<RequestLoggerActionFilter> _logger;
    public RequestLoggerActionFilter(ILogger<RequestLoggerActionFilter> logger)
    {
        _logger = logger;
    }
   
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var controllerContext = context.ActionDescriptor as ControllerActionDescriptor;
        if (controllerContext == null)
        {
            _logger.LogInformation($"Executing {context.ActionDescriptor.ToString()}");
        }
        else
        {
            var parametersValue = context.ActionArguments.Select(arg => $"{arg.Key}:{JsonSerializer.Serialize(arg.Value)}");
            var parametersString = parametersValue.Any() ? $"Parameters: {string.Join("::", parametersValue)}" : string.Empty;

            _logger.LogInformation($"Executing {controllerContext.ControllerName} / {controllerContext.ActionName}. {parametersString}");
        }

        await next();
    }

}
