using System.Collections.Generic;

namespace CodingConnected.TraCI.NET.Types
{
    /// <summary>
    /// Please refer to  https://sumo.dlr.de/wiki/TraCI/Object_Variable_Subscription
    /// </summary>
    public class VariableSubscriptionEventArgs : SubscriptionEventArgs
    {
        public IReadOnlyDictionary<byte, IResponseInfo> ResponseByVariableCode { get; }

        public VariableSubscriptionEventArgs(
            string objectId, 
            int variableCount,
            IReadOnlyDictionary<byte, IResponseInfo> responseByVariableCode) : base(objectId, variableCount)
        {
            ResponseByVariableCode = responseByVariableCode;
        }
    }
}
