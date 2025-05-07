using System;

namespace Korn.Utils
{
    public struct ExternalProcessThread
    {
        const int THREAD_STATE_ACCESS = 0x0002;

        ExternalProcessThread(IntPtr handle) => Handle = handle;

        public readonly IntPtr Handle;

        public void Suspend() => Kernel32.SuspendThread(Handle);

        public void Resume() => Kernel32.ResumeThread(Handle);

        public static ExternalProcessThread Open(ExternalProcessThreadId threadId) => new ExternalProcessThread(Kernel32.OpenThread(THREAD_STATE_ACCESS, false, threadId.ID));
        public static void Close(ExternalProcessThread thread) => Kernel32.CloseHandle(thread.Handle);
    }
}