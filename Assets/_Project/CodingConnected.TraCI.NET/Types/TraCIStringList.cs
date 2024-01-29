using System.Collections.Generic;


namespace CodingConnected.TraCI.NET.Types
{
    public class TraCIStringList : ComposedTypeBase
    {
        public List<string> Value { get; set; }

        public TraCIStringList()
        {
            Value = new List<string>();
        }

        public void Add(string item)
        {
            Value.Add(item);
        }
    }
}
