using System.Threading;

namespace LockFree.Common
{
    public class Atomic
    { 
        public static bool CAS<T>(ref T location, T comparand, T newValue) where T : class
        {
            return comparand == Interlocked.CompareExchange(ref location, newValue, comparand);
        }
    }
}
