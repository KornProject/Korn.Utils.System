using System.Runtime.InteropServices;

static class Interop
{
    const string kernel = "kernel32";

    [DllImport(kernel)] public static extern 
        nint OpenProcess(int desiredAccess, bool inheritHandle, int processID);

    [DllImport(kernel)] public static extern 
        nint OpenThread(int desiredAccess, bool inheritHandle, int threadID);

    [DllImport(kernel)] public static extern 
        bool SuspendThread(nint thread);

    [DllImport(kernel)] public static extern 
        int ResumeThread(nint thread);

    [DllImport(kernel)] public static extern
        bool CloseHandle(nint handle);
}