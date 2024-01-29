using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodingConnected.TraCI.NET.Helpers;
using CodingConnected.TraCI.NET.Types;

namespace CodingConnected.TraCI.NET.Commands
{
	public class ControlCommands : TraCICommandsBase
	{
        #region Public Methods

        /// <summary>
        /// Gets an identifying version number as described here: http://sumo.dlr.de/wiki/TraCI/Control-related_commands
        /// </summary>
        public int GetVersionId()
		{
			var command = new TraCICommand
			{
				Identifier = TraCIConstants.CMD_GETVERSION, 
				Contents = null
			};
			var response = Client.SendMessage(command);
			if (response?.Length == 2)
			{
				return BitConverter.ToInt32(response[1].Response.Take(4).Reverse().ToArray(), 0);
			}
			return -1;
		}

		/// <summary>
		/// Gets a user friendly string describing the version of SUMO
		/// </summary>
		public string GetVersionString()
		{
			var command = new TraCICommand
			{
				Identifier = TraCIConstants.CMD_GETVERSION,
				Contents = null
			};
			var response = Client.SendMessage(command);
			if (response?.Length == 2)
			{
				var strlen = response[1].Response.Skip(4).Take(4).Reverse().ToArray();
				var idl = BitConverter.ToInt32(strlen, 0);
				var ver = Encoding.ASCII.GetString(response[1].Response, 8, idl);
				return ver;
			}
			return null;
		}

