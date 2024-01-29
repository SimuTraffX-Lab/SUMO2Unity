using System;
using System.Collections.Generic;
using CodingConnected.TraCI.NET.Helpers;
using CodingConnected.TraCI.NET.Types;

namespace CodingConnected.TraCI.NET.Commands
{
	public class TrafficLightCommands : TraCICommandsBase
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
					TraCIConstants.CMD_GET_TL_VARIABLE,
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
					TraCIConstants.CMD_GET_TL_VARIABLE,
					TraCIConstants.ID_COUNT);
		}

        /// <summary>
        /// Returns the named tl's state as a tuple of light definitions from
        /// rugGyYoO, for red, yed-yellow, green, yellow, off, where lower case letters mean that the stream
        /// has to decelerate.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TraCIResponse<string> GetState(string id)
		{
			return 
				TraCICommandHelper.ExecuteGetCommand<string>(
					Client, 
					id, 
					TraCIConstants.CMD_GET_TL_VARIABLE,
					TraCIConstants.TL_RED_YELLOW_GREEN_STATE);
		}

        /// <summary>
        /// Returns the default total duration of the currently active phase in seconds; To obtain the remaining
        /// duration use (getNextSwitch() - simulation.getTime()); to obtain the spent duration subtract the 
        /// remaining from the total duration
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<double> GetPhaseDuration(string id)
		{
			return 
				TraCICommandHelper.ExecuteGetCommand<double>(
					Client, 
					id, 
					TraCIConstants.CMD_GET_TL_VARIABLE,
					TraCIConstants.TL_PHASE_DURATION);
		}

        /// <summary>
        /// Returns the list of lanes which are controlled by the named traffic light. Returns at least one entry 
        /// for every element of the phase state (signal index)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<List<string>> GetControlledLanes(string id)
		{
			return 
				TraCICommandHelper.ExecuteGetCommand<List<string>>(
					Client, 
					id, 
					TraCIConstants.CMD_GET_TL_VARIABLE,
					TraCIConstants.TL_CONTROLLED_LANES);
		}

        /// <summary>
        /// Returns the links controlled by the traffic light, sorted by the signal index and described by giving 
        /// the incoming, outgoing, and via lane.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<ControlledLinks> GetControlledLinks(string id)
		{
            var tmp = TraCICommandHelper.ExecuteGetCommand<CompoundObject>(
                    Client,
                    id,
                    TraCIConstants.CMD_GET_TL_VARIABLE,
                    TraCIConstants.TL_CONTROLLED_LINKS);

            var controlledLinks = TraCIDataConverter.ConvertToControlledLinks(tmp.Content.Value);

            var ret = new TraCIResponse<ControlledLinks>
            {
                Content = controlledLinks,
                ErrorMessage = tmp.ErrorMessage,
                Identifier = tmp.Identifier,
                ResponseIdentifier = tmp.ResponseIdentifier,
                Result = tmp.Result,
                Variable = tmp.Variable
            };

            return ret;
        }

        /// <summary>
        /// Returns the index of the current phase in the current program
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<int> GetCurrentPhase(string id)
		{
			return 
				TraCICommandHelper.ExecuteGetCommand<int>(
					Client, 
					id, 
					TraCIConstants.CMD_GET_TL_VARIABLE,
					TraCIConstants.TL_CURRENT_PHASE);
		}

        /// <summary>
        /// Returns the id of the current program
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<string> GetCurrentProgram(string id)
		{
			return 
				TraCICommandHelper.ExecuteGetCommand<string>(
					Client, 
					id, 
					TraCIConstants.CMD_GET_TL_VARIABLE,
					TraCIConstants.TL_CURRENT_PROGRAM);
		}

        /// <summary>
        /// Returns the complete traffic light program, structure described under data types
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<TrafficCompleteLightProgram> GetCompleteDefinition(string id)
		{
            var tmp = TraCICommandHelper.ExecuteGetCommand<CompoundObject>(
                    Client,
                    id,
                    TraCIConstants.CMD_GET_TL_VARIABLE,
                    TraCIConstants.TL_COMPLETE_DEFINITION_RYG);

            var tmp2 = TraCIDataConverter.ConvertToTrafficLightCompleteProgramm(tmp.Content);

            var ret = new TraCIResponse<TrafficCompleteLightProgram>
            {
                Content = tmp2,
                ErrorMessage = tmp.ErrorMessage,
                Identifier = tmp.Identifier,
                ResponseIdentifier = tmp.ResponseIdentifier,
                Result = tmp.Result,
                Variable = tmp.Variable
            };

            return ret;
        }

        /// <summary>
        /// Returns the assumed time (in seconds) at which the tls changes the phase. Please note
        /// that the time to switch is not relative to current simulation step (the result returned
        /// by the query will be absolute time, counting from simulation start); to obtain relative
        /// time, one needs to subtract current simulation time from the result returned by this 
        /// query. Please also note that the time may vary in the case of actuated/adaptive traffic lights
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TraCIResponse<double> GetNextSwitch(string id)
		{
			return 
				TraCICommandHelper.ExecuteGetCommand<double>(
					Client, 
					id, 
					TraCIConstants.CMD_GET_TL_VARIABLE,
					TraCIConstants.TL_NEXT_SWITCH);
		}

        /// <summary>
        /// Sets the phase definition to the given. Assumes the given string is a tuple of light definitions
        /// from rRgGyYoO, for red, green, yellow, off, where lower case letters mean that the stream has to 
        /// decelerate. After this call the program-ID of the traffic light will be set to "online" and the 
        /// state will be maintained until the next call of setRedYellowGreenState() or until setting another
        /// program with setProgram().
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public TraCIResponse<object> SetRedYellowGreenState(string id, string state)
        {
            return TraCICommandHelper.ExecuteSetCommand<object, string>(
                Client,
                id,
                TraCIConstants.CMD_SET_TL_VARIABLE,
                TraCIConstants.TL_RED_YELLOW_GREEN_STATE,
                state);
        }

        /// <summary>
        /// Sets the phase of the traffic light to the given. The given index must be valid for the current 
        /// program of the traffic light, this means it must be between 0 and the number of phases known to 
        /// the current program of the tls - 1.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="phaseIndex"></param>
        /// <returns></returns>
        public TraCIResponse<object> SetPhase(string id, int phaseIndex)
        {
            return TraCICommandHelper.ExecuteSetCommand<object, int>(
                Client,
                id,
                TraCIConstants.CMD_SET_TL_VARIABLE,
                TraCIConstants.TL_PHASE_INDEX,
                phaseIndex);
        }

        /// <summary>
        /// Switches the traffic light to the given program. No WAUT algorithm is used, the program is 
        /// directly instantiated. The index of the traffic light stays the same as before.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="program"></param>
        /// <returns></returns>
        public TraCIResponse<object> SetProgram(string id, string program)
        {
            return TraCICommandHelper.ExecuteSetCommand<object, string>(
                Client,
                id,
                TraCIConstants.CMD_SET_TL_VARIABLE,
                TraCIConstants.TL_PROGRAM,
                program);
        }

        /// <summary>
        /// Sets the remaining duration of the current phase in seconds.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="phaseDuration"></param>
        /// <returns></returns>
        public TraCIResponse<object> SetPhaseDuration(string id, double phaseDuration)
        {
            return TraCICommandHelper.ExecuteSetCommand<object, double>(
                Client,
                id,
                TraCIConstants.CMD_SET_TL_VARIABLE,
                TraCIConstants.TL_PHASE_DURATION,
                phaseDuration);
        }

        /// <summary>
        /// Inserts a completely new program.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="program"></param>
        /// <returns></returns>
        public TraCIResponse<object> SetCompleteRedYellowGreenDefinition(string id, TrafficLightProgram program)
        {
            // TODO: move this to TraCICommandHelper.ExecuteSetCommand
            
            var bytes = new List<byte> { TraCIConstants.TL_COMPLETE_PROGRAM_RYG }; //messageType (0x2c)
            bytes.AddRange(TraCIDataConverter.GetTraCIBytesFromASCIIString(id));
            bytes.Add(TraCIConstants.TYPE_COMPOUND); //value type compound
            bytes.AddRange(TraCIDataConverter.GetTraCIBytesFromInt32(5 + (program.Phases.Count * 4))); //item number
            bytes.Add(TraCIConstants.TYPE_STRING); //value type string
            bytes.AddRange(TraCIDataConverter.GetTraCIBytesFromASCIIString(program.ProgramId)); //program ID
            bytes.Add(TraCIConstants.TYPE_INTEGER); //value type integer
            bytes.AddRange(TraCIDataConverter.GetTraCIBytesFromInt32(0)); //Type (always 0)
            bytes.Add(TraCIConstants.TYPE_COMPOUND); //value type compound
            bytes.AddRange(TraCIDataConverter.GetTraCIBytesFromInt32(0));//Compound Length (always 0!)
            bytes.Add(TraCIConstants.TYPE_INTEGER); //value type integer
            bytes.AddRange(TraCIDataConverter.GetTraCIBytesFromInt32(program.PhaseIndex)); //Phase Index
            bytes.Add(TraCIConstants.TYPE_INTEGER); //value type integer
            bytes.AddRange(TraCIDataConverter.GetTraCIBytesFromInt32(program.Phases.Count)); //Phase Number

            foreach (var p in program.Phases)//Phases
            {
                bytes.Add(TraCIConstants.TYPE_DOUBLE); //value type integer
                bytes.AddRange(TraCIDataConverter.GetTraCIBytesFromDouble(p.Duration)); //Duration[ms]
                bytes.Add(TraCIConstants.TYPE_DOUBLE); //value type integer
                bytes.AddRange(TraCIDataConverter.GetTraCIBytesFromDouble(0)); //unused
                bytes.Add(TraCIConstants.TYPE_DOUBLE); //value type integer
                bytes.AddRange(TraCIDataConverter.GetTraCIBytesFromDouble(0)); //unused
                bytes.Add(TraCIConstants.TYPE_STRING); //value type string
                bytes.AddRange(TraCIDataConverter.GetTraCIBytesFromASCIIString(p.Definition)); //State (light/priority-tuple)
            }

            var command = new TraCICommand
            {
                Identifier = TraCIConstants.CMD_SET_TL_VARIABLE,
                Contents = bytes.ToArray()
            };

            var response = Client.SendMessage(command);

#warning is the try catch necessary?
            try
            {
                return TraCIDataConverter.ExtractDataFromResponse<object>(response, TraCIConstants.CMD_SET_TL_VARIABLE, TraCIConstants.TL_COMPLETE_PROGRAM_RYG);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectId">Id of the object to subscribe to. If you want to subscribe to id list or count, use "ignored" as object id</param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="ListOfVariablesToSubsribeTo"></param>
        public void Subscribe(string objectId, int beginTime, int endTime, List<byte> ListOfVariablesToSubsribeTo)
        {
            TraCICommandHelper.ExecuteSubscribeCommand(
                Client,
                beginTime,
                endTime,
                objectId,
                TraCIConstants.CMD_SUBSCRIBE_TL_VARIABLE,
                ListOfVariablesToSubsribeTo);
        }
        #endregion // Public Methods

        #region Constructor

        public TrafficLightCommands(TraCIClient client) : base(client)
		{
		}

		#endregion // Constructor
	}
}