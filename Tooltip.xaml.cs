using System.Windows.Controls;
using XInputium.XInput;

namespace daisywheel_osk
{
    public partial class Tooltip : UserControl
    {
        private Button _button;
        private static readonly string[] ButtonColors =
        [
            "#01306B",
            "#BE8B00",
            "#BA1E00",
            "#608300"
        ];
        private static readonly string[] WheelReserved = { "X","Y","B","A" };
        public Tooltip(string text, string buttonText, string key)
        {
            InitializeComponent();
            _text.Text = text;

            int reservedIndex = Array.IndexOf(WheelReserved, buttonText);
            string color = reservedIndex >= 0 ? ButtonColors[reservedIndex] : "#0D2F47";

            _button = new Button(16, buttonText, color, key);
            _button.Focused = true;
            _button.UpdateButton();
            _canvas.Children.Add(_button.C);
        }

        internal void HandleButtonPress(XInputButton button, bool wheelActive)
        {
            // don't do anything if _button's text is A,B,X,Y and the wheel is active, since those are reserved for the wheel
            
            if (wheelActive && WheelReserved.Contains(_button.C.Children.OfType<Viewbox>().FirstOrDefault()?.Child is TextBlock tb ? tb.Text : ""))
                return;

            if (_button.text.Text == button.ToString())
            {
                _button.Press();
                _button.UpdateButton();
            }
        }

        internal void HandleButtonRelease(XInputButton button, bool wheelActive)
        {
            if (_button.text.Text == button.ToString())
            {
                _button.Release();
                _button.UpdateButton();
            }
        }
    }
}