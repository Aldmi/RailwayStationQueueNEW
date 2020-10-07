using System;
using System.Collections.Generic;
using System.Linq;
using CSharpFunctionalExtensions;

namespace Terminal.Model
{
    public class PrefixesConfig
    {
        private readonly Dictionary<string, PrefixeConf> _prefixDict;
        public PrefixesConfig(Dictionary<string, PrefixeConf> prefixDict)
        {
            _prefixDict = prefixDict;
        }
        
        public Result<PrefixeConf> GetConf(string prefix)
        {
            return _prefixDict.TryGetValue(prefix, out var queueName) 
                ? Result.Ok(queueName) 
                : Result.Failure<PrefixeConf>($"Не найденно соответствие для ПРЕФИКСА: \"{prefix}\"");
        }
    }


    public class PrefixeConf
    {
        public string Prefix { get;  }
        public string QueueName { get; }
        public List<PermitTime> PermitTimes { get; }

        public PrefixeConf(string prefix, string queueName, List<PermitTime> permitTimes)
        {
            Prefix = prefix;
            QueueName = queueName;
            PermitTimes = permitTimes;
        }

        /// <summary>
        /// Проверка диапазона запрещенного времени.
        /// Если попали хотя бы в 1 ЗАПРЕЩЕННЫЙ диапазон вернуть PermitTime.
        /// Если в диапазон не попали вернуть null.
        /// </summary>
        public PermitTime CheckPermitRange()
        {
            //Если попали хотя бы в 1 ЗАПРЕЩЕННЫЙ диапазон
            var wtPermited = PermitTimes?.FirstOrDefault(wt => wt.CheckPermit());
            return wtPermited;
        }
    }


    /// <summary>
    /// Задает запрещенный диапазон.
    /// </summary>
    public class PermitTime
    {
        public TimeSpan StartTime { get; }
        public TimeSpan StopTime { get; }
        public string PermitMessage { get; }


        public PermitTime(TimeSpan startTime, TimeSpan stopTime, string permitMessage)
        {
            if(startTime > stopTime)
                throw new ArgumentException($"{startTime:hh\\:mm} не может быть меньше {stopTime:hh\\:mm}");

            if (string.IsNullOrEmpty(permitMessage))
                throw new ArgumentException("permitMessage не может быть пустым или NULL");
            
            StartTime = startTime;
            StopTime = stopTime;
            PermitMessage = permitMessage;
        }


        public static PermitTime Parse(string startStr, string stopStr, string permitMessage)
        {
            TimeSpan startTime;
            TimeSpan stopTime;
            try
            {
                startTime = TimeSpan.Parse(startStr.RemovingExtraSpaces());
                stopTime = TimeSpan.Parse(stopStr.RemovingExtraSpaces());
            }
            catch (Exception ex)
            {
                throw new FormatException($"PermitTimes '{startStr}' или {stopStr} не может быть распарсенна через TimeSpan.Parse  {ex.Message}");
            }
            return new PermitTime(startTime, stopTime, permitMessage);
        }


        public bool CheckPermit()
        {
            var now = DateTime.Now.TimeOfDay;
            return now >= StartTime && now <= StopTime;
        }

        public override string ToString()
        {
            try
            {
                var str = string.Format(PermitMessage, StartTime, StopTime);
                return str;
            }
            catch (Exception e)
            {
                throw new FormatException("PermitMessage содержит не верный формат подстановки ", e);
            }
        }
    }
}