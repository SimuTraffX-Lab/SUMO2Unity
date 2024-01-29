using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingConnected.TraCI.NET
{
    public abstract class TraCISubscriptionResponse
    {
        public byte ResponseCode { get; }

        public int VariableCount { get; }
        public string ObjectId { get; }

        public abstract IEnumerable<object> Responses { get; }

        public TraCISubscriptionResponse(string objectId, int variableCount, byte responseCode )
        {
            ObjectId = objectId;
            ResponseCode = responseCode;
            VariableCount = variableCount;
        }
    }
}
