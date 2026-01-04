// Petal.xaml.cs
using System.Windows.Controls;
using System.Windows.Media;
using XInputium.XInput;

namespace daisywheel_osk
{
    public class Petal
    {
        public Canvas C { get; set; }
        public bool Activated { get; set; }

        private System.Windows.Shapes.Ellipse e { get; set; }

        private const string PetalColor = "#0D2F47";
        private const string ActivatedColor = "#154E77";
        static readonly int ButtonCount = 4;
        private Button[] Buttons { get; set; }

        public char[] Chars { get; set; }
        private static readonly string[] ButtonColors =
        [
            "#01306B",
            "#BE8B00",
            "#BA1E00",
            "#608300"
        ];


        public Petal(double s, char[] c)
        {
            Chars = c;
            C = new Canvas();
            C.Width = s;
            C.Height = s;

            // make shape

            e = new System.Windows.Shapes.Ellipse();
            e.Fill = (SolidColorBrush)new BrushConverter().ConvertFrom(PetalColor);
            e.Width = s;
            e.Height = s;

            C.Children.Add(e);

            // make buttons

            Buttons = new Button[ButtonCount];

            double buttonSize = getButtonSize(s);
            for (int i = 0; i < ButtonCount; i++)
            {
                Buttons[i] = new Button(buttonSize, $"{Chars.GetValue(i)}", ButtonColors[i]);
                MoveButton(Buttons[i], s, i);
                C.Children.Add(Buttons[i].C);
            }
        }

        public void UpdatePetal()
        {
            if (Activated)
            {
                e.Fill = (SolidColorBrush)new BrushConverter().ConvertFrom(ActivatedColor);
                for (int i = 0; i < ButtonCount; ++i)
                {
                    Buttons[i].Focused = true;
                    Buttons[i].UpdateButton();
                }
            }
            else
            {
                e.Fill = (SolidColorBrush)new BrushConverter().ConvertFrom(PetalColor);
                for (int i = 0; i < ButtonCount; ++i)
                {
                    Buttons[i].Focused = false;
                    Buttons[i].UpdateButton();
                }
            }
        }

        public void UpdateSize(double s)
        {
            double trueSize = s;
            if (Activated)
            {
                trueSize = s * 1.1;
            }
            e.Width = trueSize;
            e.Height = trueSize;
            C.Width = trueSize;
            C.Height = trueSize;

            double buttonSize = getButtonSize(trueSize);
            for (int i = 0; i < ButtonCount; i++)
            {
                MoveButton(Buttons[i], trueSize, i);
                Buttons[i].UpdateSize(buttonSize);
            }
        }

        public void MoveButton(Button b, double s, int i)
        {
            double center = s / 2;

            double radius = getRadius(s);
            double petalSize = getButtonSize(s);

            double angle = 2 * Math.PI * i / ButtonCount;
            // Place the button so its center lies on the circle
            int x = (int)(center + radius * -1 * Math.Cos(angle) - petalSize / 2);
            int y = (int)(center + radius * -1 * Math.Sin(angle) - petalSize / 2);
            Canvas.SetLeft(Buttons[i].C, x);
            Canvas.SetTop(Buttons[i].C, y);
        }

        public double getButtonSize(double x)
        {
            return x / 3.5;
        }

        public double getRadius(double x)
        {
            return x / 2 / 2;
        }

        internal void HandleButtonPress(XInputButton b)
        {
            switch (b.Button)
            {
                case XButtons.X:
                    Buttons[0].Press();
                    break;
                case XButtons.Y:
                    Buttons[1].Press();
                    break;
                case XButtons.B:
                    Buttons[2].Press();
                    break;
                case XButtons.A:
                    Buttons[3].Press();
                    break;
                default:
                    break;
            }
        }

        internal void HandleButtonRelease()
        {
            foreach (Button b in Buttons)
            {
                b.Release();
            }
        }

        internal void HandleButtonRelease(XInputButton b)
        {
            switch (b.Button)
            {
                case XButtons.X:
                    Buttons[0].Release();
                    break;
                case XButtons.Y:
                    Buttons[1].Release();
                    break;
                case XButtons.B:
                    Buttons[2].Release();
                    break;
                case XButtons.A:
                    Buttons[3].Release();
                    break;
                default:
                    break;
            }
        }
    }
}

