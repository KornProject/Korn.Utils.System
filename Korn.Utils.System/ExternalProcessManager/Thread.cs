using System.Diagnostics;

namespace Korn.Utils.System;
public partial class ExternalProcessManager
{
    const int THREAD_STATE_ACCESS = 0x0002;

    public void SuspendThread(ProcessThread thread)
    {
        var threadHandle = Interop.OpenThread(THREAD_STATE_ACCESS, false, thread.Id);

        Interop.SuspendThread(threadHandle);
        Interop.CloseHandle(threadHandle);
    }

    public void ResumeThread(ProcessThread thread)
    {
        var threadHandle = Interop.OpenThread(THREAD_STATE_ACCESS, false, thread.Id);

        Interop.ResumeThread(threadHandle);
        Interop.CloseHandle(threadHandle);
    }

    public void SuspendProcess()
    {
        foreach (ProcessThread thread in Process.Threads)
            SuspendThread(thread);
    }

    public void ResumeProcess()
    {
        foreach (ProcessThread thread in Process.Threads)
            ResumeThread(thread);
    }
}