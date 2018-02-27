using System;
using System.IO;
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
            var pathConfigFile = Path.Combine(Directory.GetCurrentDirectory(), "NLog.config");
            if (!File.Exists(pathConfigFile))
            {
                throw new FileNotFoundException($"Не найден файл конфигурации лога по пути: {pathConfigFile}");
            }
 
              LogManager.Configuration = new XmlLoggingConfiguration(pathConfigFile);
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