		/// <summary>
		/// Instruct SUMO to execute a single simulation step
		/// Note: the size of the step is set via the relevant .sumcfg file
		/// </summary>
		/// <param name="targetTime">If this is not 0, SUMO will run until target time is reached</param>
		public TraCIResponse<object> SimStep(double targetTime = 0)
		{
			var command = new TraCICommand
			{
				Identifier = TraCIConstants.CMD_SIMSTEP,
				Contents = TraCIDataConverter.GetTraCIBytesFromDouble(targetTime)
			};

			var response = Client.SendMessage(command);
            var tmp = TraCIDataConverter.ExtractDataFromResponse<object>(response, TraCIConstants.CMD_SIMSTEP);

            if (tmp.Content != null)
            {
                var listOfSubscriptions = tmp.Content as List<TraCISubscriptionResponse>;
                foreach (var item in listOfSubscriptions)
                {
                    bool isVariableSubscription = true;
                    SubscriptionEventArgs eventArgs;

                    // subscription can only be Variable or Context Subrciption. If it isnt the first then it is the latter
                    var subscription = item as TraCIVariableSubscriptionResponse;
                    if (subscription != null)
                    {
                        eventArgs = new VariableSubscriptionEventArgs(
                            item.ObjectId,
                            item.VariableCount,
                            subscription.ResponseByVariableCode
                            );
                        isVariableSubscription = true;
                    }
                    else
                    {
                        var i = (item as TraCIContextSubscriptionResponse);
                        eventArgs = new ContextSubscriptionEventArgs
                            (
                            i.ObjectId,
                            i.VariableSubscriptionByObjectId,
                            i.ContextDomain,
                            i.VariableCount,
                            i.ObjectCount
                            );
                        isVariableSubscription = false;
                    }

                    eventArgs.Responses = item.Responses;

                    switch (item.ResponseCode)
                    {
                        case TraCIConstants.RESPONSE_SUBSCRIBE_INDUCTIONLOOP_VARIABLE:
                            Client.OnInductionLoopSubscription(eventArgs);
                            break;
                        case TraCIConstants.RESPONSE_SUBSCRIBE_MULTIENTRYEXIT_VARIABLE:
                            Client.OnMultiEntryExitSubscription(eventArgs);
                            break;
                        case TraCIConstants.RESPONSE_SUBSCRIBE_TL_VARIABLE:
                            Client.OnTrafficLightSubscription(eventArgs);
                            break;
                        case TraCIConstants.RESPONSE_SUBSCRIBE_LANE_VARIABLE:
                            Client.OnLaneSubscription(eventArgs);
                            break;
                        case TraCIConstants.RESPONSE_SUBSCRIBE_VEHICLE_VARIABLE:
                            Client.OnVehicleSubscription(eventArgs);
                            break;
                        case TraCIConstants.RESPONSE_SUBSCRIBE_VEHICLETYPE_VARIABLE:
                            Client.OnVehicleTypeSubscription(eventArgs);
                            break;
                        case TraCIConstants.RESPONSE_SUBSCRIBE_ROUTE_VARIABLE:
                            Client.OnRouteSubscription(eventArgs);
                            break;
                        case TraCIConstants.RESPONSE_SUBSCRIBE_POI_VARIABLE:
                            Client.OnPOISubscription(eventArgs);
                            break;
                        case TraCIConstants.RESPONSE_SUBSCRIBE_POLYGON_VARIABLE:
                            Client.OnPolygonSubscription(eventArgs);
                            break;
                        case TraCIConstants.RESPONSE_SUBSCRIBE_JUNCTION_VARIABLE:
                            Client.OnJunctionSubscription(eventArgs);
                            break;
                        case TraCIConstants.RESPONSE_SUBSCRIBE_EDGE_VARIABLE:
                            Client.OnEdgeSubscription(eventArgs);
                            break;
                        case TraCIConstants.RESPONSE_SUBSCRIBE_SIM_VARIABLE:
                            Client.OnSimulationSubscription(eventArgs);
                            break;
                        case TraCIConstants.RESPONSE_SUBSCRIBE_GUI_VARIABLE:
                            Client.OnGUISubscription(eventArgs);
                            break;
                        case TraCIConstants.RESPONSE_SUBSCRIBE_LANEAREA_VARIABLE:
                            Client.OnLaneAreaSubscription(eventArgs);
                            break;
                        case TraCIConstants.RESPONSE_SUBSCRIBE_PERSON_VARIABLE:
                            Client.OnPersonSubscription(eventArgs);
                            break;
                        case TraCIConstants.RESPONSE_SUBSCRIBE_INDUCTIONLOOP_CONTEXT:
                            Client.OnInductionLoopContextSubscription(eventArgs as ContextSubscriptionEventArgs);
                            break;
                        case TraCIConstants.RESPONSE_SUBSCRIBE_LANE_CONTEXT:
                            Client.OnLaneContextSubscription(eventArgs as ContextSubscriptionEventArgs);
                            break;
                        case TraCIConstants.RESPONSE_SUBSCRIBE_VEHICLE_CONTEXT:
                            Client.OnVehicleContextSubscription(eventArgs as ContextSubscriptionEventArgs);
                            break;
                        case TraCIConstants.RESPONSE_SUBSCRIBE_POI_CONTEXT:
                            Client.OnPOIContextSubscription(eventArgs as ContextSubscriptionEventArgs);
                            break;
                        case TraCIConstants.RESPONSE_SUBSCRIBE_POLYGON_CONTEXT:
                            Client.OnPolygonContextSubscription(eventArgs as ContextSubscriptionEventArgs);
                            break;
                        case TraCIConstants.RESPONSE_SUBSCRIBE_JUNCTION_CONTEXT:
                            Client.OnJunctionContextSubscription(eventArgs as ContextSubscriptionEventArgs);
                            break;
                        case TraCIConstants.RESPONSE_SUBSCRIBE_EDGE_CONTEXT:
                            Client.OnEdgeContextSubscription(eventArgs as ContextSubscriptionEventArgs);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            return tmp;
        }

        /// <summary>
        /// Instruct SUMO to stop the simulation and close
        /// </summary>
        public void Close()
		{
			var command = new TraCICommand
			{
				Identifier = TraCIConstants.CMD_CLOSE,
				Contents = null
			};
			// ReSharper disable once UnusedVariable
			var response = Client.SendMessage(command);
		}

		/// <summary>
		/// Tells TraCI to reload the simulation with the given options
		/// <remarks>Loading does not work when using multiple clients, currently</remarks>
		/// </summary>
		/// <param name="options">List of options to pass to SUMO</param>
		public void Load(List<string> options)
		{
			var command = new TraCICommand
			{
				Identifier = TraCIConstants.CMD_LOAD
			};
			var n = new List<byte>();
			n.AddRange(TraCIDataConverter.GetTraCIBytesFromInt32(options.Count));
			foreach (var opt in options)
			{
				n.AddRange(TraCIDataConverter.GetTraCIBytesFromInt32(opt.Length));
				n.AddRange(TraCIDataConverter.GetTraCIBytesFromASCIIString(opt));
			}
			command.Contents = n.ToArray();
			// ReSharper disable once UnusedVariable
			var response = Client.SendMessage(command);
		}

		/// <summary>
		/// Tells TraCI to reload the simulation with the given options
		/// <remarks>Loading does not work when using multiple clients, currently</remarks>
		/// </summary>
		/// <param name="options">List of options to pass to SUMO</param>
		public void SetOrder(int index)
		{
			var command = new TraCICommand
			{
				Identifier = TraCIConstants.CMD_SETORDER, 
				Contents = BitConverter.GetBytes(index).Reverse().ToArray()
			};
			var response = Client.SendMessage(command);
		}

        #endregion // Public Methods

        #region Private Methods

        
        #endregion
        #region Constructor

        public ControlCommands(TraCIClient client) : base(client)
		{
		}

		#endregion // Constructor
	}
}
