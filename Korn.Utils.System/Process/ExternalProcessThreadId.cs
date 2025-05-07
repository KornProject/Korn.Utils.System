using System;

namespace Korn.Utils
{
    public class ExternalProcessThreadId : IDisposable
    {
        public ExternalProcessThreadId(int id) => ID = id;

        public readonly int ID;

        bool isThreadInitialized;
        ExternalProcessThread thread;
        public ExternalProcessThread Thread
        {
            get
            {
                if (!isThreadInitialized)
                {
                    isThreadInitialized = true;
                    thread = ExternalProcessThread.Open(this);
                }

                return thread;
            }
        }

        public void Suspend() => Thread.Suspend();

        public void Resume() => Thread.Resume();

        bool disposed;
        public void Dispose()
        {
            if (disposed)
                return;
            disposed = true;

            if (isThreadInitialized)
                ExternalProcessThread.Close(Thread);
        }
    }
}