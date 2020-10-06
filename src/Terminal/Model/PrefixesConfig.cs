using System;
using System.Collections.Generic;
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
        public WorkTime WorkTime { get; }

        public PrefixeConf(string prefix, string queueName, WorkTime workTime)
        {
            Prefix = prefix;
            QueueName = queueName;
            WorkTime = workTime;
        }

        /// <summary>
        /// Проверка диапазона запрещенного времени.
        /// Если WorkTime не указан, то разрешенн любой диапазон
        /// </summary>
        public (WorkTime workTime , bool isPermited) CheckPermitTime()
        {
            if(WorkTime == null)
                return (null, false);

            return WorkTime.CheckPermitTime() ? (WorkTime, true): (WorkTime, false);
        }

    }

    public class WorkTime
    {
        public TimeSpan StartTime { get; }
        public TimeSpan StopTime { get; }

        public WorkTime(TimeSpan startTime, TimeSpan stopTime)
        {
            StartTime = startTime;
            StopTime = stopTime;
        }

        public static WorkTime Parse(string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            //TODO: парсить строку времени

            return new WorkTime(TimeSpan.MaxValue, TimeSpan.MaxValue);
        }

        /// <summary>
        /// true - запрет
        /// false - разрешенно
        /// </summary>
        public bool CheckPermitTime()
        {
            var now = DateTime.Now.TimeOfDay;
            return now <= StartTime || now >= StopTime;
        }


        public override string ToString()
        {
            return $"Касса не работает с: {StartTime:hh\\:mm}   до: {StopTime:hh\\:mm}";
        }
    }
}