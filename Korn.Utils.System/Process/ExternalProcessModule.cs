using System;

namespace Korn.Utils
{
    public struct ExternalProcessModule
    {
        public ExternalProcessModule(IntPtr handle, string name, string path) => (Handle, Name, Path) = (handle, name, path);

        public IntPtr Handle;
        public string Name;
        public string Path;

        public bool IsValid => Handle != IntPtr.Zero;
    }
}