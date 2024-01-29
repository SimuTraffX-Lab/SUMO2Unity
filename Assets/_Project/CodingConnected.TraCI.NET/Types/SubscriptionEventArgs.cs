using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingConnected.TraCI.NET.Types
{
    public class SubscriptionEventArgs : EventArgs
    {
        public string ObjectId { get; }


        /// <summary>
        /// The number of returned variables that were subscribed.
        /// </summary>
        public int VariableCount { get;  }

        /// <summary>
        /// The responses must be cast to the right type in order to be used.
        /// </summary>
        public IEnumerable<object> Responses;

        public int Length
        {
            get
            {
                if (Responses != null)
                {
                    return Responses.Count();
                }
                else
                {
                    return 0;
                }
            }
        }
        public SubscriptionEventArgs(string objectId, int variableCount)
        {
            ObjectId = objectId;
            VariableCount = variableCount;
        }
    }
}
