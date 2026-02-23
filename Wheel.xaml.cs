// Wheel.xaml.cs
using DaisywheelOsk;
using System.Diagnostics;
using System.IO.Packaging;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using XInputium.XInput;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
namespace daisywheel_osk
{
    public partial class Wheel : UserControl
    {
        public float Size { get; set; } = 75; // Size as percentage of the smaller dimension of the canvas
        public float Angle { get; set; } = 0;
        public int Alphabet { get; set; } = 0;
        public bool Active { get; set; } = true;
        private int SegmentCount { get; set; }

        private Layout Layout;
        private const string WheelColor = "#09273B";
        private readonly Flower flower;
        private readonly System.Windows.Shapes.Ellipse ellipse;

        public Wheel()
        {
            InitializeComponent();

            ellipse = new System.Windows.Shapes.Ellipse();
            ellipse.Fill = (SolidColorBrush)new BrushConverter().ConvertFrom(WheelColor);

            flower = new Flower(Math.Min(WheelCanvas.ActualWidth, WheelCanvas.ActualHeight), LayoutAlphabet.blankAlphabet);

            UpdateSize();

            WheelCanvas.Children.Add(ellipse);
            WheelCanvas.Children.Add(flower.C);
        }

        public void SetLayout(Layout l)
        {
            Layout = l;
            LayoutAlphabet alphabet = Layout.GetAlphabet(Alphabet);
            SegmentCount = alphabet.NumPetals;
            flower.Alphabet = alphabet;
            flower.UpdateAlphabet();
        }

        public int? ActiveSegment
        {
            get
            {
                if (!Active)
                {
                    return null;
                }
                return Mod(Convert.ToInt32(Math.Floor((SegmentCount * Angle) + 0.5)), SegmentCount);
            }
        }

        public void UpdateWheel()
        {
            flower.SelectedPetal = ActiveSegment;
            flower.UpdateFlower();
        }

        public void UpdateSize()
        {
            double actualWidth = WheelCanvas.ActualWidth;
            double actualHeight = WheelCanvas.ActualHeight;
            double dimension = Math.Min(actualWidth, actualHeight);
            double trueSize = dimension * (Size / 100);
            ellipse.Width = trueSize;
            ellipse.Height = trueSize;
            Canvas.SetLeft(ellipse, actualWidth / 2 - trueSize / 2);
            Canvas.SetTop(ellipse, actualHeight / 2 - trueSize / 2);

            flower.UpdateSize(trueSize);
            Canvas.SetLeft(flower.C, actualWidth / 2 - trueSize / 2);
            Canvas.SetTop(flower.C, actualHeight / 2 - trueSize / 2);
        }

        public void UpdateAlphabet()
        {
            LayoutAlphabet alphabet = Layout.GetAlphabet(Alphabet);
            SegmentCount = alphabet.NumPetals;
            flower.Alphabet = alphabet;
            flower.UpdateAlphabet();
        }

        private static int Mod(int x, int m) => m == 0 ? 0 : (x % m + m) % m;

        internal void HandleButtonPress(XInputButton button)
        {
            if (Active)
            {
                flower.HandleButtonPress(button);
            }
        }

        internal void HandleButtonRelease(XInputButton button)
        {
            if (Active)
            {
                flower.HandleButtonRelease(button);
            }
        }

        internal void HandleTriggers(bool left, bool right)
        {
            if (left && right && Layout.NumAlphabets == 4)
                Alphabet = 3;
            else if (left && Layout.NumAlphabets >= 2)
                Alphabet = 1;
            else if (right && Layout.NumAlphabets >= 3)
                Alphabet = 2;
            else
                Alphabet = 0;
            
            UpdateAlphabet();
        }
    }
}