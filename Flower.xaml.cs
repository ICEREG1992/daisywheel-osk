// Flower.xaml.cs
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using XInputium.XInput;

namespace daisywheel_osk
{
    public class Flower
    {
        public Canvas C { get; set; }
        public int SegmentCount { get; set; }
        public int? SelectedPetal {  get; set; }
        private Petal[] Petals { get; set; }
        private double FlowerSize { get; set; }
        private double PetalSize { get; set; }   

        public LayoutAlphabet Alphabet { get; set; }

        public Flower(double size, LayoutAlphabet a)
        {
            Alphabet = a;
            SegmentCount = Alphabet.NumPetals;
            C = new Canvas();

            Petals = new Petal[SegmentCount];
            FlowerSize = size;
            PetalSize = getPetalSize(size);

            for (int i = 0; i < SegmentCount; i++)
            {
                LayoutPetal chars = Alphabet.GetPetal(i);
                Petals[i] = new Petal(PetalSize, chars);
                MovePetal(Petals[i], i);
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
            FlowerSize = s;
            PetalSize = getPetalSize(FlowerSize);
            for (int i = 0; i < SegmentCount; i++)
            {
                MovePetal(Petals[i], i);
                Petals[i].UpdateSize(PetalSize);
            }
        }

        public void UpdateAlphabet()
        {
            C.Children.Clear();
            SegmentCount = Alphabet.NumPetals;
            Petals = new Petal[SegmentCount];
            for (int i = 0; i < SegmentCount; i++)
            {
                LayoutPetal chars = Alphabet.GetPetal(i);
                Petals[i] = new Petal(PetalSize, chars);
                MovePetal(Petals[i], i);
                C.Children.Add(Petals[i].C);
            }
            UpdateFlower();
        }

        public void MovePetal(Petal p, int i)
        {
            double center = FlowerSize / 2;
            
            double radius = getRadius(FlowerSize);

            double angle = 2 * Math.PI * i / SegmentCount;
                // Place the petal so its center lies on the circle
            int x = (int)(center + radius * Math.Sin(angle) - PetalSize / 2);
            int y = (int)(center + radius * -1 * Math.Cos(angle) - PetalSize / 2);
            Canvas.SetLeft(Petals[i].C, x);
            Canvas.SetTop(Petals[i].C, y);
            Petals[i].UpdateSize(PetalSize);
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
            if ( SelectedPetal != null)
            {
                Petals[(int)SelectedPetal].HandleButtonPress(b);
            }
        }

        internal void HandleButtonRelease(XInputButton b)
        {
            if (SelectedPetal != null)
            {
                Petals[(int)SelectedPetal].HandleButtonRelease(b);
            }
        }
    }
}
