using System;
using System.Collections.Generic;
using CodingConnected.TraCI.NET.Helpers;
using CodingConnected.TraCI.NET.Types;

namespace CodingConnected.TraCI.NET.Commands
{
	public class LaneCommands : TraCIContextSubscribableCommands
	{
        #region Protected Override Methods
        protected override byte ContextSubscribeCommand => TraCIConstants.CMD_SUBSCRIBE_LANE_CONTEXT;

        #endregion Protected Override Methods

        #region Public Methods

        /// <summary>
        /// Returns a list of ids of all lanes within the scenario (the given Lane ID is ignored)
        /// </summary>
        /// <returns></returns>
        public TraCIResponse<List<string>> GetIdList()
		{
			return 
				TraCICommandHelper.ExecuteGetCommand<List<string>>(
					Client, 
					"ignored", 
					TraCIConstants.CMD_GET_LANE_VARIABLE,
					TraCIConstants.ID_LIST);
		}

        /// <summary>
        /// Returns the number of lanes within the scenario (the given Lane ID is ignored)
        /// </summary>
        /// <returns></returns>
		public TraCIResponse<int> GetIdCount()
		{
			return 
				TraCICommandHelper.ExecuteGetCommand<int>(
					Client, 
					"ignored", 
					TraCIConstants.CMD_GET_LANE_VARIABLE,
					TraCIConstants.ID_COUNT);
		}

        /// <summary>
        /// Returns the number of links outgoing from this lane [#]
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<byte> GetLinkNumber(string id)
		{
			return 
				TraCICommandHelper.ExecuteGetCommand<byte>(
					Client, 
					id, 
					TraCIConstants.CMD_GET_LANE_VARIABLE,
					TraCIConstants.LANE_LINK_NUMBER);
		}

        /// <summary>
        /// Returns the id of the edge this lane belongs to
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<string> GetEdgeId(string id)
		{
			return 
				TraCICommandHelper.ExecuteGetCommand<string>(
					Client, 
					id, 
					TraCIConstants.CMD_GET_LANE_VARIABLE,
					TraCIConstants.LANE_EDGE_ID);
		}

        /// <summary>
        /// Returns descriptions of the links outgoing from this lane [m]
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<CompoundObject> GetLinks(string id)
		{
            //TODO: parse the result into a usable format
			return 
				TraCICommandHelper.ExecuteGetCommand<CompoundObject>(
					Client, 
					id, 
					TraCIConstants.CMD_GET_LANE_VARIABLE,
					TraCIConstants.LANE_LINKS);
		}

        /// <summary>
        /// Returns the mml-definitions of vehicle classes allowed on this lane
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<List<string>> GetAllowed(string id)
		{
			return 
				TraCICommandHelper.ExecuteGetCommand<List<string>>(
					Client, 
					id, 
					TraCIConstants.CMD_GET_LANE_VARIABLE,
					TraCIConstants.LANE_ALLOWED);
		}

        /// <summary>
        /// Returns the mml-definitions of vehicle classes not allowed on this lane
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<List<string>> GetDisallowed(string id)
		{
			return 
				TraCICommandHelper.ExecuteGetCommand<List<string>>(
					Client, 
					id, 
					TraCIConstants.CMD_GET_LANE_VARIABLE,
					TraCIConstants.LANE_DISALLOWED);
		}

        /// <summary>
        /// Returns the length of the named lane [m]
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<double> GetLength(string id)
		{
			return 
				TraCICommandHelper.ExecuteGetCommand<double>(
					Client, 
					id, 
					TraCIConstants.CMD_GET_LANE_VARIABLE,
					TraCIConstants.VAR_LENGTH);
		}

        /// <summary>
        /// Returns the maximum speed allowed on this lane [m/s]
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<double> GetMaxSpeed(string id)
		{
			return 
				TraCICommandHelper.ExecuteGetCommand<double>(
					Client, 
					id, 
					TraCIConstants.CMD_GET_LANE_VARIABLE,
					TraCIConstants.VAR_MAXSPEED);
		}

        /// <summary>
        /// Returns this lane's shape
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<Polygon> GetShape(string id)
		{
            return 
				TraCICommandHelper.ExecuteGetCommand<Polygon>(
					Client, 
					id, 
					TraCIConstants.CMD_GET_LANE_VARIABLE,
					TraCIConstants.VAR_SHAPE);
		}

        /// <summary>
        /// Returns the width of the named lane [m]
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<double> GetWidth(string id)
		{
			return 
				TraCICommandHelper.ExecuteGetCommand<double>(
					Client, 
					id, 
					TraCIConstants.CMD_GET_LANE_VARIABLE,
					TraCIConstants.VAR_WIDTH);
		}

