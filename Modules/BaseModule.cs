using CrashTimeUnleashed.Entities;

namespace CrashTimeUnleashed.Modules
{
    public abstract class BaseModule
    {
        protected bool _enabled;
        protected Player _player;

        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    if (_enabled)
                        OnEnable();
                    else
                        OnDisable();
                }
            }
        }

        protected BaseModule(Player player)
        {
            _player = player;
            _enabled = false;
        }

        public abstract void Update();

        protected virtual void OnEnable() 
        {
            Console.WriteLine($"[+] Module {GetType().Name} has been enabled.");
        }
        protected virtual void OnDisable() 
        {
            Console.WriteLine($"[+] Module {GetType().Name} has been disabled.");
        }
    }
} 