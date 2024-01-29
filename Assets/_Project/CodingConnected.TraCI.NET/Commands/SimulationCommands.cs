using System.Collections.Generic;
using CodingConnected.TraCI.NET.Helpers;
using CodingConnected.TraCI.NET.Types;

namespace CodingConnected.TraCI.NET.Commands
{
	public class SimulationCommands : TraCICommandsBase
	{
        #region Public Methods

        /// <summary>
        /// Returns the current simulation time (in s)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TraCIResponse<int> GetCurrentTime (string id)
		{
			return
				TraCICommandHelper.ExecuteGetCommand<int>(
					Client,
					id,
					TraCIConstants.CMD_GET_SIM_VARIABLE,
					TraCIConstants.VAR_TIME_STEP);
		}

        /// <summary>
        /// Returns the current simulation time (in s)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TraCIResponse<double> GetTime(string id)
        {
            return
                TraCICommandHelper.ExecuteGetCommand<double>(
                    Client,
                    id,
                    TraCIConstants.CMD_GET_SIM_VARIABLE,
                    TraCIConstants.VAR_TIME);
        }
        /// <summary>
        /// Returns the current simulation time (in ms)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<int> GetLoadedNumber (string id)
		{
			return
				TraCICommandHelper.ExecuteGetCommand<int>(
					Client,
					id,
					TraCIConstants.CMD_GET_SIM_VARIABLE,
					TraCIConstants.VAR_LOADED_VEHICLES_NUMBER);
		}

        /// <summary>
        /// The number of vehicles which were loaded in this time step.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<List<string>> GetLoadedIDList (string id)
		{
			return
				TraCICommandHelper.ExecuteGetCommand<List<string>>(
					Client,
					id,
					TraCIConstants.CMD_GET_SIM_VARIABLE,
					TraCIConstants.VAR_LOADED_VEHICLES_IDS);
		}

        /// <summary>
        /// A list of ids of vehicles which were loaded in this time step.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<int> GetDepartedNumber (string id)
		{
			return
				TraCICommandHelper.ExecuteGetCommand<int>(
					Client,
					id,
					TraCIConstants.CMD_GET_SIM_VARIABLE,
					TraCIConstants.VAR_DEPARTED_VEHICLES_NUMBER);
		}

        /// <summary>
        /// The number of vehicles which departed (were inserted into the road network) in this time step.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<List<string>> GetDepartedIDList (string id)
		{
			return
				TraCICommandHelper.ExecuteGetCommand<List<string>>(
					Client,
					id,
					TraCIConstants.CMD_GET_SIM_VARIABLE,
					TraCIConstants.VAR_DEPARTED_VEHICLES_IDS);
		}

        /// <summary>
        /// 	The number of vehicles which ended to be teleported in this time step.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<int> GetStartingTeleportNumber (string id)
		{
			return
				TraCICommandHelper.ExecuteGetCommand<int>(
					Client,
					id,
					TraCIConstants.CMD_GET_SIM_VARIABLE,
					TraCIConstants.VAR_TELEPORT_STARTING_VEHICLES_NUMBER);
		}

        /// <summary>
        /// 	A list of ids of vehicles which started to teleport in this time step.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<List<string>> GetStartingTeleportIDList (string id)
		{
			return
				TraCICommandHelper.ExecuteGetCommand<List<string>>(
					Client,
					id,
					TraCIConstants.CMD_GET_SIM_VARIABLE,
					TraCIConstants.VAR_TELEPORT_STARTING_VEHICLES_IDS);
		}

        /// <summary>
        /// 	The number of vehicles which ended to be teleported in this time step.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<int> GetEndingTeleportNumber(string id)
		{
			return
				TraCICommandHelper.ExecuteGetCommand<int>(
					Client,
					id,
					TraCIConstants.CMD_GET_SIM_VARIABLE,
					TraCIConstants.VAR_TELEPORT_ENDING_VEHICLES_NUMBER);
		}

        /// <summary>
        /// 	A list of ids of vehicles which ended to be teleported in this time step.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<List<string>> GetEndingTeleportIDList (string id)
		{
			return
				TraCICommandHelper.ExecuteGetCommand<List<string>>(
					Client,
					id,
					TraCIConstants.CMD_GET_SIM_VARIABLE,
					TraCIConstants.VAR_TELEPORT_ENDING_VEHICLES_IDS);
		}

        /// <summary>
        /// 	The number of vehicles which arrived (have reached their destination and are removed from the road network) in this time step.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<int> GetArrivedNumber (string id)
		{
			return
				TraCICommandHelper.ExecuteGetCommand<int>(
					Client,
					id,
					TraCIConstants.CMD_GET_SIM_VARIABLE,
					TraCIConstants.VAR_ARRIVED_VEHICLES_NUMBER);
		}

