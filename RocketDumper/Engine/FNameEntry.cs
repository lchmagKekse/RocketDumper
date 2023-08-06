using System.Runtime.InteropServices;

namespace RocketDumper.Engine
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct FNameEntry
    {
        public ulong Flags;
        public int Index;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x0C)]
        public byte[] UnknownData;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x400)]
        public string Name;
    }
}
