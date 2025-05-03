using CrashTimeUnleashed.Entities;
using System.Runtime.InteropServices;

namespace CrashTimeUnleashed.Modules
{
    public class NoClip : BaseModule
    {
        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(int vKey);

        private float _speed;

        public NoClip(Player player) : base(player)
        {
            _speed = 1.0f;
        }

        protected override void OnEnable()
        {
            // I'm currently locking all rotations as I was unable to correctly calculate
            // keystrokes to match the angle, hopefully I will resolve this soon
            _player.WriteFloat(_player.xRotAddr, 0.0f);
            _player.WriteFloat(_player.yRotAddr, 0.0f);
            _player.WriteFloat(_player.zRotAddr, 0.0f);
            base.OnEnable();
        }

        public override void Update()
        {
            if (!Enabled) return;

            float x = _player.ReadFloat(_player.xAddr);
            float y = _player.ReadFloat(_player.yAddr);
            float z = _player.ReadFloat(_player.zAddr);

            _player.WriteFloat(_player.xRotAddr, 0.0f);
            _player.WriteFloat(_player.yRotAddr, 0.0f);
            _player.WriteFloat(_player.zRotAddr, 0.0f);

            float moveX = 0;
            float moveY = 0;
            float moveZ = 0;

            if ((GetAsyncKeyState(0x57) & 0x8000) != 0) // W
            {
                moveZ += _speed;
            }
            if ((GetAsyncKeyState(0x53) & 0x8000) != 0) // S
            {
                moveZ -= _speed;
            }

            if ((GetAsyncKeyState(0x41) & 0x8000) != 0) // A
            {
                moveX -= _speed;
            }
            if ((GetAsyncKeyState(0x44) & 0x8000) != 0) // D
            {
                moveX += _speed;
            }

            if ((GetAsyncKeyState(0x20) & 0x8000) != 0) // Space
            {
                moveY += _speed;
            }
            if ((GetAsyncKeyState(0x11) & 0x8000) != 0) // Left Ctrl
            {
                moveY -= _speed;
            }

            _player.WriteFloat(_player.xAddr, x + moveX);
            _player.WriteFloat(_player.yAddr, y + moveY);
            _player.WriteFloat(_player.zAddr, z + moveZ);
        }

        public void SetSpeed(float newSpeed)
        {
            _speed = newSpeed;
        }

        public float GetSpeed()
        {
            return _speed;
        }
    }
}
