using CrashTimeUnleashed.Entities;
using Swed64;
using System.Diagnostics;

namespace CrashTimeUnleashed
{
    public class Memory
    {
        private Swed? _swed;
        private IntPtr moduleBase;

        public void Initialize()
        {
            Console.Title = "Crash Time Unleashed Trainer - Debug Window";
            Console.WriteLine("Thanks for using Crash Time Unleashed Trainer!\n");

            _swed = FindGameProcess();
            if (_swed == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[!] Failed to find game process.");
                Console.WriteLine("[!] Make sure the game is running and try again. Exiting..");
                Console.ResetColor();

                Console.ReadKey();
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
                Console.WriteLine("[!] There has been an issue while initializing player!");
                Console.WriteLine("[!] Make sure the game is running and try again. Exiting..");

                Console.ReadKey();
                Console.ResetColor();
                return null;
            }

            Player player = new Player(_swed, moduleBase);
            player.LoadAddresses();

            return player;
        }

        public Swed? FindGameProcess()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("[?] Looking for game process.. (BurningWheelsHi or CrashTime2Hi)");

            string[] processNames = { "BurningWheelsHi", "CrashTime2Hi", "BurningWheelsLow", "CrashTime2Low" };

            foreach (var name in processNames)
            {
                try
                {
                    _swed = new Swed(name);

                    if ((name == "CrashTime2Low") || (name == "BurningWheelsLow"))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"[!] Low Graphics version is currently sadly not supported.");
                        Console.WriteLine($"[!] Please run the game in High Graphics mode. That should be in the game installation directory and ends with Hi.exe");
                        Console.ReadKey();
                        Environment.Exit(501);
                    }

                    moduleBase = _swed.GetModuleBase(name + ".exe");
                    return _swed;
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Game process not found");
                    Console.ResetColor();
                    Console.WriteLine("Trying alternative process name..\n");
                }
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error: Could not find a valid game process!");
            return null;
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
