using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryZero.Interfaces
{
    public interface IGrid2D<T>
    {
        int Width { get; }
        int Height { get; }
        bool IsReadOnly { get; }
        T this[int x, int y] { get; set; }
    }
}
