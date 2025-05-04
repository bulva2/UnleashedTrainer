using Swed64;

namespace CrashTimeUnleashed.Entities
{
    public class Player
    {
        public IntPtr xAddr;
        public IntPtr yAddr;
        public IntPtr zAddr;

        public IntPtr xRotAddr;
        public IntPtr yRotAddr;
        public IntPtr zRotAddr;

        public IntPtr carIndexAddr;

        private IntPtr _moduleBase;
        private Swed _swed;

        public Player(Swed swed, nint moduleBase)
        {
            _swed = swed;
            _moduleBase = moduleBase;
        }

        public IntPtr GetModuleBase()
        {
            return _moduleBase;
        }

        public IntPtr GetProcessHandle()
        {
            return _swed.GetProcess().Handle;
        }

        public void LoadAddresses()
        {
            xAddr = _moduleBase + 0x5BFBB8;
            yAddr = _moduleBase + 0x5BFBBC;
            zAddr = _moduleBase + 0x5BFBC0;

            xRotAddr = _moduleBase + 0x5BFBC4;
            yRotAddr = _moduleBase + 0x5BFBC8;
            zRotAddr = _moduleBase + 0x5BFBCC;

            carIndexAddr = _moduleBase + 0x34BF8C;
        }

        public float ReadFloat(IntPtr address)
        {
            return _swed.ReadFloat(address);
        }

        public void WriteFloat(IntPtr address, float value)
        {
            _swed.WriteFloat(address, value);
        }

        public byte[] ReadBytes(IntPtr address, int length)
        {
            return _swed.ReadBytes(address, length);
        }

        public void WriteBytes(IntPtr address, byte[] bytes)
        {
            _swed.WriteBytes(address, bytes);
        }

        public int ReadInt(IntPtr address)
        {
            return _swed.ReadInt(address);
        }
    }
}
