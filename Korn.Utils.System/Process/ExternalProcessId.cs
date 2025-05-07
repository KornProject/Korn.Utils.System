using System;

namespace Korn.Utils
{
    public class ExternalProcessId : IDisposable
    {
        public ExternalProcessId(int pid) => ID = pid;

        public readonly int ID;

        bool isProcessInitialized;
        ExternalProcess process;
        public ExternalProcess Process
        {
            get
            {
                if (!isProcessInitialized)
                {
                    isProcessInitialized = true;
                    process = ExternalProcess.Open(this);
                }

                return process;
            }
        }

        bool isParentIdKnown;
        int parentId;
        public int ParentId
        {
            get
            {
                if (!isParentIdKnown)
                {
                    isParentIdKnown = true;
                    parentId = Process.GetParentProcessId();
                }

                return parentId;
            }
        }

        bool isModulesInitialized;
        ExternalProcessModules modules;
        public ExternalProcessModules Modules
        {
            get
            {
                if (!isModulesInitialized)
                {
                    isModulesInitialized = true;
                    modules = new ExternalProcessModules(this);
                }

                return modules;
            }
        }

        bool isThreadsInitialized;
        ExternalProcessThreads threads;
        public ExternalProcessThreads Threads
        {
            get
            {
                if (!isThreadsInitialized)
                {
                    isThreadsInitialized = true;
                    threads = new ExternalProcessThreads(this);
                }

                return threads;
            }
        }

        public void SuspendProcess() => Threads.SuspendThreads();

        public void ResumeProcess() => Threads?.ResumeThreads();

        #region IDisposable
        bool disposed;
        public void Dispose()
        {
            if (disposed)
                return;
            disposed = true;

            if (isProcessInitialized)
                ExternalProcess.Close(process);

            if (isModulesInitialized)
                modules.Dispose();

            if (isThreadsInitialized)
                threads.Dispose();
        }

        ~ExternalProcessId() => Dispose();
        #endregion IDisposable
    }
}