        /// <summary>
        /// Sum of CO2 emissions on this lane in mg during this time step.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<double> GetCO2Emission(string id)
		{
			return 
				TraCICommandHelper.ExecuteGetCommand<double>(
					Client, 
					id, 
					TraCIConstants.CMD_GET_LANE_VARIABLE,
					TraCIConstants.VAR_CO2EMISSION);
		}

        /// <summary>
        /// Sum of CO emissions on this lane in mg during this time step.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<double> GetCOEmission(string id)
		{
			return 
				TraCICommandHelper.ExecuteGetCommand<double>(
					Client, 
					id, 
					TraCIConstants.CMD_GET_LANE_VARIABLE,
					TraCIConstants.VAR_COEMISSION);
		}

        /// <summary>
        /// Sum of HC emissions on this lane in mg during this time step.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<double> GetHCEmission(string id)
		{
			return 
				TraCICommandHelper.ExecuteGetCommand<double>(
					Client, 
					id, 
					TraCIConstants.CMD_GET_LANE_VARIABLE,
					TraCIConstants.VAR_HCEMISSION);
		}

        /// <summary>
        /// Sum of PMx emissions on this lane in mg during this time step.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<double> GetPMxEmission(string id)
		{
			return 
				TraCICommandHelper.ExecuteGetCommand<double>(
					Client, 
					id, 
					TraCIConstants.CMD_GET_LANE_VARIABLE,
					TraCIConstants.VAR_PMXEMISSION);
		}

        /// <summary>
        /// Sum of NOx emissions on this lane in mg during this time step.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<double> GetNOxEmission(string id)
		{
			return 
				TraCICommandHelper.ExecuteGetCommand<double>(
					Client, 
					id, 
					TraCIConstants.CMD_GET_LANE_VARIABLE,
					TraCIConstants.VAR_NOXEMISSION);
		}

        /// <summary>
        /// Sum of fuel consumption on this lane in ml during this time step.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<double> GetFuelConsumption(string id)
		{
			return 
				TraCICommandHelper.ExecuteGetCommand<double>(
					Client, 
					id, 
					TraCIConstants.CMD_GET_LANE_VARIABLE,
					TraCIConstants.VAR_FUELCONSUMPTION);
		}

        /// <summary>
        /// Returns the list of foe lanes. There are two modes for calling this method. If toLane is a normal road
        /// lane that is reachable from the laneID argument, the list contains all lanes that are the origin of a 
        /// connection with right-of-way over the connection between laneID and toLane. If toLane is empty and laneID
        /// is an internal lane, the list contains all internal lanes that intersect with laneID.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="toLane"></param>
        /// <returns></returns>
        public TraCIResponse<object> GetFoes(string id, string toLane)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sum of noise generated on this lane in dBA.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<double> GetNoiseEmission(string id)
		{
			return 
				TraCICommandHelper.ExecuteGetCommand<double>(
					Client, 
					id, 
					TraCIConstants.CMD_GET_LANE_VARIABLE,
					TraCIConstants.VAR_NOISEEMISSION);
		}

        /// <summary>
        /// Sum of electricity consumption on this lane in kWh during this time step.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<double> GetElectricityConsumption(string id)
		{
			return 
				TraCICommandHelper.ExecuteGetCommand<double>(
					Client, 
					id, 
					TraCIConstants.CMD_GET_LANE_VARIABLE,
					TraCIConstants.VAR_ELECTRICITYCONSUMPTION);
		}

        /// <summary>
        /// The number of vehicles on this lane within the last time step.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<int> GetLastStepVehicleNumber(string id)
		{
			return 
				TraCICommandHelper.ExecuteGetCommand<int>(
					Client, 
					id, 
					TraCIConstants.CMD_GET_LANE_VARIABLE,
					TraCIConstants.LAST_STEP_VEHICLE_NUMBER);
		}

        /// <summary>
        /// Returns the mean speed of vehicles that were on this lane within the last simulation step [m/s]
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<double> GetLastStepMeanSpeed(string id)
		{
			return 
				TraCICommandHelper.ExecuteGetCommand<double>(
					Client, 
					id, 
					TraCIConstants.CMD_GET_LANE_VARIABLE,
					TraCIConstants.LAST_STEP_MEAN_SPEED);
		}

        /// <summary>
        /// Returns the list of ids of vehicles that were on this lane in the last simulation step
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<List<string>> GetLastStepVehicleIds(string id)
		{
			return 
				TraCICommandHelper.ExecuteGetCommand<List<string>>(
					Client, 
					id, 
					TraCIConstants.CMD_GET_LANE_VARIABLE,
					TraCIConstants.LAST_STEP_VEHICLE_ID_LIST);
		}

