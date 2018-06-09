using System;
using System.Threading;
using System.Threading.Tasks;

namespace Server.Actions
{
    /// <summary>
    /// ОБЕРТКА НАД ДЕЙСТВИЕМ.
    /// Ответ о выполнении действия можно дожидаться с помощью await MarkerEndAction()
    /// </summary>
    public class ActionWrapper
    {
        #region prop

        private Func<CancellationToken, Task> Act { get; }

        #endregion




        #region ctor

        public ActionWrapper(Func<CancellationToken, Task> act)
        {
            Act = act;
        }

        #endregion



        /// <summary>
        /// Маркер завершения задачи
        /// Успешное завершение возвращается null
        /// Если ошибка -  возвращается Exception
        /// </summary>
        private TaskCompletionSource<Exception> _tcs;
        public Task<Exception> MarkerEndAction()
        {
            _tcs = new TaskCompletionSource<Exception>();
            return _tcs.Task;
        }


        /// <summary>
        /// Обертка над задачей CashierAct
        /// Выполянть на очереди.
        /// </summary>
        public async Task Invoke(CancellationToken token)
        {
            try
            {
                await Act(token);
                _tcs.TrySetResult(null);
            }
            catch (Exception ex)
            {
                _tcs.TrySetResult(ex);
               // throw;
            }        
        }

    }
}