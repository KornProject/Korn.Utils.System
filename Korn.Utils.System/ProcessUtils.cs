using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Korn.Utils 
{
    public static class ProcessUtils
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetParentProcessID(this Process process) => GetParentProcessID(process.Id);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetParentProcessID(int processID) => Interop.GetParentProcessId(processID);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Process GetParentProcess(this Process process) => GetParentProcess(process.Id);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Process GetParentProcess(int processID) => Process.GetProcessById(Interop.GetParentProcessId(processID));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Process GetProcessByID(int processID)
        {
            try
            {
                return Process.GetProcessById(processID);
            }
            catch
            {
                return null;
            }
        }
    }
}