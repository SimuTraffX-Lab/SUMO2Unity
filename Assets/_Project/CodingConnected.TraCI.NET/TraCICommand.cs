namespace CodingConnected.TraCI.NET
{
    internal class TraCICommand
    {
        public byte Identifier { get; set; }
        public byte[] Contents { get; set; }
    }
}