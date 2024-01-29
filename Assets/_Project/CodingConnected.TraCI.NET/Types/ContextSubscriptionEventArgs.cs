using System.Collections.Generic;

namespace CodingConnected.TraCI.NET.Types
{
    /// <summary>
    /// Please refer to  https://sumo.dlr.de/wiki/TraCI/Object_Context_Subscription
    /// </summary>
    public class ContextSubscriptionEventArgs : SubscriptionEventArgs
    {
        /// <summary>
        /// Get the variable subscription of the object by the id of the object.
        /// The object was inside the context range of the object this subscription happened under.
        /// </summary>
        public IReadOnlyDictionary<string, TraCIVariableSubscriptionResponse> VariableSubscriptionByObjectId { get; }

        /// <summary>
        /// the type of objects in the addressed object's surrounding to ask values from. <para/>
        /// For more information please visit https://sumo.dlr.de/wiki/TraCI/Object_Context_Subscription
        /// </summary>
        public byte ContextDomain { get; }

        /// <summary>
        /// The number of objects in range. (the objects are of type of the ContextDomain).
        /// </summary>
        public int ObjectCount { get; }

        public ContextSubscriptionEventArgs(
            string objectId,
            IReadOnlyDictionary<string, TraCIVariableSubscriptionResponse> variableSubscriptionResponseByObjectId,
            byte contextDomain,
            int variableCount,
            int objectCount) : base(objectId, variableCount)
        {
            VariableSubscriptionByObjectId = variableSubscriptionResponseByObjectId;

            ContextDomain = contextDomain;
            ObjectCount = objectCount;
        }
    }
}
