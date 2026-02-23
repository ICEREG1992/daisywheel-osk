using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace daisywheel_osk
{
    public class Button
    {
        public Canvas C;
        public bool Pressed = false;
        public bool Focused = false;

        private System.Windows.Shapes.Ellipse e { get; set; }
        private Viewbox viewbox { get; set; }
        private TextBlock text { get; set; }

        private const string ButtonColor = "#0D2F47";
        private string color { get; set; } = "#0D2F47";

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        [StructLayout(LayoutKind.Explicit, Size = 40)]
        private struct INPUT
        {
            [FieldOffset(0)] public uint type;
            [FieldOffset(8)] public KEYBDINPUT ki;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }
        private const uint INPUT_KEYBOARD = 1;
        private const uint KEYEVENTF_UNICODE = 0x0004;
        private const uint KEYEVENTF_KEYUP = 0x0002;

        public Button(double s, string t, string c)
        {
            C = new Canvas
            {
                Width = s,
                Height = s
            };

            e = new System.Windows.Shapes.Ellipse
            {
                Width = s,
                Height = s,
                Fill = (SolidColorBrush)new BrushConverter().ConvertFrom(ButtonColor)
            };

            C.Children.Add(e);

            text = new TextBlock
            {
                Text = t,
                Foreground = Brushes.White
            };

            viewbox = new Viewbox
            {
                Child = text,
                Stretch = Stretch.Uniform,
                Width = s,
                Height = s
            };

            C.Children.Add(viewbox);

            color = c;
        }

        public void Press()
        {
            if (Pressed) return;
            System.Diagnostics.Debug.WriteLine($"INPUT size: {Marshal.SizeOf(typeof(INPUT))}");
            Pressed = true;
            // send keypress event
            System.Diagnostics.Debug.WriteLine($"Press() called with text: '{text.Text}'");
            if (text.Text.Length == 0) return;
            char ch = text.Text[0];
            System.Diagnostics.Debug.WriteLine($"Sending char: '{ch}' (Unicode: {(int)ch})");

            INPUT[] inputs = new INPUT[]
            {
                new INPUT
                {
                    type = INPUT_KEYBOARD,
                    ki = new KEYBDINPUT
                    {
                        wVk = 0,
                        wScan = ch,
                        dwFlags = KEYEVENTF_UNICODE,
                        time = 0,
                        dwExtraInfo = 0
                    }
                },
                new INPUT
                {
                    type = INPUT_KEYBOARD,
                    ki = new KEYBDINPUT
                    {
                        wVk = 0,
                        wScan = ch,
                        dwFlags = KEYEVENTF_UNICODE | KEYEVENTF_KEYUP,
                        time = 0,
                        dwExtraInfo = 0
                    }
                }
            };

            uint result = SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
            Debug.WriteLine($"SendInput result: {result}, LastError: {Marshal.GetLastWin32Error()}");
        }


        public void Release()
        {
            Pressed = false;
        }

        public void UpdateButton()
        {
            if (Pressed)
            {
                e.Fill = (SolidColorBrush)new BrushConverter().ConvertFrom("#505050");
            }
            else if (Focused)
            {
                e.Fill = (SolidColorBrush)new BrushConverter().ConvertFrom(color);
            }
            else
            {
                e.Fill = (SolidColorBrush)new BrushConverter().ConvertFrom(ButtonColor);
            }
        }

        public void UpdateSize(double s)
        {
            e.Width = s;
            e.Height = s;
            viewbox.Width = s;
            viewbox.Height = s;
        }
    }
}
