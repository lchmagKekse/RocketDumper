using System.Diagnostics;
using System.Runtime.InteropServices;

namespace RocketDumper
{
    public class MemoryHelper
    {
        private string _processName = "RocketLeague";
        private IntPtr _processHandle;
        private Process _targetProcess;
        public IntPtr BaseAddress;

        [DllImport("kernel32.dll")]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        private static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        public MemoryHelper()
        {
            _targetProcess = GetProcess();
            _processHandle = OpenProcess(0x0010, false, _targetProcess.Id);

            if (_processHandle == IntPtr.Zero || _targetProcess.MainModule == null)
            {
                Console.WriteLine("Failed to open process.");
                Environment.Exit(0);
            }

            BaseAddress = _targetProcess.MainModule.BaseAddress;
        }

        ~MemoryHelper()
        {
            CloseHandle(_processHandle);
        }

        public T ReadMemory<T>(IntPtr address) where T : struct
        {
            int bufferSize = Marshal.SizeOf<T>();
            byte[] buffer = new byte[bufferSize];

            if (!ReadProcessMemory(_processHandle, address, buffer, bufferSize, out int bytesRead))
            {
                Console.WriteLine("Failed to read process memory.");
                Environment.Exit(0);
            }

            if (bytesRead != bufferSize)
            {
                Console.WriteLine("Failed to read the expected number of bytes.");
                Environment.Exit(0);
            }

            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            try
            {
                return Marshal.PtrToStructure<T>(handle.AddrOfPinnedObject());
            }
            finally
            {
                handle.Free();
            }
        }

        public Process GetProcess()
        {
            Process[] processes = Process.GetProcessesByName(_processName);

            if (processes.Length == 0)
            {
                Console.WriteLine("Process not found.");
                Environment.Exit(0);
            }

            Process targetProcess = processes[0];
            return targetProcess;
        }

        public IntPtr FindPattern(string pattern)
        {
            string[] patternBytes = pattern.Split(' ');
            byte[] bytePattern = new byte[patternBytes.Length];

            for (int i = 0; i < patternBytes.Length; i++)
            {
                if (patternBytes[i] == "??")
                {
                    bytePattern[i] = 0x00;
                    continue;
                }

                bytePattern[i] = Convert.ToByte(patternBytes[i], 16);
            }

            byte[] buffer = new byte[4096];

            while (ReadProcessMemory(_processHandle, BaseAddress, buffer, buffer.Length, out int bytesRead))
            {
                for (int i = 0; i < bytesRead - bytePattern.Length; i++)
                {
                    bool found = true;
                    for (int j = 0; j < bytePattern.Length; j++)
                    {
                        if (bytePattern[j] != 0x00 && bytePattern[j] != buffer[i + j])
                        {
                            found = false;
                            break;
                        }
                    }

                    if (found)
                    {
                        IntPtr address = BaseAddress + i;
                        return address;
                    }
                }

                BaseAddress += bytesRead;
            }

            Console.WriteLine("Pattern not found in the process.");
            Environment.Exit(0);
            return IntPtr.Zero;
        }
    }
}