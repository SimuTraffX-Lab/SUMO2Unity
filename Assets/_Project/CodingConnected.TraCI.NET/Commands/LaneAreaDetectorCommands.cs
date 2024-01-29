using System;
using System.Collections.Generic;
using System.Linq;
using CodingConnected.TraCI.NET.Helpers;

namespace CodingConnected.TraCI.NET.Commands
{
	public class LaneAreaDetectorCommands : TraCICommandsBase
	{
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
					TraCIConstants.CMD_GET_LANEAREA_VARIABLE,
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
					TraCIConstants.CMD_GET_LANEAREA_VARIABLE,
					TraCIConstants.ID_COUNT);
		}

        /// <summary>
        /// Returns the starting position of the detector measured from the beginning of the lane in meters.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<double> GetPosition(string id)
		{
			return 
				TraCICommandHelper.ExecuteGetCommand<double>(
					Client, 
					id, 
					TraCIConstants.CMD_GET_LANEAREA_VARIABLE,
					TraCIConstants.VAR_POSITION);
		}

        /// <summary>
        /// Returns the length of the detector
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<double> GetLength(string id)
		{
			return 
				TraCICommandHelper.ExecuteGetCommand<double>(
					Client, 
					id, 
					TraCIConstants.CMD_GET_LANEAREA_VARIABLE,
					TraCIConstants.VAR_LENGTH);
		}

        /// <summary>
        /// Returns the id of the lane the detector is on.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<string> GetLaneId(string id)
		{
			return 
				TraCICommandHelper.ExecuteGetCommand<string>(
					Client, 
					id, 
					TraCIConstants.CMD_GET_LANEAREA_VARIABLE,
					TraCIConstants.VAR_LANE_ID);
		}

        /// <summary>
        /// Returns the number of vehicles that were on the named detector within the last simulation step.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TraCIResponse<int> GetLastStepVehicleNumber(string id)
		{
			return 
				TraCICommandHelper.ExecuteGetCommand<int>(
					Client, 
					id, 
					TraCIConstants.CMD_GET_LANEAREA_VARIABLE,
					TraCIConstants.LAST_STEP_VEHICLE_NUMBER);
		}

        /// <summary>
        /// Returns the current mean speed in m/s of vehicles that were on the named e2.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<double> GetLastStepMeanSpeed(string id)
		{
			return 
				TraCICommandHelper.ExecuteGetCommand<double>(
					Client, 
					id, 
					TraCIConstants.CMD_GET_LANEAREA_VARIABLE,
					TraCIConstants.LAST_STEP_MEAN_SPEED);
		}

        /// <summary>
        /// Returns the list of ids of vehicles that were on the named detector in the last simulation step.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<List<string>> GetLastStepVehicleIds(string id)
		{
			return 
				TraCICommandHelper.ExecuteGetCommand<List<string>>(
					Client, 
					id, 
					TraCIConstants.CMD_GET_LANEAREA_VARIABLE,
					TraCIConstants.LAST_STEP_VEHICLE_ID_LIST);
		}

        /// <summary>
        /// Returns the percentage of space the detector was occupied by a vehicle [%]
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TraCIResponse<double> GetLastStepOccupancy(string id)
		{
			return 
				TraCICommandHelper.ExecuteGetCommand<double>(
					Client, 
					id, 
					TraCIConstants.CMD_GET_LANEAREA_VARIABLE,
					TraCIConstants.LAST_STEP_OCCUPANCY);
		}

        /// <summary>
        /// Returns the jam length in meters within the last simulation step.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TraCIResponse<double> GetJamLengthMeters(string id)
		{
			return 
				TraCICommandHelper.ExecuteGetCommand<double>(
					Client, 
					id, 
					TraCIConstants.CMD_GET_LANEAREA_VARIABLE,
					TraCIConstants.JAM_LENGTH_METERS);
		}

        /// <summary>
        /// Returns the jam length in vehicles within the last simulation step.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<double> GetJamLengthVehicle(string id)
		{
			return 
				TraCICommandHelper.ExecuteGetCommand<double>(
					Client, 
					id, 
					TraCIConstants.CMD_GET_LANEAREA_VARIABLE,
					TraCIConstants.JAM_LENGTH_VEHICLE);
		}

        public void Subscribe(string objectId, int beginTime, int endTime, List<byte> ListOfVariablesToSubsribeTo)
        {
            TraCICommandHelper.ExecuteSubscribeCommand(
                Client,
                beginTime,
                endTime,
                objectId,
                TraCIConstants.CMD_SUBSCRIBE_LANEAREA_VARIABLE,
                ListOfVariablesToSubsribeTo);
        }
        #endregion // Public Methods

        #region Constructor

        public LaneAreaDetectorCommands(TraCIClient client) : base(client)
		{
		}

		#endregion // Constructor
	}
}