        /// <summary>
        /// A list of ids of vehicles which arrived (have reached their destination and are removed from the road network) in this time step.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<List<string>> GetArrivedIDList (string id)
		{
			return
				TraCICommandHelper.ExecuteGetCommand<List<string>>(
					Client,
					id,
					TraCIConstants.CMD_GET_SIM_VARIABLE,
					TraCIConstants.VAR_ARRIVED_VEHICLES_IDS);
		}

        /// <summary>
        /// The lower left and the upper right corner of the bounding box of the simulation network.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<Polygon> GetNetBoundary (string id)
		{
			return
				TraCICommandHelper.ExecuteGetCommand<Polygon>(
					Client,
					id,
					TraCIConstants.CMD_GET_SIM_VARIABLE,
					TraCIConstants.VAR_NET_BOUNDING_BOX);
		}

        /// <summary>
        /// 	The number of vehicles which are in the net plus the ones still waiting to start. This number may be smaller than the actual number of vehicles still to come because of delayed route file parsing.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<int> GetMinExpectedNumber (string id)
		{
			return
				TraCICommandHelper.ExecuteGetCommand<int>(
					Client,
					id,
					TraCIConstants.CMD_GET_SIM_VARIABLE,
					TraCIConstants.VAR_MIN_EXPECTED_VEHICLES);
		}

        /// <summary>
        /// The number of vehicles that halted on a scheduled stop in this time step.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<int> GetStopStartingVehiclesNumber (string id)
		{
			return
				TraCICommandHelper.ExecuteGetCommand<int>(
					Client,
					id,
					TraCIConstants.CMD_GET_SIM_VARIABLE,
					TraCIConstants.VAR_STOP_STARTING_VEHICLES_NUMBER);
		}

        /// <summary>
        /// 	A list of ids of vehicles that halted on a scheduled stop in this time step.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<List<string>> GetStopStartingVehiclesIDList (string id)
		{
			return
				TraCICommandHelper.ExecuteGetCommand<List<string>>(
					Client,
					id,
					TraCIConstants.CMD_GET_SIM_VARIABLE,
					TraCIConstants.VAR_STOP_STARTING_VEHICLES_IDS);
		}

        /// <summary>
        /// 	The number of vehicles that begin to continue their journey, leaving a scheduled stop in this time step.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<int> GetStopEndingVehiclesNumber (string id)
		{
			return
				TraCICommandHelper.ExecuteGetCommand<int>(
					Client,
					id,
					TraCIConstants.CMD_GET_SIM_VARIABLE,
					TraCIConstants.VAR_STOP_ENDING_VEHICLES_NUMBER);
		}

        /// <summary>
        /// 	A list of ids of vehicles that begin to continue their journey, leaving a scheduled stop in this time step.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<List<string>> GetStopEndingVehiclesIDList (string id)
		{
			return
				TraCICommandHelper.ExecuteGetCommand<List<string>>(
					Client,
					id,
					TraCIConstants.CMD_GET_SIM_VARIABLE,
					TraCIConstants.VAR_STOP_ENDING_VEHICLES_IDS);
		}

        /// <summary>
        /// The number of vehicles that were involved in a collision in this time step.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<int> GetCollidingVehiclesNumber (string id)
		{
			return
				TraCICommandHelper.ExecuteGetCommand<int>(
					Client,
					id,
					TraCIConstants.CMD_GET_SIM_VARIABLE,
					TraCIConstants.VAR_COLLIDING_VEHICLES_NUMBER);
		}

        /// <summary>
        /// 	A list of ids of vehicles that were involved in a collision in this time step.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<List<string>> GetCollidingVehiclesIDList (string id)
		{
			return
				TraCICommandHelper.ExecuteGetCommand<List<string>>(
					Client,
					id,
					TraCIConstants.CMD_GET_SIM_VARIABLE,
					TraCIConstants.VAR_COLLIDING_VEHICLES_IDS);
		}

        /// <summary>
        /// The number of vehicles that had an emergency stop in this time step.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<int> GetEmergencyStoppingVehiclesNumber(string id)
        {
            return
                TraCICommandHelper.ExecuteGetCommand<int>(
                    Client,
                    id,
                    TraCIConstants.CMD_GET_SIM_VARIABLE,
                    TraCIConstants.VAR_EMERGENCYSTOPPING_VEHICLES_NUMBER);
        }

        /// <summary>
        /// 	A list of ids of vehicles that had an emergency stop in this time step.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<List<string>> GetEmergencyStoppingVehiclesIDList(string id)
        {
            return
                TraCICommandHelper.ExecuteGetCommand<List<string>>(
                    Client,
                    id,
                    TraCIConstants.CMD_GET_SIM_VARIABLE,
                    TraCIConstants.VAR_EMERGENCYSTOPPING_VEHICLES_IDS);
        }

