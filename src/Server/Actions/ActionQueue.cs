using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Server.Actions
{

    /// <summary>
    /// ОЧЕРЕДЬ ДЕЙСТВИЙ
    /// 
    /// </summary>
    public class ActionQueue
    {
        #region prop

        private CancellationTokenSource _cts;
        private ConcurrentQueue<ActionWrapper> Queue { get; } = new ConcurrentQueue<ActionWrapper>();
        public int ConstCyclePeriod { get; set; }

        #endregion




        #region Methode

        public async Task Start()
        {
            _cts = new CancellationTokenSource();
            await Task.Run(async () =>
            {
               await CycleInvoke();
            },_cts.Token);
        }


        public void Stop()
        {
            _cts?.Cancel();
        }


        /// <summary>
        /// Добавление элемента в очередь
        /// </summary>
        public void Enqueue(ActionWrapper act)
        {
            Queue.Enqueue(act);
        }


        /// <summary>
        /// Циклическое разматывание очереди.
        /// </summary>
        private async Task CycleInvoke()
        {
            while (!_cts.IsCancellationRequested)
            {
                ActionWrapper act;
                if (Queue.TryDequeue(out act))
                {
                    await act.Invoke(_cts.Token);
                    await Task.Delay(ConstCyclePeriod, _cts.Token);
                }
            }           
        }

        #endregion
    }
}