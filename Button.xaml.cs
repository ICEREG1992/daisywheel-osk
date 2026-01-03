using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace daisywheel_osk
{
    public class Button
    {
        public Canvas C;
        public bool Pressed = false;
        public bool Focused = false;

        private System.Windows.Shapes.Ellipse e { get; set; }
        private Viewbox viewbox {  get; set; }
        private System.Windows.Controls.TextBlock text { get; set; }
        private const string ButtonColor = "#0D2F47";
        private string color { get; set; } = "#0D2F47";

        public Button(double s, string t, string c)
        {
            C = new Canvas();
            C.Width = s;
            C.Height = s;

            // make shape
            
            e = new System.Windows.Shapes.Ellipse();
            e.Fill = (SolidColorBrush)new BrushConverter().ConvertFrom(ButtonColor);
            e.Width = s;
            e.Height = s;

            C.Children.Add(e);

            // make text in viewbox

            text = new System.Windows.Controls.TextBlock();
            text.Text = t;
            text.Foreground = Brushes.White;
            viewbox = new Viewbox();
            viewbox.Child = text;
            viewbox.Stretch = System.Windows.Media.Stretch.Uniform;
            viewbox.Width = s;
            viewbox.Height = s;

            C.Children.Add(viewbox);
            
            color = c;
        }

        public void UpdateButton()
        {
            if (Focused)
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
