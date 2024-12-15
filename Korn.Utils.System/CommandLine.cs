using System.Diagnostics;

namespace Korn.Utils;
public class CommandLine
{
    public CommandLine(string host = "cmd", Action<string>? outputHandler = null, Action? exitHandler = null, string? arguments = null, bool isHidden = true)
    {
        Process = new()
        {
            StartInfo = new()
            {
                WindowStyle = isHidden ? ProcessWindowStyle.Hidden : ProcessWindowStyle.Normal,
                Arguments = arguments??"",
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                FileName = host,
            }
        };

        if (outputHandler is not null)
        {
            Process.OutputDataReceived += (s, e) => 
            {
                if (!string.IsNullOrEmpty(e.Data))
                    outputHandler(e.Data!);
            };
        }

        if (exitHandler is not null)
            Process.Exited += (s, e) => exitHandler();

        Process.Start();
        Process.BeginOutputReadLine();
    }

    public readonly Process Process;
}