﻿using ClickableTransparentOverlay;
using ImGuiNET;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using CrashTimeUnleashed.Modules;
using CrashTimeUnleashed.Entities;
using Swed64;

namespace CrashTimeUnleashed
{
    public class Renderer : Overlay
    {
        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(int vKey);

        private int targetFrameRate = 60;
        private DateTime lastFrameTime = DateTime.Now;
        private TimeSpan frameBudget;

        private bool showGUI = true;
        private bool prevInsertState = false;

        private Vector2 windowSize = new Vector2(600, 400);
        private bool initialized = false;

        private NoClip? _noClip;
        private GodMode? _godMode;
        private InfiniteNitro? _infiniteNitro;
        private Player? _player;

        private Memory? _memory;

        private int _xCoord = 0;
        private int _yCoord = 0;
        private int _zCoord = 0;

        private int _propertyDamage = 0;

        public Renderer() : base("Crash Time 2 - Unleashed Trainer", true, 2560, 1440)
        {
            frameBudget = TimeSpan.FromSeconds(1.0 / targetFrameRate);
        }

        public void Initialize(Memory memory)
        {
            _memory = memory;
            _player = memory.InitializePlayer();
            if (_player != null)
            {
                _noClip = new NoClip(_player);
                _godMode = new GodMode(_player);
                _infiniteNitro = new InfiniteNitro(_player);
            }
        }

