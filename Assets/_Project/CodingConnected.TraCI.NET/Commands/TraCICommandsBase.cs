using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingConnected.TraCI.NET.Commands
{
	public abstract class TraCICommandsBase
	{
		#region Fields

		protected readonly TraCIClient Client;

		#endregion // Fields

		#region Constructor

		protected TraCICommandsBase(TraCIClient client)
		{
			Client = client;
		}

        #endregion // Constructor

        #region Abstract Methods

        #endregion

        
    }
}
