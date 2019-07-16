using System.Threading;

namespace ZDPress.Opc
{
    public static class OpcResponderSingleton
    {
        private static readonly object SLock = new object();
        private static OpcResponder _instance = null;

        public static OpcResponder Instance
        {
            get
            {
                if (_instance != null) return _instance;
                Monitor.Enter(SLock);
                OpcResponder temp = new OpcResponder();
                Interlocked.Exchange(ref _instance, temp);
                Monitor.Exit(SLock);
                return _instance;
            }
        }
    }
}
