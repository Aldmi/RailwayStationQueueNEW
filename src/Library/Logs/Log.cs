using System;
using System.Collections.Generic;
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


        /// <summary>
        /// Добавляет логирования кастомного свойства в отдельную колонку (csv)
        /// Например: <column name="Дата добавления в очередь" layout="${event-context:item=DateAdded2Queue}" />
        /// </summary>
        /// <param name="dict"></param>
        public void LogEventContext(Dictionary<string, object> dict)
        {
            var myEvent = new LogEventInfo(LogLevel.Info, _logger.Name, string.Empty);
            foreach (var kvp in dict)
            {
                myEvent.Properties.Add(kvp.Key, kvp.Value);
            }
            _logger.Log(myEvent);
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