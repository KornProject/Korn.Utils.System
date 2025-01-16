using System;
using System.Runtime.InteropServices;

unsafe static class Interop
{
    const string kernel = "kernel32";
    const string ntdll = "ntdll";

    [DllImport(kernel)] public static extern 
        IntPtr OpenProcess(IntPtr desiredAccess, bool inheritHandle, int processID);

    [DllImport(kernel)] public static extern
        IntPtr OpenThread(IntPtr desiredAccess, bool inheritHandle, int threadID);

    [DllImport(kernel)] public static extern 
        bool SuspendThread(IntPtr thread);

    [DllImport(kernel)] public static extern 
        int ResumeThread(IntPtr thread);

    [DllImport(kernel)] public static extern
        bool CloseHandle(IntPtr handle);

    [DllImport(ntdll)] static extern
        uint NtQueryInformationProcess(IntPtr processHandle, int processInformationClass, void* processInformation, int processInformationLength, IntPtr* returnLength);

    [StructLayout(LayoutKind.Sequential)]
    struct PROCESS_BASIC_INFORMATION
    {
        public int ExitStatus;
        public IntPtr PebBaseAddress;
        public ulong AffinityMask;
        public IntPtr BasePriority;
        public IntPtr UniqueProcessID;
        public IntPtr InheritedFromUniqueProcessID;
    }

    public static int GetParentProcessId(int processID)
    {
        const int ProcessBasicInformation = 0;
        const int PROCESS_QUERY_INFORMATION = 0x0400;

        var processHandle = OpenProcess((IntPtr)PROCESS_QUERY_INFORMATION, false, processID);

        PROCESS_BASIC_INFORMATION pbi;
        IntPtr writtenLength;
        NtQueryInformationProcess(processHandle, ProcessBasicInformation, &pbi, sizeof(PROCESS_BASIC_INFORMATION), &writtenLength);

        CloseHandle(processHandle);
        return (int)pbi.InheritedFromUniqueProcessID;
    }
}