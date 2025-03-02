using System;
using System.Diagnostics;

namespace Korn.Utils
{
    public class CommandLine
    {
        public CommandLine(string host = "cmd", Action<string> outputHandler = null, Action exitHandler = null, string arguments = null, bool isHidden = true)
        {
            Process = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    WindowStyle = isHidden ? ProcessWindowStyle.Hidden : ProcessWindowStyle.Normal,
                    Arguments = arguments ?? "",
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    FileName = host,
                }
            };

            if (outputHandler != null)
            {
                Process.OutputDataReceived += (s, e) =>
                {
                    if (e.Data != null)
                        outputHandler(e.Data);
                };
            }

            if (exitHandler != null)
                Process.Exited += (s, e) => exitHandler();

            Process.Start();
            Process.BeginOutputReadLine();
        }

        public readonly Process Process;

        public void WriteLine(string line) => Process.StandardInput.WriteLine(line);
    }
}