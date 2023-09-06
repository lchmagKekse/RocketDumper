namespace RocketDumper.Engine
{
    public struct UObject
    {
        public IntPtr VfTableObject;
        public IntPtr HashNext;
        public ulong ObjectFlags;
        public IntPtr HashOuterNext;
        public IntPtr StateFrame;
        public IntPtr Linker;
        public int LinkerIndex;
        public int ObjectInternalInteger;
        public int NetIndex;
        public IntPtr Outer;
        public int FNameEntryId;
        public int InstanceNumber;
        public IntPtr Class;
        public IntPtr ObjectArchetype;
    }
}
