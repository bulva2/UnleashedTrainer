using System;
using System.Collections.Generic;
using CrashTimeUnleashed.Entities;
using CrashTimeUnleashed.Utils;
using System.Runtime.InteropServices;
using ImGuiNET;
using System.Diagnostics;
using System.Threading;

namespace CrashTimeUnleashed.Modules
{
    public class GodMode : BaseModule
    {
        private bool _patched = false;
        private List<PatchData> _patchAddresses = new List<PatchData>();
        private List<IntPtr> _zeroValueAddresses = new List<IntPtr>();
        private DateTime _lastResetTime = DateTime.MinValue;
        private int _resetFrequency = 25; // milliseconds

        private class PatchData
        {
            public IntPtr Address;
            public int Size;
            public byte[] OriginalBytes;
            public bool Disabled = false;

            public PatchData(IntPtr address, int size)
            {
                Address = address;
                Size = size;
                OriginalBytes = new byte[size];
            }
        }

        public GodMode(Player player) : base(player)
        {
            InitializeAddresses(player);
        }
        
        private void InitializeAddresses(Player player)
        {
            _patchAddresses.Clear();
            _zeroValueAddresses.Clear();
            
            _patchAddresses.Add(new PatchData(player.GetModuleBase() + 0x47433C, 7)); // 0047433C - F3 0F11 AE 840D0000  - movss [esi+00000D84],xmm5
            _patchAddresses.Add(new PatchData(player.GetModuleBase() + 0x4743BA, 7)); // 004743BA - F3 0F11 86 840D0000  - movss [esi+00000D84],xmm0
            _patchAddresses.Add(new PatchData(player.GetModuleBase() + 0x474448, 7)); // 00474448 - F3 0F11 86 840D0000  - movss [esi+00000D84],xmm0
            _patchAddresses.Add(new PatchData(player.GetModuleBase() + 0x474494, 7)); // 00474494 - F3 0F11 86 840D0000  - movss [esi+00000D84],xmm0
            _patchAddresses.Add(new PatchData(player.GetModuleBase() + 0x4744B0, 7)); // 004744B0 - F3 0F11 8E 840D0000  - movss [esi+00000D84],xmm1
            _patchAddresses.Add(new PatchData(player.GetModuleBase() + 0x0D8E0, 7));  // 0040D8E0 - F3 0F11 A7 940D0000  - movss [edi+00000D94],xmm4
            _patchAddresses.Add(new PatchData(player.GetModuleBase() + 0x744D9, 3));  // 004744D9 - D9 58 D8  - fstp dword ptr [eax-28]
            _patchAddresses.Add(new PatchData(player.GetModuleBase() + 0x742C5, 3));  // 004742C5 - F3 0F11 3A  - movss [edx],xmm7

            _zeroValueAddresses.Add(player.GetModuleBase() + 0x5BF658); // BurningWheelsHi.exe+5BF658
            _zeroValueAddresses.Add(player.GetModuleBase() + 0x5BF654); // BurningWheelsHi.exe+5BF654
            _zeroValueAddresses.Add(player.GetModuleBase() + 0x5BF644); // BurningWheelsHi.exe+5BF644
        }

        protected override void OnEnable()
        {
            if (!_patched)
            {
                EnableGodMode();
            }
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            if (_patched)
            {
                DisableGodMode();
            }
            base.OnDisable();
        }

        public override void Update()
        {
            if (!_enabled) return;

            TimeSpan elapsed = DateTime.Now - _lastResetTime;
            if (elapsed.TotalMilliseconds > _resetFrequency)
            {
                SetZeroValues();
                _lastResetTime = DateTime.Now;
            }
        }

        private void EnableGodMode()
        {
            IntPtr processHandle = _player.GetProcessHandle();
            Console.WriteLine("[GodMode] Enabling...");

            int successCount = 0;
            foreach (var patch in _patchAddresses)
            {
                if (patch.Disabled) continue;
                
                try
                {
                    patch.OriginalBytes = _player.ReadBytes(patch.Address, patch.Size);
                    
                    MemoryPatcher.NopEx(processHandle, patch.Address, patch.Size);
                    successCount++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[GodMode] Error patching {patch.Address:X8}: {ex.Message}");
                    patch.Disabled = true;
                }
            }

            Console.WriteLine($"[GodMode] Successfully patched {successCount} of {_patchAddresses.Count} instructions");
            
            SetZeroValues();
            _patched = true;
        }

        private void DisableGodMode()
        {
            IntPtr processHandle = _player.GetProcessHandle();
            
            foreach (var patch in _patchAddresses)
            {
                if (patch.Disabled) continue;
                
                try
                {
                    MemoryPatcher.PatchEx(processHandle, patch.Address, patch.OriginalBytes);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[GodMode] Error restoring {patch.Address:X8}: {ex.Message}");
                }
            }

            _patched = false;
            Console.WriteLine("[GodMode] Disabled");
        }

        private void SetZeroValues()
        {
            foreach (var address in _zeroValueAddresses)
            {
                try
                {
                    float currentValue = _player.ReadFloat(address);
                    if (currentValue > 0.01f)
                    {
                        _player.WriteFloat(address, 0.0f);
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"[GodMode] Error setting zero value at {address:X8}: {ex.Message}");
                    Console.ResetColor();
                }
            }
        }
    }
} 