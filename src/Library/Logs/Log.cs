using System;
using System.IO;
using NLog;
using NLog.Config;

namespace Library.Logs
{
    public class Log : IDisposable
    {
        private readonly Logger _logger;
        private static bool _isEnable;





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

        public static void EnableLogging(bool enable)
        {
            _isEnable = enable;
        }


        public void Info(string message)
        {
            if(!_isEnable)
                return;

            _logger.Info(message);
        }

        public void Debug(string message)
        {
            if (!_isEnable)
                return;

            _logger.Debug(message);
        }

        public void Error(string message)
        {
            if (!_isEnable)
                return;

            _logger.Error(message);
        }

        public void Fatal(string message)
        {
            if (!_isEnable)
                return;

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