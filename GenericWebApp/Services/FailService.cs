using System;
using NLog;

namespace GenericWebApp.Services
{
    public class FailService : IFail
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        public void YoloAndFail()
        {
            _log.Info("YOLO!");
            throw new Exception("Oh my! This is awkward...");
        }
    }
}