using System.Runtime.InteropServices;
using Korn.Utils.Unsafe;
using System;

#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
namespace Korn.Utils
{
    public unsafe class ExternalProcessModules : Toolhelp32Snapshot
    {
        public ExternalProcessModules(ExternalProcessId processId) : base(processId, SnapshotFlags.Module | SnapshotFlags.Module32) => process = processId.Process;

        ExternalProcess process;

        bool Module32First(ref ManagedModuleEntry32 module) => Kernel32.Module32First(Handle, ref module);
        bool Module32First(NativeModuleEntry32* module) => Kernel32.Module32First(Handle, module);
        bool Module32Next(ref ManagedModuleEntry32 module) => Kernel32.Module32Next(Handle, ref module);
        bool Module32Next(NativeModuleEntry32* module) => Kernel32.Module32Next(Handle, module);

        bool GetModuleEntry(string nameWithExtension, ref ManagedModuleEntry32 module)
        {
            module.Size = Marshal.SizeOf<ManagedModuleEntry32>();
            if (Module32First(ref module))
                do
                    if (module.ModuleName == nameWithExtension)
                        return true;
                while (Module32Next(ref module));

            return false;
        }
        bool FastGetModuleEntry(ulong nameFootprint, NativeModuleEntry32* module)
        {
            module->Size = sizeof(NativeModuleEntry32);
            if (Module32First(module))
                do
                    if (*(ulong*)module->ModuleName == nameFootprint)
                        return true;
                while (Module32Next(module));

            return false;
        }

        public ExternalProcessModule GetModule(string nameWithExtension)
        {
            var moduleEntry = new ManagedModuleEntry32();
            GetModuleEntry(nameWithExtension, ref moduleEntry);

            var handle = moduleEntry.ModuleHandle;
            var name = moduleEntry.ModuleName;
            var path = moduleEntry.ExePath;
            var module = new ExternalProcessModule(handle, name, path);
            return module;
        }
        public ExternalProcessModule FastGetModule(ulong nameFootprint)
        {
            var moduleEntry = new NativeModuleEntry32();
            FastGetModuleEntry(nameFootprint, &moduleEntry);

            var handle = moduleEntry.ModuleHandle;
            var name = Marshal.PtrToStringAnsi((IntPtr)moduleEntry.ModuleName);
            var path = Marshal.PtrToStringAnsi((IntPtr)moduleEntry.ExePath);
            var module = new ExternalProcessModule(handle, name, path);
            return module;
        }

        public IntPtr GetModuleHandle(string nameWithExtension)
        {
            var module = new ManagedModuleEntry32();
            if (GetModuleEntry(nameWithExtension, ref module))
                return module.ModuleHandle;
            return IntPtr.Zero;
        }
        public IntPtr FastGetModuleHandle(ulong nameFootprint)
        {
            var module = new NativeModuleEntry32();
            if (FastGetModuleEntry(nameFootprint, &module))
                return module.ModuleHandle;
            return IntPtr.Zero;
        }

        public bool ContainsModule(string nameWithExtension) => GetModuleHandle(nameWithExtension) != IntPtr.Zero;
        public bool FastContainsModule(ulong nameFootprint) => FastGetModuleHandle(nameFootprint) != IntPtr.Zero;

        public ExternalProcessModuleExports GetModuleExports(string moduleNameWithExtension)
        {
            var module = new ManagedModuleEntry32();
            if (!GetModuleEntry(moduleNameWithExtension, ref module))
                return null;

            var handle = module.ModuleHandle;
            var exports = new ExternalProcessModuleExports(process, handle);
            return exports;
        }

        public static ulong GetNameFootprint(string name)
        {
            var value = 0UL;
            var length = name.Length;
            if (length > 8)
                length = 8;

            var chars = name.ToCharArray();
            for (var i = 0; i < length; i++)
                value |= (ulong)chars[i * 2] << (i * 8);

            return value;
        }
    }
}