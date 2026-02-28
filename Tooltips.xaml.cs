using DaisywheelOsk;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using XInputium.XInput;

namespace daisywheel_osk
{
    public partial class Tooltips : UserControl
    {
        private readonly Tooltip[][] _tooltips;
        private float Size { get; set; } = SettingsStore.Instance.Settings.Size;

        public Tooltips()
        {
            InitializeComponent();

            _tooltips = new Tooltip[2][];

            // Sample text — swap these out for real settings later
            (string label, string buttonText, string key)[] row1 =
            [
                ("Backspace", "X", "\b"),
                ("Enter", "A", "\r"),
                ("Space", "Y", " ")
            ];
            (string label, string buttonText, string key)[] row2 =
            [
                ("Numbers", "LT", ""),
                ("Caps", "RT", "")
            ];

            _tooltips[0] = new Tooltip[row1.Length];
            _tooltips[1] = new Tooltip[row2.Length];
            for (int i = 0; i < row1.Length; i++)
            {
                var (label, buttonText, key) = row1[i];
                var tooltip = new Tooltip(label, buttonText, key);
                _tooltips[0][i] = tooltip;
                _canvas.Children.Add(tooltip);
            }
            for (int i = 0; i < row2.Length; i++)
            {
                var (label, buttonText, key) = row2[i];
                var tooltip = new Tooltip(label, buttonText, key);
                _tooltips[1][i] = tooltip;
                _canvas.Children.Add(tooltip);
            }
        }

        public void UpdateSize(double windowWidth, double windowHeight)
        {
            double size = SettingsStore.Instance.Settings.Size;
            double dimension = Math.Min(windowWidth, windowHeight);
            double wheelRadius = dimension * (size / 100.0) / 2.0;

            _canvas.Width = windowWidth;
            _canvas.Height = windowHeight / 2.0 - wheelRadius;

            const double spacing = 16.0;
            const double rowSpacing = 8.0;
            var infinite = new Size(double.PositiveInfinity, double.PositiveInfinity);

            // Measure all tooltips first
            foreach (var row in _tooltips)
                foreach (var tooltip in row)
                    tooltip.Measure(infinite);

            double rowHeight = _tooltips[0][0].DesiredSize.Height; // assume uniform height
            double totalRowsHeight = rowHeight * _tooltips.Length + rowSpacing * (_tooltips.Length - 1);
            double startY = (_canvas.Height - totalRowsHeight) / 2.0;

            for (int r = 0; r < _tooltips.Length; r++)
            {
                var row = _tooltips[r];
                double totalWidth = 0;
                foreach (var tooltip in row)
                    totalWidth += tooltip.DesiredSize.Width;
                totalWidth += spacing * (row.Length - 1);

                double x = (_canvas.Width - totalWidth) / 2.0;
                double y = startY + r * (rowHeight + rowSpacing);

                foreach (var tooltip in row)
                {
                    Canvas.SetLeft(tooltip, x);
                    Canvas.SetTop(tooltip, y);
                    x += tooltip.DesiredSize.Width + spacing;
                }
            }
        }

        public void HandleButtonPress(XInputButton button, bool wheelActive)
        {
            foreach (var row in _tooltips)
                foreach (var tooltip in row)
                    tooltip.HandleButtonPress(button, wheelActive);
        }

        internal void HandleButtonRelease(XInputButton button, bool wheelActive)
        {
            foreach (var row in _tooltips)
                foreach (var tooltip in row)
                    tooltip.HandleButtonRelease(button, wheelActive);
        }
    }
}