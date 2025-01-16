using System;
using System.Diagnostics;

namespace Korn.Utils
{
    public partial class ExternalProcessManager : IDisposable
    {
        const int PROCESS_THREAD_ACCESS = 0x0800;

        public ExternalProcessManager(Process process)
        {
            Process = process;
            ProcessHandle = Interop.OpenProcess((IntPtr)PROCESS_THREAD_ACCESS, false, process.Id);
        }

        public readonly Process Process;
        public IntPtr ProcessHandle { get; private set; }

        bool disposed;
        public void Dispose()
        {
            if (disposed)
                return;

            disposed = true;
            Interop.CloseHandle(ProcessHandle);
        }

        ~ExternalProcessManager() => Dispose();
    }
}