using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryZero.Interfaces
{
    interface IPotentiallyReadOnly
    {
        bool IsReadOnly { get; }
    }
}
