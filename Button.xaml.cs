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
            Pressed = true;
            // send keypress event
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
