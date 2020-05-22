using System.Collections.Generic;
using CSharpFunctionalExtensions;

namespace Terminal.Model
{
    public class PrefixesMapping2QueueModel
    {
        private readonly Dictionary<string, string> _prefixMapping;


        #region ctor
        public PrefixesMapping2QueueModel(Dictionary<string, string> prefixMapping)
        {
            _prefixMapping = prefixMapping;
        }
        #endregion


        public Result<string> GetQueueName(string prefix)
        {
            return _prefixMapping.TryGetValue(prefix, out var queueName) 
                ? Result.Ok(queueName) 
                : Result.Failure<string>($"Не найденно соответствие для ПРЕФИКСА: \"{prefix}\"");
        }
    }
}