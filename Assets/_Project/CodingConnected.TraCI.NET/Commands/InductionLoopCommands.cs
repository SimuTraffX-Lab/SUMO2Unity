using System;
using System.Collections.Generic;
using CodingConnected.TraCI.NET.Helpers;

namespace CodingConnected.TraCI.NET.Commands
{
	public class InductionLoopCommands : TraCIContextSubscribableCommands
	{
        #region Protected Override Methods
        protected override byte ContextSubscribeCommand => TraCIConstants.CMD_SUBSCRIBE_INDUCTIONLOOP_CONTEXT;

        #endregion Protected Override Methods

        #region Public Methods

        /// <summary>
        /// Returns a list of all objects in the network.
        /// </summary>
        /// <returns></returns>
        public TraCIResponse<List<string>> GetIdList()
		{
			return 
				TraCICommandHelper.ExecuteGetCommand<List<string>>(
					Client, 
					"ignored", 
					TraCIConstants.CMD_GET_INDUCTIONLOOP_VARIABLE,
					TraCIConstants.ID_LIST);
		}

        /// <summary>
        /// Returns the number of currently loaded objects.
        /// </summary>
        /// <returns></returns>
		public TraCIResponse<int> GetIdCount()
		{
			return 
				TraCICommandHelper.ExecuteGetCommand<int>(
					Client, 
					"ignored", 
					TraCIConstants.CMD_GET_INDUCTIONLOOP_VARIABLE,
					TraCIConstants.ID_COUNT);
		}

        /// <summary>
        /// Returns the position measured from the beginning of the lane in meters.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<double> GetPosition(string id)
		{
			return 
				TraCICommandHelper.ExecuteGetCommand<double>(
					Client, 
					id, 
					TraCIConstants.CMD_GET_INDUCTIONLOOP_VARIABLE,
					TraCIConstants.VAR_POSITION);
		}

        /// <summary>
        /// Returns the id of the lane the loop is on.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<string> GetLaneId(string id)
		{
			return 
				TraCICommandHelper.ExecuteGetCommand<string>(
					Client, 
					id, 
					TraCIConstants.CMD_GET_INDUCTIONLOOP_VARIABLE,
					TraCIConstants.VAR_LANE_ID);
		}

        /// <summary>
        /// Returns the number of vehicles that were on the named induction loop within the last simulation step.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<int> GetLastStepVehicleNumber(string id)
		{
			return 
				TraCICommandHelper.ExecuteGetCommand<int>(
					Client, 
					id, 
					TraCIConstants.CMD_GET_INDUCTIONLOOP_VARIABLE,
					TraCIConstants.LAST_STEP_VEHICLE_NUMBER);
		}

        /// <summary>
        /// Returns the mean speed in m/s of vehicles that were on the named induction loop within the last simulation step.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<double> GetLastStepMeanSpeed(string id)
		{
			return 
				TraCICommandHelper.ExecuteGetCommand<double>(
					Client, 
					id, 
					TraCIConstants.CMD_GET_INDUCTIONLOOP_VARIABLE,
					TraCIConstants.LAST_STEP_MEAN_SPEED);
		}

        /// <summary>
        /// Returns the list of ids of vehicles that were on the named induction loop in the last simulation step.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<List<string>> GetLastStepVehicleIds(string id)
		{
			return 
				TraCICommandHelper.ExecuteGetCommand<List<string>>(
					Client, 
					id, 
					TraCIConstants.CMD_GET_INDUCTIONLOOP_VARIABLE,
					TraCIConstants.LAST_STEP_VEHICLE_ID_LIST);
		}

        /// <summary>
        /// Returns the percentage of time the detector was occupied by a vehicle.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<double> GetLastStepOccupancy(string id)
		{
			return 
				TraCICommandHelper.ExecuteGetCommand<double>(
					Client, 
					id, 
					TraCIConstants.CMD_GET_INDUCTIONLOOP_VARIABLE,
					TraCIConstants.LAST_STEP_OCCUPANCY);
		}

        /// <summary>
        /// Returns the mean length in m of vehicles which were on the detector in the last step.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<double> GetLastStepMeanLength(string id)
		{
			return 
				TraCICommandHelper.ExecuteGetCommand<double>(
					Client, 
					id, 
					TraCIConstants.CMD_GET_INDUCTIONLOOP_VARIABLE,
					TraCIConstants.LAST_STEP_LENGTH);
		}

        /// <summary>
        /// Returns the time in s since last detection.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<double> GetTimeSinceDetection(string id)
		{
			return 
				TraCICommandHelper.ExecuteGetCommand<double>(
					Client, 
					id, 
					TraCIConstants.CMD_GET_INDUCTIONLOOP_VARIABLE,
					TraCIConstants.LAST_STEP_TIME_SINCE_DETECTION);
		}

        /// <summary>
        /// Returns a complex structure containing several information about vehicles which passed the detector.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<List<object>> GetVehicleData(string id)
		{
			// TODO: implement reading this data (http://sumo.dlr.de/wiki/TraCI/Induction_Loop_Value_Retrieval#Response_to_.22last_step.27s_vehicle_data.22_.280x17.29)
			throw new NotSupportedException("This method return complex data; TODO");
		}

        public void Subscribe(string objectId, int beginTime, int endTime, List<byte> ListOfVariablesToSubsribeTo)
        {
            TraCICommandHelper.ExecuteSubscribeCommand(
                Client,
                beginTime,
                endTime,
                objectId,
                TraCIConstants.CMD_SUBSCRIBE_INDUCTIONLOOP_VARIABLE,
                ListOfVariablesToSubsribeTo);
        }
        #endregion // Public Methods

        #region Constructor

        public InductionLoopCommands(TraCIClient client) : base(client)
		{
		}

		#endregion // Constructor
	}
}