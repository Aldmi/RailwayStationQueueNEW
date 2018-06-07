using System;
using System.Threading;
using System.Threading.Tasks;

namespace Server.Actions
{

    public class ActionFromCashier
    {
        #region prop

        //public CashierHandling CashierHandling { get; } //????
        private Func<CancellationToken, Task> CashierAct { get; }

        #endregion




        #region ctor

        public ActionFromCashier(Func<CancellationToken, Task> cashierAct)
        {
           // CashierHandling = cashierHandling;
            CashierAct = cashierAct;
        }

        #endregion



        /// <summary>
        /// Маркер завершения задачи
        /// Успешное завершение возвращается null
        /// Еслт ошибка возвращается Exception
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
                await CashierAct(token);
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