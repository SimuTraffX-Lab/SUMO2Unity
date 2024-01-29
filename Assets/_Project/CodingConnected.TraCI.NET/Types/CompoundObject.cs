using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingConnected.TraCI.NET.Types
{
    public class CompoundObject : ComposedTypeBase
    {
        public List<ComposedTypeBase> Value { get; set; }

        public CompoundObject()
        {
            Value = new List<ComposedTypeBase>();
        }
    }
}
