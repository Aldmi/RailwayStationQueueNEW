using NLog;

namespace Library.Logs
{
    public static class NlogFactory
    {
        private static Logger Logger { get; }




        static NlogFactory()
        {
            Logger= LogManager.GetCurrentClassLogger();
        }




        public static void Info(string message)
        {
            Logger.Info(message);
        }

        public static void Debug(string message)
        {
            Logger.Debug(message);
        }

        public static void Error(string message)
        {
            Logger.Error(message);
        }

        public static void Fatal(string message)
        {
            Logger.Fatal(message);
        }
    }
}