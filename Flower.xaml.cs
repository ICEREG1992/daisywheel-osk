// Flower.xaml.cs
using System;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using XInputium.XInput;

namespace daisywheel_osk
{
    public class Flower
    {
        public Canvas C { get; set; }
        public int SegmentCount { get; }
        public int SelectedPetal {  get; set; }
        private Petal[] Petals { get; set; }

        public Flower(int segCount, double size)
        {
            SegmentCount = segCount;
            C = new Canvas();

            Petals = new Petal[SegmentCount];

            double petalSize = getPetalSize(size);

            for (int i = 0; i < SegmentCount; i++)
            {
                Petals[i] = new Petal(petalSize);
                MovePetal(Petals[i], size, i);
                C.Children.Add(Petals[i].C);
            }
            
        }

        public void UpdateFlower()
        {
            for (int i = 0; i < SegmentCount; i++)
            {
                if (i == SelectedPetal)
                {
                    Petals[i].Activated = true;
                } 
                else
                {
                    Petals[i].Activated = false;
                    Petals[i].HandleButtonRelease();
                }
                Petals[i].UpdatePetal();
            }
        }

        public void UpdateSize(double s)
        {
            double petalSize = getPetalSize(s);
            for (int i = 0; i < SegmentCount; i++)
            {
                MovePetal(Petals[i], s, i);
                Petals[i].UpdateSize(petalSize);
            }
        }

        public void MovePetal(Petal p, double s, int i)
        {
            double center = s / 2;
            
            double radius = getRadius(s);
            double petalSize = getPetalSize(s);

            double angle = 2 * Math.PI * i / SegmentCount;
                // Place the petal so its center lies on the circle
            int x = (int)(center + radius * Math.Sin(angle) - petalSize / 2);
            int y = (int)(center + radius * -1 * Math.Cos(angle) - petalSize / 2);
            Canvas.SetLeft(Petals[i].C, x);
            Canvas.SetTop(Petals[i].C, y);
                Petals[i].UpdateSize(petalSize);
        }

        public double getPetalSize(double x)
        {
            return x / 4.5;
        }

        public double getRadius(double x)
        {
            return x / 2 / 1.5;
        }

        internal void HandleButtonPress(XInputButton b)
        {
            Petals[SelectedPetal].HandleButtonPress(b);
        }

        internal void HandleButtonRelease(XInputButton b)
        {
            Petals[SelectedPetal].HandleButtonRelease(b);
        }
    }
}
