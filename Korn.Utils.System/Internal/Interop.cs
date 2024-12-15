using System.Runtime.InteropServices;

unsafe static class Interop
{
    const string kernel = "kernel32";
    const string ntdll = "ntdll";

    [DllImport(kernel)] public static extern 
        nint OpenProcess(nint desiredAccess, bool inheritHandle, int processID);

    [DllImport(kernel)] public static extern 
        nint OpenThread(nint desiredAccess, bool inheritHandle, int threadID);

    [DllImport(kernel)] public static extern 
        bool SuspendThread(nint thread);

    [DllImport(kernel)] public static extern 
        int ResumeThread(nint thread);

    [DllImport(kernel)] public static extern
        bool CloseHandle(nint handle);

    [DllImport(ntdll)] static extern
        uint NtQueryInformationProcess(nint processHandle, int processInformationClass, void* processInformation, int processInformationLength, nint* returnLength);

    [StructLayout(LayoutKind.Sequential)]
    record struct PROCESS_BASIC_INFORMATION(int ExitStatus, nint PebBaseAddress, ulong AffinityMask, nint BasePriority, nint UniqueProcessId, nint InheritedFromUniqueProcessId);

    public static int GetParentProcessId(int processID)
    {
        const int ProcessBasicInformation = 0;
        const nint PROCESS_QUERY_INFORMATION = 0x0400;

        var processHandle = OpenProcess(PROCESS_QUERY_INFORMATION, false, processID);

        PROCESS_BASIC_INFORMATION pbi;
        nint writtenLength;
        NtQueryInformationProcess(processHandle, ProcessBasicInformation, &pbi, sizeof(PROCESS_BASIC_INFORMATION), &writtenLength);

        CloseHandle(processHandle);
        return (int)pbi.InheritedFromUniqueProcessId;
    }
}