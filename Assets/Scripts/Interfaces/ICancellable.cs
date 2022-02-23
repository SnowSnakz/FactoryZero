using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryZero.Interfaces
{
    public interface ICancellable
    {
        bool Cancel { get; set; }
    }
}