        /// <summary>
        /// The number of vehicles that enter a parking position in this time step.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<int> GetParkingStartingVehiclesNumber (string id)
		{
			return
				TraCICommandHelper.ExecuteGetCommand<int>(
					Client,
					id,
					TraCIConstants.CMD_GET_SIM_VARIABLE,
					TraCIConstants.VAR_PARKING_STARTING_VEHICLES_NUMBER);
		}

        /// <summary>
        /// 	A list of ids of vehicles that enter a parking position in this time step.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<List<string>> GetParkingStartingVehiclesIDList (string id)
		{
			return
				TraCICommandHelper.ExecuteGetCommand<List<string>>(
					Client,
					id,
					TraCIConstants.CMD_GET_SIM_VARIABLE,
					TraCIConstants.VAR_PARKING_STARTING_VEHICLES_IDS);
		}

        /// <summary>
        /// 	The number of vehicles that begin to continue their journey, leaving a scheduled parking in this time step.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<int> GetParkingEndingVehiclesNumber (string id)
		{
			return
				TraCICommandHelper.ExecuteGetCommand<int>(
					Client,
					id,
					TraCIConstants.CMD_GET_SIM_VARIABLE,
					TraCIConstants.VAR_PARKING_ENDING_VEHICLES_NUMBER);
		}

        /// <summary>
        /// A list of ids of vehicles that begin to continue their journey, leaving a scheduled parking in this time step.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<List<string>> GetParkingEndingVehiclesIDList (string id)
		{
			return
				TraCICommandHelper.ExecuteGetCommand<List<string>>(
					Client,
					id,
					TraCIConstants.CMD_GET_SIM_VARIABLE,
					TraCIConstants.VAR_PARKING_ENDING_VEHICLES_IDS);
		}

        /// <summary>
        /// Get the total number of waiting persons at the named bus stop.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<int> GetBusStopWaiting (string id)
		{
			return
				TraCICommandHelper.ExecuteGetCommand<int>(
					Client,
					id,
					TraCIConstants.CMD_GET_SIM_VARIABLE,
					TraCIConstants.VAR_BUS_STOP_WAITING);
		}

        /// <summary>
        /// Returns the length of one simulation step in seconds.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<double> GetDeltaT (string id)
		{
			return
				TraCICommandHelper.ExecuteGetCommand<double>(
					Client,
					id,
					TraCIConstants.CMD_GET_SIM_VARIABLE,
					TraCIConstants.VAR_DELTA_T);
		}

        /// <summary>
        /// Returns the value for the given string parameter
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<string> GetParameter(string id)
		{
			return
				TraCICommandHelper.ExecuteGetCommand<string>(
					Client,
					id,
					TraCIConstants.CMD_GET_SIM_VARIABLE,
					TraCIConstants.VAR_PARAMETER);
		}

        // TODO: 'extended retrieval', see: http://sumo.dlr.de/wiki/TraCI/Simulation_Value_Retrieval

        /// <summary>
        /// Discards all loaded vehicles with a depart time below the current time step which could not be inserted yet. If the given routeID has non-zero length, only vehicles that have this routeID are discarded.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="routeId"></param>
        /// <returns></returns>
        public TraCIResponse<object> ClearPending(string id, string routeId)
        {
            return TraCICommandHelper.ExecuteSetCommand<object, string>(
                    Client,
                    id,
                    TraCIConstants.CMD_SET_SIM_VARIABLE,
                    TraCIConstants.CMD_CLEAR_PENDING_VEHICLES,
                    routeId
                    );
        }

        /// <summary>
        /// Saves current simulation state to the given filename.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public TraCIResponse<object> SaveState(string id, string filename)
        {
            return TraCICommandHelper.ExecuteSetCommand<object, string>(
                    Client,
                    id,
                    TraCIConstants.CMD_SET_SIM_VARIABLE,
                    TraCIConstants.CMD_SAVE_SIMSTATE,
                    filename
                    );
        }

        public void Subscribe(string objectId, int beginTime, int endTime, List<byte> ListOfVariablesToSubsribeTo)
        {
            TraCICommandHelper.ExecuteSubscribeCommand(
                Client,
                beginTime,
                endTime,
                objectId,
                TraCIConstants.CMD_SUBSCRIBE_SIM_VARIABLE,
                ListOfVariablesToSubsribeTo);
        }
        #endregion // Public Methods

        #region Constructor

        public SimulationCommands(TraCIClient client) : base(client)
		{
		}

		#endregion // Constructor
	}
}

