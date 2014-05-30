using System.Web;
using System.Web.Mvc;
using NLog;

namespace GenericWebApp
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new ExceptionLoggerAttribute());
        }
    }

    public class ExceptionLoggerAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            var logger = LogManager.GetLogger(filterContext.Controller.ControllerContext.Controller.GetType().FullName);

            logger.ErrorException("Unhandled error in a controller", filterContext.Exception);
        }
    }
}
