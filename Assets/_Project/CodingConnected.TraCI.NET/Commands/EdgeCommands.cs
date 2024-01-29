using System.Collections.Generic;
using CodingConnected.TraCI.NET.Helpers;
using CodingConnected.TraCI.NET.Types;

namespace CodingConnected.TraCI.NET.Commands
{
	public class EdgeCommands : TraCIContextSubscribableCommands
	{
        #region Protected Override Methods
        protected override byte ContextSubscribeCommand => TraCIConstants.CMD_SUBSCRIBE_EDGE_CONTEXT;
       
        #endregion Protected Override Methods

        #region Public Methods

        /// <summary>
        /// Returns a list of ids of all edges within the scenario
        /// </summary>
        /// <returns></returns>
        public TraCIResponse<List<string>> GetIdList ()
		{
			return
				TraCICommandHelper.ExecuteGetCommand<List<string>>(
					Client,
					"ignored",
					TraCIConstants.CMD_GET_EDGE_VARIABLE,
					TraCIConstants.ID_LIST);
		}

        /// <summary>
        /// Returns the number of edges within the scenario
        /// </summary>
        /// <returns></returns>
		public TraCIResponse<int> GetIdCount ()
		{
			return
				TraCICommandHelper.ExecuteGetCommand<int>(
					Client,
					"ignored",
					TraCIConstants.CMD_GET_EDGE_VARIABLE,
					TraCIConstants.ID_COUNT);
		}

        /// <summary>
        /// Returns the number of lanes for the given edge ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<int> GetLaneNumber (string id)
		{
			return
				TraCICommandHelper.ExecuteGetCommand<int>(
					Client,
					id,
					TraCIConstants.CMD_GET_EDGE_VARIABLE,
					TraCIConstants.VAR_LANE_INDEX);
		}

        /// <summary>
        /// Returns the street name for the given edge ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TraCIResponse<string> GetStreetName(string id)
        {
            return
                TraCICommandHelper.ExecuteGetCommand<string>(
                    Client,
                    id,
                    TraCIConstants.CMD_GET_EDGE_VARIABLE,
                    TraCIConstants.VAR_STREET_NAME);
        }

        /// <summary>
        /// Returns the current travel time (length/mean speed).
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<double> GetTraveltime (string id)
		{
			return
				TraCICommandHelper.ExecuteGetCommand<double>(
					Client,
					id,
					TraCIConstants.CMD_GET_EDGE_VARIABLE,
					TraCIConstants.VAR_CURRENT_TRAVELTIME);
		}

        /// <summary>
        /// Sum of CO2 emissions on this edge in mg during this time step.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<double> GetCO2Emission (string id)
		{
			return
				TraCICommandHelper.ExecuteGetCommand<double>(
					Client,
					id,
					TraCIConstants.CMD_GET_EDGE_VARIABLE,
					TraCIConstants.VAR_CO2EMISSION);
		}

        /// <summary>
        /// Sum of CO emissions on this edge in mg during this time step.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<double> GetCOEmission (string id)
		{
			return
				TraCICommandHelper.ExecuteGetCommand<double>(
					Client,
					id,
					TraCIConstants.CMD_GET_EDGE_VARIABLE,
					TraCIConstants.VAR_COEMISSION);
		}

        /// <summary>
        /// Sum of HC emissions on this edge in mg during this time step.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<double> GetHCEmission (string id)
		{
			return
				TraCICommandHelper.ExecuteGetCommand<double>(
					Client,
					id,
					TraCIConstants.CMD_GET_EDGE_VARIABLE,
					TraCIConstants.VAR_HCEMISSION);
		}

        /// <summary>
        /// Sum of PMx emissions on this edge in mg during this time step.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<double> GetPMxEmission (string id)
		{
			return
				TraCICommandHelper.ExecuteGetCommand<double>(
					Client,
					id,
					TraCIConstants.CMD_GET_EDGE_VARIABLE,
					TraCIConstants.VAR_PMXEMISSION);
		}

