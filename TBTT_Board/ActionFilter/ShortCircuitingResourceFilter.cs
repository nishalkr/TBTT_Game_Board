using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace TBTT_Board.ActionFilter
{
    public class ShortCircuitingResourceFilter : Attribute, IResourceFilter
    {
 
     public void OnResourceExecuting(ResourceExecutingContext context)
        {

            var readOnlyDomainName = "ngrok.io";
            var domainName = context.HttpContext.Request.Host.ToString();
            var controllerName = context.ActionDescriptor.RouteValues["controller"].ToUpper();
            var actionName = context.ActionDescriptor.RouteValues["action"].ToUpper();

            if ((domainName != null) && (readOnlyDomainName!=null) && (readOnlyDomainName.Length>0) && (domainName.IndexOf(readOnlyDomainName) > 0))
            {
                if ((controllerName == "BADMINTONCOURT") || (controllerName == "HOME"))
                {
                    switch (actionName)
                    {
                        case "ADDTOGAMEBOARD":
                            context.Result = new ContentResult()
                            {
                                Content = "UnAuthorized Action."
                            };

                            break;
                        case "ADDTOAVAILABLELIST":
                            context.Result = new ContentResult()
                            {
                                Content = "UnAuthorized Action."
                            };

                            break;
                        case "ADDTOWAITINGLIST":
                            context.Result = new ContentResult()
                            {
                                Content = "UnAuthorized Action."
                            };

                            break;
                        default:
                            break;
                    }
                }
            }

           

        }
        public void OnResourceExecuted(ResourceExecutedContext context)
        {
        }

     
    }
}
