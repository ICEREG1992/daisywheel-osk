// Wheel.xaml.cs
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using XInputium.XInput;
namespace daisywheel_osk
{
    public partial class Wheel : UserControl
    {
        public int SegmentCount { get; set; } = 8;
        public float Angle { get; set; } = 0;
        public bool Active { get; set; } = true;

        private const string WheelColor = "#09273B";
        private readonly Flower flower;
        private readonly System.Windows.Shapes.Ellipse ellipse;

        public Wheel()
        {
            InitializeComponent();

            ellipse = new System.Windows.Shapes.Ellipse();
            ellipse.Fill = (SolidColorBrush)new BrushConverter().ConvertFrom(WheelColor);

            flower = new Flower(SegmentCount, Math.Min(WheelCanvas.ActualWidth, WheelCanvas.ActualHeight));

            UpdateSize();

            WheelCanvas.Children.Add(ellipse);
            WheelCanvas.Children.Add(flower.C);
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
            double Size = Math.Min(actualWidth, actualHeight);
            ellipse.Width = Size;
            ellipse.Height = Size;
            Canvas.SetLeft(ellipse, actualWidth / 2 - Size / 2);
            Canvas.SetTop(ellipse, actualHeight / 2 - Size / 2);

            flower.UpdateSize(Size);
            Canvas.SetLeft(flower.C, actualWidth / 2 - Size / 2);
            Canvas.SetTop(flower.C, actualHeight / 2 - Size / 2);
        }

        private static int Mod(int x, int m) => (x % m + m) % m;

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
    }
}