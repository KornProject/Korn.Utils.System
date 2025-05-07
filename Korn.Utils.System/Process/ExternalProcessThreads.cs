using System.Collections.Generic;

namespace Korn.Utils
{
    public unsafe class ExternalProcessThreads : Toolhelp32Snapshot
    {
        const int THREAD_STATE_ACCESS = 0x0002;

        public ExternalProcessThreads(ExternalProcessId process) : base(process, SnapshotFlags.Thread) { }

        bool Thread32First(ThreadEntry32* thread) => Kernel32.Thread32First(Handle, thread);
        bool Thread32Next(ThreadEntry32* thread) => Kernel32.Thread32Next(Handle, thread);

        public List<ExternalProcessThreadId> GetThreads()
        {
            var threads = new List<ExternalProcessThreadId>();

            var entry = new ThreadEntry32 { Size = sizeof(ThreadEntry32) };
            var entryPointer = &entry;

            if (Thread32First(entryPointer))
                do AddThread();
                while (Thread32Next(entryPointer));

            return threads;

            void AddThread() => threads.Add(new ExternalProcessThreadId(entryPointer->ThreadID));
        }

        List<ExternalProcessThreadId> lastSuspendedThreads;
        public void SuspendThreads()
        {
            lastSuspendedThreads = GetThreads();
            foreach (var thread in lastSuspendedThreads)
                thread.Suspend();
        }

        public void ResumeThreads()
        {
            if (lastSuspendedThreads != null)
                foreach (var thread in lastSuspendedThreads)
                    thread.Resume();
        }
    }
}