using System.Web.Mvc;
using GenericWebApp.Services;
using NLog;

namespace GenericWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        public ActionResult Index()
        {
            _log.Debug("Getting #Index");
            return View();
        }

        public ActionResult About()
        {
            _log.Debug("Getting #About");

            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            _log.Debug("Getting #Contact");

            ViewBag.Message = "Your contact page.";
            return View();
        }

        public ActionResult Yolo()
        {
            var failService = new FailService();
            failService.YoloAndFail();
            
            return new EmptyResult();
        }
    }
}