using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Server.Infrastructure;

namespace Server.Actions
{

    public class ActionFromCashier
    {
        #region prop

        public CashierHandling CashierHandling { get; }
        public Func<Task> CashierAct { get; }

        #endregion




        #region ctor

        public ActionFromCashier(CashierHandling cashierHandling, Func<Task> cashierAct)
        {
            CashierHandling = cashierHandling;
            CashierAct = cashierAct;
        }

        #endregion

    }
}