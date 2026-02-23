using System;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace daisywheel_osk
{
    public class HotkeyService : IDisposable
    {
        [DllImport("user32.dll")] private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);
        [DllImport("user32.dll")] private static extern bool UnhookWindowsHookEx(IntPtr hhk);
        [DllImport("user32.dll")] private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("kernel32.dll")] private static extern IntPtr GetModuleHandle(string lpModuleName);

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;

        private IntPtr _hookHandle;
        private readonly LowLevelKeyboardProc _proc; // keep reference to prevent GC

        // The hotkey to listen for — easily made configurable later
        public Key TriggerKey { get; set; } = Key.D;
        public bool RequireControl { get; set; } = false;
        public bool RequireShift { get; set; } = true;
        public bool RequireWin { get; set; } = true;

        public event Action? HotkeyPressed;
        public event Action? EscapePressed;

        public HotkeyService()
        {
            _proc = HookCallback;
        }

        public void Install()
        {
            using var module = System.Diagnostics.Process.GetCurrentProcess().MainModule!;
            _hookHandle = SetWindowsHookEx(WH_KEYBOARD_LL, _proc, GetModuleHandle(module.ModuleName!), 0);
        }

        public void Uninstall()
        {
            if (_hookHandle != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_hookHandle);
                _hookHandle = IntPtr.Zero;
            }
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                var key = KeyInterop.KeyFromVirtualKey(vkCode);

                bool ctrlOk = !RequireControl || (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl));
                bool shiftOk = !RequireShift || (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift));
                bool winOk = !RequireWin || (Keyboard.IsKeyDown(Key.LWin) || Keyboard.IsKeyDown(Key.RWin));

                if (key == TriggerKey && ctrlOk && shiftOk && winOk)
                {
                    HotkeyPressed?.Invoke();
                    return (IntPtr)1;
                }

                if (key == Key.Escape)
                {
                    EscapePressed?.Invoke();
                    return (IntPtr)1;
                }
            }

            return CallNextHookEx(_hookHandle, nCode, wParam, lParam);
        }

        public void Dispose() => Uninstall();
    }
}