using System;

namespace CodingConnected.TraCI.NET
{
	public class TraCICommandException : Exception
	{
		public TraCICommandException(byte commandType, byte variableType, string message = null) : base(message)
		{
			CommandType = commandType;
			VariableType = variableType;
		}

		public byte CommandType { get; }
		public byte VariableType { get; }
	}
}