        /// <summary>
        /// Sum of NOx emissions on this edge in mg during this time step.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<double> GetNOxEmission (string id)
		{
			return
				TraCICommandHelper.ExecuteGetCommand<double>(
					Client,
					id,
					TraCIConstants.CMD_GET_EDGE_VARIABLE,
					TraCIConstants.VAR_NOXEMISSION);
		}

        /// <summary>
        /// Sum of fuel consumption on this edge in ml during this time step.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<double> GetFuelConsumption (string id)
		{
			return
				TraCICommandHelper.ExecuteGetCommand<double>(
					Client,
					id,
					TraCIConstants.CMD_GET_EDGE_VARIABLE,
					TraCIConstants.VAR_FUELCONSUMPTION);
		}

        /// <summary>
        /// Sum of noise generated on this edge in dBA.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<double> GetNoiseEmission (string id)
		{
			return
				TraCICommandHelper.ExecuteGetCommand<double>(
					Client,
					id,
					TraCIConstants.CMD_GET_EDGE_VARIABLE,
					TraCIConstants.VAR_NOISEEMISSION);
		}

        /// <summary>
        /// Sum of electricity consumption on this edge in kWh during this time step.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<double> GetElectricityConsumption (string id)
		{
			return
				TraCICommandHelper.ExecuteGetCommand<double>(
					Client,
					id,
					TraCIConstants.CMD_GET_EDGE_VARIABLE,
					TraCIConstants.VAR_ELECTRICITYCONSUMPTION);
		}

        /// <summary>
        /// The number of vehicles on this edge within the last time step.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<int> GetLastStepVehicleNumber (string id)
		{
			return
				TraCICommandHelper.ExecuteGetCommand<int>(
					Client,
					id,
					TraCIConstants.CMD_GET_EDGE_VARIABLE,
					TraCIConstants.STOP_CONTAINER_STOP);
		}

        /// <summary>
        /// Returns the mean speed of vehicles that were on the named edge within the last simulation step [m/s]
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<double> GetLastStepMeanSpeed (string id)
		{
			return
				TraCICommandHelper.ExecuteGetCommand<double>(
					Client,
					id,
					TraCIConstants.CMD_GET_EDGE_VARIABLE,
					TraCIConstants.TYPE_COLOR);
		}

        /// <summary>
        /// Returns the list of ids of vehicles that were on the named edge in the last simulation step. The order is from rightmost to leftmost lane and downstream for each lane.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TraCIResponse<List<string>> GetLastStepVehicleIDs (string id)
		{
			return
				TraCICommandHelper.ExecuteGetCommand<List<string>>(
					Client,
					id,
					TraCIConstants.CMD_GET_EDGE_VARIABLE,
					TraCIConstants.LAST_STEP_VEHICLE_ID_LIST);
		}

        /// <summary>
        /// Returns the percentage of time the edge was occupied by a vehicle [%]
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<double> GetLastStepOccupancy (string id)
		{
			return
				TraCICommandHelper.ExecuteGetCommand<double>(
					Client,
					id,
					TraCIConstants.CMD_GET_EDGE_VARIABLE,
					TraCIConstants.LAST_STEP_OCCUPANCY);
		}

        /// <summary>
        /// The mean length of vehicles which were on the edge in the last step [m]
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<double> GetLastStepLength (string id)
		{
			return
				TraCICommandHelper.ExecuteGetCommand<double>(
					Client,
					id,
					TraCIConstants.CMD_GET_EDGE_VARIABLE,
					TraCIConstants.LAST_STEP_LENGTH);
		}

        /// <summary>
        /// Returns the waiting time for all vehicles on the edge [s]
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<double> GetWaitingTime (string id)
		{
			return
				TraCICommandHelper.ExecuteGetCommand<double>(
					Client,
					id,
					TraCIConstants.CMD_GET_EDGE_VARIABLE,
					TraCIConstants.VAR_WAITING_TIME);
		}

