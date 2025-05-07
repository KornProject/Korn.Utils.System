using System;
using System.Threading;

namespace Korn.Utils.System
{
    public class CrossSystemLocker : IDisposable
    {
        CrossSystemLocker(Mutex mutex) => this.mutex = mutex;

        Mutex mutex;

        public void Unlock() => mutex?.Dispose();

        public static bool TryCreateLock(string name, out CrossSystemLocker locker)
        {
            name = GlobalizeName(name);

            var mutex = new Mutex(false, name, out var isNew);
            if (!isNew)
            {
                locker = null;
                mutex.Dispose();
                return false;
            }

            locker = new CrossSystemLocker(mutex);
            return true;
        }

        public static CrossSystemLocker WaitAndLock(string name)
        {
            name = GlobalizeName(name);

            var mutex = new Mutex(false, name);
            mutex.WaitOne();

            var locker = new CrossSystemLocker(mutex);
            return locker;
        }

        static string GlobalizeName(string name) => $"KornMutex-{name}";

        #region IDisposable
        bool disposed;
        public void Dispose()
        {
            if (disposed)
                return;
            disposed = true;

            mutex.ReleaseMutex();
            mutex.Dispose();
        }
        #endregion
    }
}