using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;

namespace JsReportTest.Filters
{
    /// <summary>
    /// Action处理前后 token的统一过滤处理
    /// </summary>
    public class TokenFilter : ActionFilterAttribute
    {
        /// <summary>
        /// 忽略的Action名称
        /// </summary>
        //private readonly string[] IgnoreActions = new string[] {  };
        private readonly string[] IgnoreActions = new string[] { "LabelFilterTest" };
        
        /// <summary>
        /// 过滤请求
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var action = context.RouteData.Values["action"];
            if (IgnoreActions.Contains(action?.ToString())) return;
            base.OnActionExecuting(context);

        }
        /// <summary>
        /// 过滤返回的结果集
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var action = context.RouteData.Values["action"];
            var controller = context.RouteData.Values["controller"];
            if (IgnoreActions.Contains(action?.ToString())) return;
            var value = (context.Result as ObjectResult)?.Value ?? (context.Result as JsonResult)?.Value ?? (context.Result as ViewResult)?.Model;
            context.Result = new OkObjectResult(value);
        }
    }
}
