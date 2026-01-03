using System;
using System.Windows;
using System.Windows.Threading;
using XInputium.XInput;

namespace daisywheel_osk
{
    public partial class MainWindow : Window
    {
        private readonly DispatcherTimer _timer;
        private XGamepad _gamepad;
        private XInputDeviceManager _deviceManager;

        public MainWindow()
        {
            InitializeComponent();

            // Logical device used by the app
            _gamepad = new XGamepad();

            _gamepad.LeftJoystick.InnerDeadZone = 0.2f;

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
            _timer.Start();
        }

        private void UpdateController(object? sender, EventArgs e)
        {
            // Poll device manager first to refresh connection states
            _deviceManager.Update();

            if (!_gamepad.IsConnected)
            {
                OutputText.Text = "No controller connected";
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

            OutputText.Text =
                $"LX: {lx:F3}, LY: {ly:F3}\n" +
                $"A: {aPressed}, B: {bPressed}\n" +
                $"X: {xPressed}, Y: {yPressed}\n" +
                $"Left Stick Angle: {decimal.Round((decimal)_gamepad.LeftJoystick.Angle, 2)}\n" +
                $"Selected Petal: {_wheel.ActiveSegment}";
        }
        private void UpdateSize(object sender, SizeChangedEventArgs e)
        {
            _wheel.UpdateSize();
        }

        private void _wheel_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
