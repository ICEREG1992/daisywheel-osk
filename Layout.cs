using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace daisywheel_osk
{
    public class Layout
    {
        public string Name { get; set; }
        private List<LayoutAlphabet> _alphabets = new();
        public List<LayoutAlphabet> Alphabets
        {
            get => _alphabets;
            set
            {
                _alphabets = value;
                NumAlphabets = value.Count;
            }
        }
        public int NumAlphabets { get; private set; }
        public Layout(string name, List<LayoutAlphabet> alphabets)
        {
            Name = name;
            Alphabets = alphabets;
        }

        public LayoutAlphabet GetAlphabet(int index)
        {
            return Alphabets[index];
        }

    }

    public class LayoutAlphabet
    {
        private List<LayoutPetal> _petals = new();
        public List<LayoutPetal> Petals
        {
            get => _petals;
            set
            {
                _petals = value;
                NumPetals = value.Count;
            }
        }
        public int NumPetals { get; private set; }

        public LayoutAlphabet(List<LayoutPetal> petals)
        {
            Petals = petals;
        }

        public LayoutPetal GetPetal(int index)
        {
            return Petals[index];
        }

        public static LayoutAlphabet blankAlphabet = new LayoutAlphabet([]);
    }

    public class LayoutPetal
    {
        public string[] Keys { get; set; }

        public LayoutPetal(string a, string b, string c, string d)
        {
            Keys = [a, b, c, d];
        }

        public string GetChar(int index)
        {
            return Keys[index];
        }
    }
}