        protected override void Render()
        {
            // Aim for those 60 FPS (Prevents your CPU from getting cooked)
            TimeSpan elapsed = DateTime.Now - lastFrameTime;
            if (elapsed < frameBudget)
            {
                int sleepTime = (int)((frameBudget - elapsed).TotalMilliseconds);
                if (sleepTime > 1)
                    Thread.Sleep(sleepTime);
            }
            lastFrameTime = DateTime.Now;

            if (!initialized)
            {
                SetupWindowStyle();

                string? exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string fontPath = Path.Combine(exeDir!, "Fonts", "FiraSans-Regular.ttf");

                ReplaceFont(fontPath, 18, FontGlyphRangeType.English);

                ImGui.SetNextWindowSize(windowSize, ImGuiCond.Once);
                ImGui.SetNextWindowPos(new Vector2(100, 100), ImGuiCond.Once);

                if (_memory == null)
                {
                    Console.WriteLine("[!] Memory not initialized. Cannot set up GUI.");
                    return;
                }

                _propertyDamage = _memory.GetPropertyDamage();
                initialized = true;
            }

            bool isInsertPressed = (GetAsyncKeyState(0x2D) & 0x8000) != 0;

            if (isInsertPressed && !prevInsertState)
            {
                showGUI = !showGUI;
            }
            prevInsertState = isInsertPressed;

            // Update modules
            if (_noClip != null)
            {
                _noClip.Update();
            }

            if (_godMode != null)
            {
                _godMode.Update();
            }
            
            if (_infiniteNitro != null)
            {
                _infiniteNitro.Update();
            }

            if (!showGUI)
                return;

            ImGui.Begin("Crash Time 2 - Unleashed Trainer");

            if (ImGui.BeginTabBar("MyTabBar"))
            {
                // Tab - NoClip
                if (ImGui.BeginTabItem("NoClip"))
                {
                    ImGui.Spacing();
                    ImGui.Text("NoClip");
                    if (_noClip != null)
                    {
                        bool enabled = _noClip.Enabled;
                        if (ImGui.Checkbox("Enabled", ref enabled))
                        {
                            _noClip.Enabled = enabled;
                        }
                        float speed = _noClip.GetSpeed();
                        ImGui.SetNextItemWidth(200.0f);
                        if (ImGui.SliderFloat("NoClip Speed", ref speed, 0.1f, 20.0f))
                        {
                            _noClip.SetSpeed(speed);
                        }
                    }
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("GodMode"))
                {
                    ImGui.Spacing();
                    ImGui.Text("GodMode [!] Experimental [!]");
                    if (_godMode != null)
                    {
                        bool enabled = _godMode.Enabled;
                        if (ImGui.Checkbox("Enabled", ref enabled))
                        {
                            _godMode.Enabled = enabled;
                        }
                        
                        ImGui.Spacing();

                        ImGui.Text("GodMode is an experimental feature,");
                        ImGui.Text("it might not work properly or cause your game to crash!");
                    }
                    ImGui.EndTabItem();
                }
                
                // Tab - Nitro
                if (ImGui.BeginTabItem("Nitro"))
                {
                    ImGui.Spacing();
                    if (_infiniteNitro != null)
                    {
                        ImGui.Text("Infinite Nitro");
                        
                        bool enabled = _infiniteNitro.Enabled;
                        if (ImGui.Checkbox("Enabled", ref enabled))
                        {
                            _infiniteNitro.Enabled = enabled;
                        }
                    }
                    ImGui.EndTabItem();
                }

                // Tab - Teleportation
                if (ImGui.BeginTabItem("Teleportation"))
                {
                    ImGui.Text("Set coordinates");
                    ImGui.InputInt("Coord X", ref _xCoord);
                    ImGui.InputInt("Coord Y", ref _yCoord);
                    ImGui.InputInt("Coord Z", ref _zCoord);

                    if (ImGui.Button("Teleport to coordinates"))
                    {
                        if (_player != null)
                        {
                            _player.WriteFloat(_player.xAddr, _xCoord);
                            _player.WriteFloat(_player.yAddr, _yCoord);
                            _player.WriteFloat(_player.zAddr, _zCoord);
                        }
                    }

                    ImGui.Spacing();
                    ImGui.Text("Predefined Locations:");

                    // Autobahn category
                    if (ImGui.CollapsingHeader("Autobahn"))
                    {
                        if (ImGui.Button("Unfinished Building (Roof)"))
                        {
                            if (_player != null)
                            {
                                _player.WriteFloat(_player.xAddr, -811.89f);
                                _player.WriteFloat(_player.yAddr, 114.00f);
                                _player.WriteFloat(_player.zAddr, -569.51f);
                            }
                        }

                        // If you wish to contribute some location for Autobahn then please insert it here! <3
                    }

                    // City category
                    if (ImGui.CollapsingHeader("City"))
                    {
                        if (ImGui.Button("Northwest Factory (MadCop Part 1)"))
                        {
                            if (_player != null)
                            {
                                _player.WriteFloat(_player.xAddr, -916.72f);
                                _player.WriteFloat(_player.yAddr, 49.00f);
                                _player.WriteFloat(_player.zAddr, 660.16f);
                            }
                        }

                        if (ImGui.Button("Northwest Factory (MadCop Part 2)"))
                        {
                            if (_player != null)
                            {
                                _player.WriteFloat(_player.xAddr, -1156.61f);
                                _player.WriteFloat(_player.yAddr, 32.00f);
                                _player.WriteFloat(_player.zAddr, 502.70f);
                            }
                        }

                        // If you wish to contribute some location for City then please insert it here! <3
                    }

                    ImGui.EndTabItem();
                }

                // Tab - Statistics
                if (ImGui.BeginTabItem("Statistics"))
                {
                    ImGui.Text("Profile Statistics");
                    ImGui.InputInt("Property Damage", ref _propertyDamage);

                    if (ImGui.Button("Set Property Damage"))
                    {
                        if (_memory != null)
                        {
                            _memory.WritePropertyDamage(_propertyDamage);
                        }
                    }
                    ImGui.EndTabItem();
                }

                // Tab - Debug
                if (ImGui.BeginTabItem("Debug"))
                {
                    if (_player != null)
                    {
                        float x = _player.ReadFloat(_player.xAddr);
                        float y = _player.ReadFloat(_player.yAddr);
                        float z = _player.ReadFloat(_player.zAddr);
                        float xRot = _player.ReadFloat(_player.xRotAddr);
                        float yRot = _player.ReadFloat(_player.yRotAddr);
                        float zRot = _player.ReadFloat(_player.zRotAddr);
                        float carIndex = _player.ReadInt(_player.carIndexAddr);

                        ImGui.Text("Coordinates:");
                        ImGui.Text($"X: {x:F2}");
                        ImGui.Text($"Y: {y:F2}");
                        ImGui.Text($"Z: {z:F2}");

                        ImGui.Spacing();

                        ImGui.Text("Rotations:");
                        ImGui.Text($"X Rotation (Yaw): {xRot:F2}");
                        ImGui.Text($"Y Rotation (Pitch): {yRot:F2}");
                        ImGui.Text($"Z Rotation (Roll): {zRot:F2}");

                        ImGui.Spacing();

                        ImGui.Text($"Car Index: {carIndex}");

                        ImGui.NewLine();

                        ImGui.Text("This is a very early version of the trainer.");
                        ImGui.Text("Feel free to contribute to the project on GitHub.");
                        ImGui.Text("Made with love by bulva2 - https://github.com/bulva2/UnleashedTrainer <3");
                    }
                    ImGui.EndTabItem();
                }
            }

            ImGui.EndTabBar();
            ImGui.End();
        }

