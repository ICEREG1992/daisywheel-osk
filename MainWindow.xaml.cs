using DaisywheelOsk;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Packaging;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;
using XInputium.XInput;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace daisywheel_osk
{
    public partial class MainWindow : Window
    {
        private readonly DispatcherTimer _timer;
        private XGamepad _gamepad;
        private XInputDeviceManager _deviceManager;

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hwnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hwnd, int nIndex, int dwNewLong);

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TRANSPARENT = 0x00000020;
        private const int WS_EX_LAYERED = 0x00080000;
        private const int WS_EX_NOACTIVATE = 0x08000000;
        private const int WS_EX_TOOLWINDOW = 0x00000080;

        public MainWindow()
        {
            InitializeComponent();

            _wheel.SetLayout(LoadLayout(SettingsStore.Instance.Settings.Layout));

            // Logical device used by the app
            _gamepad = new XGamepad();

            _gamepad.LeftJoystick.InnerDeadZone = 0.2f;

            _gamepad.ButtonPressed += (s, e) =>
            {
                _wheel.HandleButtonPress(e.Button);
                _tooltips.HandleButtonPress(e.Button, _wheel.Active);
            };
            _gamepad.ButtonReleased += (s, e) =>
            {
                _wheel.HandleButtonRelease(e.Button);
                _tooltips.HandleButtonRelease(e.Button, _wheel.Active);
            };
            XInputium.DigitalButton leftTrigger = _gamepad.LeftTrigger.ToDigitalButton(0.5f);
            XInputium.DigitalButton rightTrigger = _gamepad.RightTrigger.ToDigitalButton(0.5f);

            leftTrigger.IsPressedChanged += (s, e) =>
            {
                _wheel.HandleTriggers(leftTrigger.IsPressed, rightTrigger.IsPressed);
            };
            rightTrigger.IsPressedChanged += (s, e) =>
            {
                _wheel.HandleTriggers(leftTrigger.IsPressed, rightTrigger.IsPressed);
            };


            // Manager that monitors all physical controllers
            _deviceManager = new XInputDeviceManager();

            // Subscribe to connection/disconnection events
            _deviceManager.DeviceStateChanged += (_, e) =>
            {
                // Only react to connection events
                if (e.Device.IsConnected)
                {
                    // Attach the logical XGamepad to the connected physical device
                    _gamepad.Device = e.Device;
                }
                else
                {
                    // If the device disconnected, detach it
                    if (_gamepad.Device == e.Device)
                        _gamepad.Device = null;
                }
            };

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(16)
            };
            _timer.Tick += UpdateController;

            IsVisibleChanged += (_, e) =>
            {
                if ((bool)e.NewValue)
                {
                    WindowState = WindowState.Maximized;
                    _timer.Start();
                }
                else
                {
                    WindowState = WindowState.Minimized;
                    _timer.Stop();
                }
            };
        }

        private Layout LoadLayout(string path)
        {
            String filepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "layouts", path + ".yaml");
            var yaml = File.ReadAllText(filepath);
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            var raw = deserializer.Deserialize<Dictionary<string, object>>(yaml);

            String name = raw["layout"].ToString();
            var alphabets = new List<LayoutAlphabet>();

            var keyboards = (List<object>)raw["alphabets"];
            foreach (var entry in keyboards)
            {
                var dict = (Dictionary<object, object>)entry;
                foreach (var kvp in dict)
                {
                    // var alphabetName = kvp.Key.ToString();
                    var rows = (List<object>)kvp.Value;
                    var petals = new List<LayoutPetal>();

                    foreach (var row in rows)
                    {
                        var chars = (List<object>)row;
                        petals.Add(new LayoutPetal(
                            chars[0].ToString(),
                            chars[1].ToString(),
                            chars[2].ToString(),
                            chars[3].ToString()
                        ));
                    }

                    alphabets.Add(new LayoutAlphabet(petals));
                }
            }

            return new Layout(name, alphabets);
        }


        private void UpdateController(object? sender, EventArgs e)
        {
            // Poll device manager first to refresh connection states
            _deviceManager.Update();

            if (!_gamepad.IsConnected)
            {
                DebugText.Text = "No controller connected";
                return;
            }

            _gamepad.Update();

            // Update wheel angle
            _wheel.Angle = _gamepad.LeftJoystick.Angle;
            if (_gamepad.LeftJoystick.X == 0 && _gamepad.LeftJoystick.Y == 0)
            {
                _wheel.Active = false;
            }
            else
            {  
                _wheel.Active = true;
            }
            _wheel.UpdateWheel();

            float lx = _gamepad.LeftJoystick.X;
            float ly = _gamepad.LeftJoystick.Y;

            bool aPressed = _gamepad.Buttons.A.IsPressed;
            bool bPressed = _gamepad.Buttons.B.IsPressed;
            bool xPressed = _gamepad.Buttons.X.IsPressed;
            bool yPressed = _gamepad.Buttons.Y.IsPressed;
            DebugText.Text =
                $"LX: {lx:F3}, LY: {ly:F3}\n" +
                $"A: {aPressed}, B: {bPressed}\n" +
                $"X: {xPressed}, Y: {yPressed}\n" +
                $"Left Stick Angle: {decimal.Round((decimal)_gamepad.LeftJoystick.Angle, 2)}\n" +
                $"Selected Petal: {_wheel.ActiveSegment}";
        }

        private void UpdateSize(object sender, SizeChangedEventArgs e)
        {
            _wheel.UpdateSize();
            _tooltips.UpdateSize(ActualWidth, ActualHeight);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.Escape)
                Hide();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var hwnd = new WindowInteropHelper(this).Handle;
            int style = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, style | WS_EX_TRANSPARENT | WS_EX_LAYERED | WS_EX_NOACTIVATE | WS_EX_TOOLWINDOW);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }


    }
}
