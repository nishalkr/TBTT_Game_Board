using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;


namespace TBTT_Board.Infrastructure
{
    public class ValidateRequestActionFilter : IActionFilter
    {

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // do something before the action executes
            //var domainName = context.HttpContext.Request.Url
            var controllerName = context.ActionDescriptor.RouteValues["controller"].ToUpper();
            var actionName = context.ActionDescriptor.RouteValues["action"].ToUpper();
            var response = context.HttpContext.Response;

            if (controllerName == "BADMINTONCOURT")  {
                switch (actionName)
                {
                   case "ADDTOGAMEBOARD":
                        response.StatusCode = 401;
                        context.HttpContext.Response.Headers.Clear();
                        context.Result = new BadRequestObjectResult("Action NOT allowed");
                        return;
                        break;
                    default:
                        break;
                }
            }
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            // do something after the action executes
        }
    }
}
