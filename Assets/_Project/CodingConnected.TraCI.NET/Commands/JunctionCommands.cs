using System.Collections.Generic;
using CodingConnected.TraCI.NET.Helpers;
using CodingConnected.TraCI.NET.Types;

namespace CodingConnected.TraCI.NET.Commands
{
	public class JunctionCommands : TraCIContextSubscribableCommands
	{
        #region Protected Override Methods
        protected override byte ContextSubscribeCommand => TraCIConstants.CMD_SUBSCRIBE_JUNCTION_CONTEXT;

        #endregion Protected Override Methods

        #region Public Methods

        /// <summary>
        /// Returns a list of ids of all junctions within the scenario 
        /// </summary>
        /// <returns></returns>
        public TraCIResponse<List<string>> GetIdList()
		{
			return
				TraCICommandHelper.ExecuteGetCommand<List<string>>(
					Client,
					"ignored",
					TraCIConstants.CMD_GET_JUNCTION_VARIABLE,
					TraCIConstants.ID_LIST);
		}

        /// <summary>
        /// Returns the number of junctions within the scenario
        /// </summary>
        /// <returns></returns>
		public TraCIResponse<int> GetIdCount()
		{
			return
				TraCICommandHelper.ExecuteGetCommand<int>(
					Client,
					"ignored",
					TraCIConstants.CMD_GET_JUNCTION_VARIABLE,
					TraCIConstants.ID_COUNT);
		}

        /// <summary>
        /// Returns the position of the named junction [m,m]
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<Position2D> GetPosition(string id)
		{
			return
				TraCICommandHelper.ExecuteGetCommand<Position2D>(
					Client,
					id,
					TraCIConstants.CMD_GET_JUNCTION_VARIABLE,
					TraCIConstants.VAR_POSITION);
		}

        /// <summary>
        /// Returns the shape (list of 2D-positions) of the named junction
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<Polygon> GetShape(string id)
		{
			return
				TraCICommandHelper.ExecuteGetCommand<Polygon>(
				Client,
				id,
				TraCIConstants.CMD_GET_JUNCTION_VARIABLE,
				TraCIConstants.VAR_SHAPE);
		}


        public void Subscribe(string objectId, int beginTime, int endTime, List<byte> ListOfVariablesToSubsribeTo)
        {
            TraCICommandHelper.ExecuteSubscribeCommand(
                Client,
                beginTime,
                endTime,
                objectId,
                TraCIConstants.CMD_SUBSCRIBE_JUNCTION_VARIABLE,
                ListOfVariablesToSubsribeTo);
        }
        #endregion // Public Methods

        #region Constructor

        public JunctionCommands(TraCIClient client) : base(client)
		{
		}

		#endregion // Constructor
	}
}

