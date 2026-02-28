using System.Windows.Controls;

namespace daisywheel_osk
{
    public partial class Tooltip : UserControl
    {
        private Button _button;
        public Tooltip(string text)
        {
            InitializeComponent();
            _text.Text = text;
            _button = new Button(16, "A", "#0D2F47");
            _canvas.Children.Add(_button.C);
        }
    }
}