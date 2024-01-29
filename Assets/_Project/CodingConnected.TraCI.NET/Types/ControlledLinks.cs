using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingConnected.TraCI.NET.Types
{
    public class ControlledLinks: ComposedTypeBase
    {
        public int NumberOfSignals { get; set; }
        public List<List<string>> Links { get; set; }

        public ControlledLinks()
        {
            Links = new List<List<string>>();
        }
    }
}
