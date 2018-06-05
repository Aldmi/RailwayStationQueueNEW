using System.Collections.Concurrent;

namespace Server.Actions
{
    public class ActionQueue
    {
        private ConcurrentQueue<ActionFromCashier> Queue { get; } = new ConcurrentQueue<ActionFromCashier>();



        #region Methode

        public void Start()
        {
            
        }

        public void Stop()
        {

        }


        public void Enqueue(ActionFromCashier act)
        {
            Queue.Enqueue(act);
        }





        #endregion

    }
}