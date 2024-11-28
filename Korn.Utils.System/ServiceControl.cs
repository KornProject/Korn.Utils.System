using System.Diagnostics;

namespace Korn.Utils.System;
public static class ServiceControl
{
    public static void Execute(string arguments)
    {
        var startInfo = new ProcessStartInfo()
        {
            WindowStyle = ProcessWindowStyle.Hidden,
            FileName = "sc.exe",
            Arguments = arguments
        };

        Process.Start(startInfo);
    }
}