        /// <summary>
        /// 	Returns the list of ids of persons that were on the named edge in the last simulation step
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<List<string>> GetLastStepPersonIDs (string id)
		{
			return
				TraCICommandHelper.ExecuteGetCommand<List<string>>(
					Client,
					id,
					TraCIConstants.CMD_GET_EDGE_VARIABLE,
					TraCIConstants.LAST_STEP_PERSON_ID_LIST);
		}

        /// <summary>
        /// Returns the total number of halting vehicles for the last time step on the given edge. A speed of less than 0.1 m/s is considered a halt.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<int> GetLastStepHaltingNumber(string id)
		{
			return
				TraCICommandHelper.ExecuteGetCommand<int>(
					Client,
					id,
					TraCIConstants.CMD_GET_EDGE_VARIABLE,
					TraCIConstants.LAST_STEP_VEHICLE_HALTING_NUMBER);
		}

        // TODO: 'extended retrieval', see: http://sumo.dlr.de/wiki/TraCI/Edge_Value_Retrieval


        /// <summary>
        /// Inserts the information about the travel time of the named edge valid from begin time to end time into the global edge weights times container.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="travelTimeValue"></param>
        /// <returns></returns>
        public TraCIResponse<double> AdaptTraveltime(string id, int beginTime, int endTime,double travelTimeValue)
        {
            var tmp = new CompoundObject();
            tmp.Value.Add(new TraCIInteger() { Value = beginTime });
            tmp.Value.Add(new TraCIInteger() { Value = endTime });
            tmp.Value.Add(new TraCIDouble() { Value = travelTimeValue });

            return TraCICommandHelper.ExecuteSetCommand<double, CompoundObject>(
                     Client,
                     id,
                     TraCIConstants.CMD_SET_EDGE_VARIABLE,
                     TraCIConstants.VAR_EDGE_TRAVELTIME,
                     tmp
                     );
        }

        /// <summary>
        /// Inserts the information about the effort of the named edge valid from begin time to end time into the global edge weights container.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="effortValue"></param>
        /// <returns></returns>
        public TraCIResponse<object> SetEffort(string id, double beginTime, double endTime, double effortValue)
        {
            var tmp = new CompoundObject();
            tmp.Value.Add(new TraCIDouble() { Value = beginTime });
            tmp.Value.Add(new TraCIDouble() { Value = endTime });
            tmp.Value.Add(new TraCIDouble() { Value = effortValue });

            return TraCICommandHelper.ExecuteSetCommand<object, CompoundObject>(
                     Client,
                     id,
                     TraCIConstants.CMD_SET_EDGE_VARIABLE,
                     TraCIConstants.VAR_EDGE_EFFORT,
                     tmp
                     );
        }

        /// <summary>
        /// Set a new maximum speed (in m/s) for all lanes of the edge.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="speed"></param>
        /// <returns></returns>
        public TraCIResponse<object> SetMaxSpeed(string id, double speed)
        {
            return TraCICommandHelper.ExecuteSetCommand<object, double>(
                                 Client,
                                 id,
                                 TraCIConstants.CMD_SET_EDGE_VARIABLE,
                                 TraCIConstants.VAR_MAXSPEED,
                                 speed
                                 );
        }

        public void Subscribe(string objectId, int beginTime, int endTime, List<byte> ListOfVariablesToSubsribeTo)
        {
            TraCICommandHelper.ExecuteSubscribeCommand(
                Client,
                beginTime,
                endTime,
                objectId,
                TraCIConstants.CMD_SUBSCRIBE_EDGE_VARIABLE,
                ListOfVariablesToSubsribeTo);
        }
        #endregion // Public Methods

        #region Constructor

        public EdgeCommands(TraCIClient client) : base(client)
		{
		}

		#endregion // Constructor
	}
}

