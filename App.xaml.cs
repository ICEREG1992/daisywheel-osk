using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace daisywheel_osk
{
    public partial class App : Application
    {
        private TaskbarIcon _trayIcon = null!;
        private MainWindow? _mainWindow;
        private SettingsWindow? _settingsWindow;

        private HotkeyService _hotkeyService = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _trayIcon = (TaskbarIcon)FindResource("TrayIcon");
            _trayIcon.TrayMouseDoubleClick += (_, _) => ToggleWindow();

            _mainWindow = new MainWindow();

            _hotkeyService = new HotkeyService
            {
                TriggerKey = Key.D,
                RequireShift = true,
                RequireWin = true,
                RequireControl = false
            };
            _hotkeyService.HotkeyPressed += () => Dispatcher.InvokeAsync(ToggleWindow);
            _hotkeyService.EscapePressed += () => Dispatcher.InvokeAsync(() =>
            {
                if (_mainWindow?.IsVisible == true)
                    _mainWindow.Hide();
            });
            _hotkeyService.Install();
        }

        private void ToggleWindow()
        {
            if (_mainWindow == null) return;

            if (_mainWindow.IsVisible)
            {
                _mainWindow.Hide();
            }
            else
            {
                _mainWindow.Show();
            }
        }

        private void TraySettings_Click(object sender, RoutedEventArgs e)
        {
            if (_settingsWindow == null || !_settingsWindow.IsLoaded)
                _settingsWindow = new SettingsWindow();

            _settingsWindow.Show();
            _settingsWindow.Activate();
        }
        private void TrayExit_Click(object sender, RoutedEventArgs e) => Shutdown();

        protected override void OnExit(ExitEventArgs e)
        {
            _hotkeyService.Dispose();
            _trayIcon.Dispose();
            base.OnExit(e);
        }
    }
}