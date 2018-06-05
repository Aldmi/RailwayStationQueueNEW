using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Server.Infrastructure;

namespace Server.Actions
{

    public class ActionFromCashier
    {
        #region prop

        public CashierHandling CashierHandling { get; } //????
        private Func<Task> CashierAct { get; }

        #endregion




        #region ctor

        public ActionFromCashier(CashierHandling cashierHandling, Func<Task> cashierAct)
        {
            CashierHandling = cashierHandling;
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
        public async Task Invoke()
        {
            try
            {
                await CashierAct();
                _tcs.TrySetResult(null);
            }
            catch (Exception ex)
            {
                _tcs.TrySetResult(ex);
               // throw;
            }        
        }


        //private TaskCompletionSource<bool> _tcs;
        //private Task<bool> PlayWithControl()
        //{
        //    _tcs = new TaskCompletionSource<bool>();
        //    Task.Run(async () =>
        //    {
        //        if (_currentCallId != null)
        //        {
        //            try
        //            {
        //                _praesideoOi.startCreatedCall(_currentCallId.Value);
        //                await Task.Delay(_timeResponse);
        //                _tcs.TrySetResult(false);
        //            }
        //            catch (Exception)
        //            {
        //                _tcs.TrySetResult(false);
        //            }
        //        }
        //    });

        //    return _tcs.Task;
        //}


    }
}