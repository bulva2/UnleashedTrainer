using System;
using CrashTimeUnleashed.Entities;
using ImGuiNET;

namespace CrashTimeUnleashed.Modules
{
    public class InfiniteNitro : BaseModule
    {
        private IntPtr _nitroAddress;
        private float _nitroValue = 1.0f;
        private DateTime _lastUpdateTime = DateTime.MinValue;

        public InfiniteNitro(Player player) : base(player)
        {
            InitializeAddresses(player);
        }

        private void InitializeAddresses(Player player)
        {
            _nitroAddress = player.GetModuleBase() + 0x5C1538;
        }

        public float NitroValue
        {
            get => _nitroValue;
            set
            {
                if (_nitroValue != value)
                {
                    _nitroValue = value;
                    if (_enabled)
                    {
                        try
                        {
                            _player.WriteFloat(_nitroAddress, _nitroValue);
                            Console.WriteLine($"[InfiniteNitro] Set value to {_nitroValue}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[InfiniteNitro] Error setting value: {ex.Message}");
                        }
                    }
                }
            }
        }
        
        public float GetCurrentNitro()
        {
            try
            {
                return _player.ReadFloat(_nitroAddress);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[InfiniteNitro] Error reading current value: {ex.Message}");
                return 0.0f;
            }
        }

        protected override void OnEnable()
        {
            try
            {
                _player.WriteFloat(_nitroAddress, _nitroValue);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[InfiniteNitro] Error setting initial value: {ex.Message}");
            }
            
            base.OnEnable();
        }

        public override void Update()
        {
            if (!_enabled) return;

            // I'm pretty sure updating nitro every 100ms is more than enough
            TimeSpan elapsed = DateTime.Now - _lastUpdateTime;
            if (elapsed.TotalMilliseconds > 100)
            {
                try
                {
                    float currentNitro = _player.ReadFloat(_nitroAddress);
                    if (Math.Abs(currentNitro - _nitroValue) > 0.01f)
                    {
                        _player.WriteFloat(_nitroAddress, _nitroValue);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[InfiniteNitro] Error updating nitro: {ex.Message}");
                }
                
                _lastUpdateTime = DateTime.Now;
            }
        }
    }
} 