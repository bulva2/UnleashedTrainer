using Swed64;
using System.Runtime.InteropServices;

namespace CrashTimeUnleashed
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Memory memory = new Memory();
            memory.Initialize();

            Renderer renderer = new Renderer();
            renderer.Initialize(memory);
            
            Thread renderThread = new Thread(() => renderer.Start().Wait());
            renderThread.Start();
        }
    }
}
