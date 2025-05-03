using CrashTimeUnleashed.Entities;
using Swed64;

namespace CrashTimeUnleashed
{
    public class Memory
    {
        private Swed? _swed;
        private IntPtr moduleBase;

        public void Initialize()
        {
            _swed = FindGameProcess();
            if (_swed == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[!] Failed to find game process.");
                Console.WriteLine("[!] Make sure the game (BurningWheelsHi.exe) is running and try again. Exiting..");
                Console.ResetColor();
                Environment.Exit(404);
                return;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[+] Game process found. The trainer is ready to go!");
                Console.WriteLine("[+] Press 'INSERT' to toggle the menu.");
                Console.ResetColor();
            }
        }

        public Player? InitializePlayer()
        {
            if (_swed == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[!] Game process not found. Cannot initialize player.");
                Console.WriteLine("[!] Make sure the game is running and try again. Exiting..");
                Console.ResetColor();
                return null;
            }

            Player player = new Player(_swed, moduleBase);
            player.LoadAddresses();

            return player;
        }

        public Swed? FindGameProcess()
        {
            try
            {
                _swed = new Swed("BurningWheelsHi");
                moduleBase = _swed.GetModuleBase("BurningWheelsHi.exe");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error finding game process: {e.Message}");
                return null;
            }

            return _swed;
        }

        public int GetPropertyDamage()
        {
            if (_swed == null)
                return 0;

            nint ptr1 = _swed.ReadInt(moduleBase + 0x5BCE00);
            nint ptr2 = _swed.ReadInt(ptr1 + 0x44);
            int value = _swed.ReadInt(ptr2 + 0xC0);

            return value;
        }

        public void WritePropertyDamage(int value)
        {
            if (_swed == null)
                return;
            nint ptr1 = _swed.ReadInt(moduleBase + 0x5BCE00);
            nint ptr2 = _swed.ReadInt(ptr1 + 0x44);
            _swed.WriteInt(ptr2 + 0xC0, value);
        }
    }
}
