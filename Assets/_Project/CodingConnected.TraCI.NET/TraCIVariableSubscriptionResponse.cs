using System.Collections.Generic;

namespace CodingConnected.TraCI.NET
{
    /// <summary>
    /// Please refer to  https://sumo.dlr.de/wiki/TraCI/Object_Variable_Subscription
    /// </summary>
    public class TraCIVariableSubscriptionResponse : TraCISubscriptionResponse
    {
        internal Dictionary<byte, IResponseInfo> responseByVariableCode { get; }

        public TraCIVariableSubscriptionResponse(string objectId, int variableCount, byte responseCode)
            : base(objectId, variableCount, responseCode) => responseByVariableCode = new Dictionary<byte, IResponseInfo>();

        public IReadOnlyDictionary<byte, IResponseInfo> ResponseByVariableCode { get => responseByVariableCode; }

        public override IEnumerable<object> Responses { get => ResponseByVariableCode.Values; }
    }
}
