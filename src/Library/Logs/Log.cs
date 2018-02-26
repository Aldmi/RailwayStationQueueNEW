using System;
using NLog;
using NLog.Config;

namespace Library.Logs
{
    public class Log : IDisposable
    {
        private readonly Logger _logger;



        #region ctor

        public Log(string nameLog)
        {
            LogManager.Configuration = new XmlLoggingConfiguration("C:\\Git\\RailwayStationQueueNEW\\src\\Library\\NLog.config");
            _logger = LogManager.GetLogger(nameLog);
        }

        #endregion





        #region Methode

        public void Info(string message)
        {
            _logger.Info(message);
        }

        public void Debug(string message)
        {
            _logger.Debug(message);
        }

        public void Error(string message)
        {
            _logger.Error(message);
        }

        public void Fatal(string message)
        {
            _logger.Fatal(message);
        }

        #endregion


        #region Dispose

        public void Dispose()
        {
            LogManager.DisableLogging().Dispose();
        }

        #endregion
    }
}