        public void SetupWindowStyle()
        {
            ImGuiStylePtr style = ImGui.GetStyle();
            style.FrameRounding = 3.0f;
            style.WindowRounding = 7.0f;
            style.ScrollbarRounding = 0;
            style.Alpha = 1;

            style.Colors[(int)ImGuiCol.Border] = new Vector4(1.00f, 0.5f, 0.0f, 1.0f);
            style.Colors[(int)ImGuiCol.FrameBg] = new Vector4(1.00f, 0.50f, 0.00f, 0.80f);
            style.Colors[(int)ImGuiCol.FrameBgHovered] = new Vector4(1.00f, 0.50f, 0.00f, 0.50f);
            style.Colors[(int)ImGuiCol.FrameBgActive] = new Vector4(1.00f, 0.50f, 0.00f, 0.50f);
            style.Colors[(int)ImGuiCol.TitleBg] = new Vector4(0.96f, 0.96f, 0.96f, 1.00f);
            style.Colors[(int)ImGuiCol.TitleBgCollapsed] = new Vector4(1.00f, 1.00f, 1.00f, 0.51f);
            style.Colors[(int)ImGuiCol.TitleBgActive] = new Vector4(1.00f, 0.50f, 0.00f, 1.00f);
            style.Colors[(int)ImGuiCol.CheckMark] = new Vector4(1.00f, 1.00f, 1.00f, 1.00f);
            style.Colors[(int)ImGuiCol.ResizeGrip] = new Vector4(1.00f, 0.50f, 0.00f, 1.00f);
            style.Colors[(int)ImGuiCol.ResizeGripHovered] = new Vector4(1.00f, 0.50f, 0.00f, 0.80f);
            style.Colors[(int)ImGuiCol.ResizeGripActive] = new Vector4(1.00f, 0.50f, 0.00f, 0.70f);
            style.Colors[(int)ImGuiCol.Button] = new Vector4(1.00f, 0.5f, 0.0f, 1.0f);
            style.Colors[(int)ImGuiCol.ButtonHovered] = new Vector4(1.00f, 0.50f, 0.00f, 0.50f);
            style.Colors[(int)ImGuiCol.ButtonActive] = new Vector4(1.00f, 0.50f, 0.00f, 0.50f);
            style.Colors[(int)ImGuiCol.Header] = new Vector4(1.00f, 0.50f, 0.00f, 1.00f);
            style.Colors[(int)ImGuiCol.HeaderHovered] = new Vector4(1.00f, 0.50f, 0.00f, 0.50f);
            style.Colors[(int)ImGuiCol.HeaderActive] = new Vector4(1.00f, 0.50f, 0.00f, 0.50f);
            style.Colors[(int)ImGuiCol.Tab] = new Vector4(1.00f, 0.50f, 0.00f, 1.00f);
            style.Colors[(int)ImGuiCol.TabHovered] = new Vector4(1.00f, 0.50f, 0.00f, 0.50f);
            style.Colors[(int)ImGuiCol.TabSelected] = new Vector4(1.00f, 0.50f, 0.00f, 0.50f);
            style.Colors[(int)ImGuiCol.SliderGrab] = new Vector4(1.00f, 1.00f, 1.00f, 1.00f);
            style.Colors[(int)ImGuiCol.SliderGrabActive] = new Vector4(1.00f, 0.50f, 0.00f, 1.00f);
        }
    }
}
