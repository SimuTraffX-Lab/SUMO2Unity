namespace CodingConnected.TraCI.NET
{
    internal class TraCIResult
    {
        public int Length { get; set; }
        public byte Identifier { get; set; }
        public byte[] Response { get; set; }
    }
}