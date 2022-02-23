using FactoryZero.Interfaces;
using System;

namespace FactoryZero.Util
{
    public class FloatGrid2D : IGrid2D<float>
    {
        float[] values;
        int width, height;

        public FloatGrid2D(int width, int height)
        {
            this.width = width;
            this.height = height;

            values = new float[width * height];
        }

        // Converts a 2D index to 1D index for `values` array
        int GetIndex(int x, int y)
        {
            return x + y * width;
        }

        // A wrapper around GetIndex which performs checks and throws exceptions.
        int TranslateIndex(int x, int y)
        {

            if (x < 0 || y < 0 || x >= width || y >= width)
            {
                throw new IndexOutOfRangeException($"Index [{x}, {y}] does not fall within the expected range of [0..{width - 1}, 0..{height - 1}]");
            }

            int index1d = GetIndex(x, y);
            if (values.Length <= index1d)
            {
                throw new IndexOutOfRangeException($"Index [{x}, {y}] appears to be in the expected range [0..{width - 1}, 0..{height - 1}], but internally the translated index [{index1d}] is out of bounds [0..{values.Length - 1}]");
            }

            return index1d;
        }

        public float this[int x, int y]
        {
            get
            {
                int index1d = TranslateIndex(x, y);
                return values[index1d];
            }
            set
            {
                int index1d = TranslateIndex(x, y);
                values[index1d] = value;
            }
        }

        public int Width => width;

        public int Height => height;

        public bool IsReadOnly => false;
    }
}
