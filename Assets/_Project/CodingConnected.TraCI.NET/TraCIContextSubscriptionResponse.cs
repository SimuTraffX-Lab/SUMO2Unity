using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingConnected.TraCI.NET
{
    public class TraCIContextSubscriptionResponse : TraCISubscriptionResponse
    {
        public override IEnumerable<object> Responses => VariableSubscriptionByObjectId.Values;

        internal Dictionary<string, TraCIVariableSubscriptionResponse> VariableSubscriptionByObjectId { get; }

        /// <summary>
        /// the type of objects in the addressed object's surrounding to ask values from. <para/>
        /// For more information please visit https://sumo.dlr.de/wiki/TraCI/Object_Context_Subscription
        /// </summary>
        internal byte ContextDomain { get; }

        /// <summary>
        /// The number of objects in range. (the objects are of type of the ContextDomain).
        /// </summary>
        internal int ObjectCount { get; }
        internal TraCIContextSubscriptionResponse(
            string objectId,
            byte responseCode,
            byte contextDomain,
            int variableCount,
            int objectCount
            )
            : base(objectId, variableCount, responseCode)
        {
            VariableSubscriptionByObjectId = new Dictionary<string, TraCIVariableSubscriptionResponse>();
            ContextDomain = contextDomain;
            ObjectCount = objectCount;
        }
    }
}
