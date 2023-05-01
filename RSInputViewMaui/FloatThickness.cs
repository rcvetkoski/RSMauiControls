using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSInputViewMaui
{
    public struct FloatThickness
    {
        public float Left { get; set; }
        public float Top { get; set; }
        public float Right { get; set; }
        public float Bottom { get; set; }

        public FloatThickness(float uniformSize)
        {
            Left = Top = Right = Bottom = uniformSize;
        }

        public FloatThickness(float horizontalSize, float verticalSize)
        {
            Left = Right = horizontalSize;
            Top = Bottom = verticalSize;
        }

        public FloatThickness(float left, float top, float right, float bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }
    }
}
