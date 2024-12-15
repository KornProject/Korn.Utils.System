using System.Diagnostics;

namespace Korn.Utils;
public partial class ExternalProcessManager : IDisposable
{
    public ExternalProcessManager(Process process)
    {
        Process = process;
        ProcessHandle = Interop.OpenProcess(PROCESS_THREAD_ACCESS, false, process.Id);
    }

    const int PROCESS_THREAD_ACCESS = 0x0800;

    public readonly Process Process;
    public nint ProcessHandle { get; private set; }    

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