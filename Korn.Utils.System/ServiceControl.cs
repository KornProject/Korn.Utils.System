using System.Diagnostics;

namespace Korn.Utils
{
    public class ServiceControl
    {
        public ServiceControl(string name) => this.name = name;

        string name;
        string wrappedName => $"\"{name}\"";

        public void Stop() => Execute($"stop {wrappedName}");
        public void Start() => Execute($"start {wrappedName}");        
        public void Delete() => Execute($"delete {wrappedName}");
        public void Config(string mode) => Execute($"config {wrappedName} start= {mode}");
        public void Create(string binaryPath) => Execute($"create {wrappedName} binpath= \"{binaryPath}\"");

        public static void Execute(string arguments)
        {
            var startInfo = new ProcessStartInfo()
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = "sc.exe",
                Arguments = arguments
            };

            global::System.Diagnostics.Process.Start(startInfo);
        }
    }
}