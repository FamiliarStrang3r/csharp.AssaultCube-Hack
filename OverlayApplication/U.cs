using System;
using System.Linq;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

namespace OverlayApplication
{
    public static class U
    {
        public static Rectangle GetClientRectangle(IntPtr handle)
        {
            return User32.ClientToScreen(handle, out var point) && User32.GetClientRect(handle, out var rect) ?
                new Rectangle(point.X, point.Y, rect.Right - rect.Left, rect.Bottom - rect.Top) : default;
        }

        public static Module GetModule(this Process process, string moduleName)
        {
            ProcessModule processModule = process.GetProcessModule(moduleName);

            return processModule is null || processModule.BaseAddress == IntPtr.Zero ? 
                default : new Module(process, processModule);
        }

        public static ProcessModule GetProcessModule(this Process process, string moduleName)
        {
            return process?.Modules.OfType<ProcessModule>().FirstOrDefault(a => string.Equals(a.ModuleName.ToLower(), moduleName.ToLower()));
        }

        public static bool IsRunning(this Process process)
        {
            try
            {
                Process.GetProcessById(process.Id);
            }
            catch (InvalidOperationException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }

            return true;
        }

        public static T Read<T>(this Process process, IntPtr lpBaseAddress) where T : unmanaged
        {
            return Read<T>(process.Handle, lpBaseAddress);
        }

        //public static T Read<T>(this Module module, int offset)
        //{
        //    return default;
        //}

        public static T Read<T>(IntPtr hProcess, IntPtr lpBaseAddress) where T : unmanaged
        {
            var size = Marshal.SizeOf<T>();
            var buffer = (object)default(T);
            Kernel32.ReadProcessMemory(hProcess, lpBaseAddress, buffer, size, out var lpNumberOfBytesRead);
            return lpNumberOfBytesRead == size ? (T)buffer : default;
        }

        public static bool Write<T>(this Process process, IntPtr lpBaseAddress, T value) where T : unmanaged
        {
            //return Write<T>(process.Handle, lpBaseAddress);

            var buffer = new T[Marshal.SizeOf<T>()];
            buffer[0] = value;
            return Kernel32.WriteProcessMemory(process.Handle, lpBaseAddress, buffer, Marshal.SizeOf<T>(), out var bytesread);
        }

        //public static bool Write<T>(IntPtr hProcess, IntPtr lpBaseAddress, T value)
        //{
        //    //var size = Marshal.SizeOf<T>();
        //    //var buffer = (object)default(T);
        //    Kernel32.WriteProcessMemory(hProcess, lpBaseAddress, buffer, size, out var lpNumberOfBytesWritten);
        //    return lpNumberOfBytesWritten == size;
        //}
    }
}
