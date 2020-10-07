using System;
using System.Diagnostics;

namespace OverlayApplication
{
    public class Module : IDisposable
    {
        private Process Process { get; set; }

        private ProcessModule ProcessModule { get; set; }

        public Module(Process process, ProcessModule processModule)
        {
            Process = process;
            ProcessModule = processModule;
        }

        public void Dispose()
        {
            Process.Dispose();
            Process = default;

            ProcessModule.Dispose();
            ProcessModule = default;
        }
    }
}
