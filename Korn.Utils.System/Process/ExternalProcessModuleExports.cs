using Korn.Utils.PEImageReader;
using System;

namespace Korn.Utils
{
    public unsafe class ExternalProcessModuleExports : IDisposable
    {
        public ExternalProcessModuleExports(ExternalProcess process, IntPtr moduleHandle)
        {
            this.process = process;
            this.moduleHandle = moduleHandle;
            cache = new Cache(this);
        }

        ExternalProcess process;
        IntPtr moduleHandle;        
        Cache cache;

        public uint GetFunctionRva(string name) => cache.GetFunctionRva(name);

        #region IDisposable
        bool disposed;
        public void Dispose()
        {
            if (disposed)
                return;
            disposed = true;
        }

        ~ExternalProcessModuleExports() => Dispose();
        #endregion

        public class Cache
        {
            public Cache(ExternalProcessModuleExports objective)
            {
                var memory = objective.process.Memory;
                var moduleHandle = objective.moduleHandle;
                pe = new PERuntimeImage(memory, (void*)moduleHandle);

                var functionCount = pe.ExportFunctionsCount;
                functions = new Function[functionCount];
                for (var index = 0; index < functionCount; index++)
                {
                    var nameRva = pe.GetExportFunctionNameRva(index);
                    var name = pe.GetNameOfExportFunction(nameRva);
                    var rva = pe.GetExportFunctionRva(index);
                    functions[index] = new Function(name, rva);
                }
            }

            PERuntimeImage pe;
            Function[] functions;

            public uint GetFunctionRva(string name)
            {
                var hash = HashedString.CalculateHash(name);
                foreach (var function in functions)
                    if (function.Name.Hash == hash)
                        return function.RVA;
                return 0;
            }

            struct Function
            {
                public Function(string name, uint rva) : this(new HashedString(name), rva) {}
                public Function(HashedString name, uint rva) => (Name, RVA) = (name, rva);

                public HashedString Name;
                public uint RVA;
            }
        }
    }
}