        /// <summary>
        /// Returns the total lengths of vehicles on this lane during the last simulation step divided by the length of this lane
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<double> GetLastStepOccupancy(string id)
		{
			return 
				TraCICommandHelper.ExecuteGetCommand<double>(
					Client, 
					id, 
					TraCIConstants.CMD_GET_LANE_VARIABLE,
					TraCIConstants.LAST_STEP_OCCUPANCY);
		}

        /// <summary>
        /// The mean length of vehicles which were on this lane in the last step [m]
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<double> GetLastStepLength(string id)
		{
			return 
				TraCICommandHelper.ExecuteGetCommand<double>(
					Client, 
					id, 
					TraCIConstants.CMD_GET_LANE_VARIABLE,
					TraCIConstants.LAST_STEP_LENGTH);
		}

        /// <summary>
        /// Returns the waiting time for all vehicles on the lane [s]
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<double> GetWaitingTime(string id)
		{
			return 
				TraCICommandHelper.ExecuteGetCommand<double>(
					Client, 
					id, 
					TraCIConstants.CMD_GET_LANE_VARIABLE,
					TraCIConstants.VAR_WAITING_TIME);
		}

        /// <summary>
        /// Returns the estimated travel time for the last time step on the given lane [s]
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<double> GetTravelTime(string id)
		{
			return 
				TraCICommandHelper.ExecuteGetCommand<double>(
					Client, 
					id, 
					TraCIConstants.CMD_GET_LANE_VARIABLE,
					TraCIConstants.VAR_CURRENT_TRAVELTIME);
		}

        /// <summary>
        /// Returns the total number of halting vehicles for the last time step on the given lane.
        /// A speed of less than 0.1 m/s is considered a halt.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TraCIResponse<int> GetLastStepHaltingNumber(string id)
		{
			return 
				TraCICommandHelper.ExecuteGetCommand<int>(
					Client, 
					id, 
					TraCIConstants.CMD_GET_LANE_VARIABLE,
					TraCIConstants.LAST_STEP_VEHICLE_HALTING_NUMBER);
		}

        /// <summary>
        /// Sets a list of allowed vehicle classes.
        /// </summary>
        public TraCIResponse<object> SetAllowed(string laneId, List<string> allowedVehicleClasses)
        {
            return TraCICommandHelper.ExecuteSetCommand<object, List<string>>(
                Client,
                laneId,
                TraCIConstants.CMD_SET_LANE_VARIABLE,
                TraCIConstants.LANE_ALLOWED,
                allowedVehicleClasses);
        }

        /// <summary>
        /// Sets a list of disallowed vehicle classes.
        /// </summary>
        public TraCIResponse<object> SetDisallowed(string laneId, List<string> disallowedVehicleClasses)
        {
            return TraCICommandHelper.ExecuteSetCommand<object, List<string>>(
                Client,
                laneId,
                TraCIConstants.CMD_SET_LANE_VARIABLE,
                TraCIConstants.LANE_DISALLOWED,
                disallowedVehicleClasses);
        }

        /// <summary>
        /// Sets the length of the lane in m
        /// </summary>
        public TraCIResponse<object> SetLength(string laneId, double length)
        {
            return TraCICommandHelper.ExecuteSetCommand<object, double>(
                Client,
                laneId,
                TraCIConstants.CMD_SET_LANE_VARIABLE,
                TraCIConstants.VAR_LENGTH,
                length);
        }

        /// <summary>
        /// Sets a new maximum allowed speed on the lane in m/s.
        /// </summary>
        public TraCIResponse<object> SetMaxSpeed(string laneId, double maxSpeed)
        {
            return TraCICommandHelper.ExecuteSetCommand<object, double>(
                Client,
                laneId,
                TraCIConstants.CMD_SET_LANE_VARIABLE,
                TraCIConstants.VAR_MAXSPEED,
                maxSpeed);
        }



        public void Subscribe(string objectId, int beginTime, int endTime, List<byte> ListOfVariablesToSubsribeTo)
        {
            TraCICommandHelper.ExecuteSubscribeCommand(
                Client,
                beginTime,
                endTime,
                objectId,
                TraCIConstants.CMD_SUBSCRIBE_LANE_VARIABLE,
                ListOfVariablesToSubsribeTo);
        }
        #endregion // Public Methods

        #region Constructor

        public LaneCommands(TraCIClient client) : base(client)
		{
		}

		#endregion // Constructor
	}
}