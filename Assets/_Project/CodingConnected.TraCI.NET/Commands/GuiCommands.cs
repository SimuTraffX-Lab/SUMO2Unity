using System.Collections.Generic;
using CodingConnected.TraCI.NET.Helpers;
using CodingConnected.TraCI.NET.Types;

namespace CodingConnected.TraCI.NET.Commands
{
	public class GuiCommands : TraCICommandsBase
	{
        #region Public Methods

        /// <summary>
        /// determines whether graphical capabilities exist
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TraCIResponse<bool> HasView(string id)
        {
            return
                TraCICommandHelper.ExecuteGetCommand<bool>(
                    Client,
                    id,
                    TraCIConstants.CMD_GET_GUI_VARIABLE,
                    TraCIConstants.VAR_HAS_VIEW);
        }

        /// <summary>
        /// the current zoom level (in %)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TraCIResponse<double> GetZoom(string id)
		{
			return
				TraCICommandHelper.ExecuteGetCommand<double>(
					Client,
					id,
					TraCIConstants.CMD_GET_GUI_VARIABLE,
					TraCIConstants.VAR_VIEW_ZOOM);
		}

        /// <summary>
        /// the center of the currently visible part of the net
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<Position2D> GetOffset(string id)
		{
			return
				TraCICommandHelper.ExecuteGetCommand<Position2D>(
				Client,
				id,
				TraCIConstants.CMD_GET_GUI_VARIABLE,
				TraCIConstants.VAR_VIEW_OFFSET);
		}

        /// <summary>
        /// the visualization scheme used (e.g. "standard" or "real world")
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<string> GetSchema(string id)
		{
			return
				TraCICommandHelper.ExecuteGetCommand<string>(
					Client,
					id,
					TraCIConstants.CMD_GET_GUI_VARIABLE,
					TraCIConstants.VAR_VIEW_SCHEMA);
		}

        /// <summary>
        /// the lower left and the upper right corner of the visible network
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public TraCIResponse<Polygon> GetBoundary(string id)
		{
			return
				TraCICommandHelper.ExecuteGetCommand<Polygon>(
				Client,
				id,
				TraCIConstants.CMD_GET_GUI_VARIABLE,
				TraCIConstants.VAR_VIEW_BOUNDARY);
		}

        /// <summary>
        /// 	Sets the current zoom level in %
        /// </summary>
        /// <param name="id"></param>
        /// <param name="zoom"></param>
        /// <returns></returns>
        public TraCIResponse<object> SetZoom(string id, double zoom)
        {
            return TraCICommandHelper.ExecuteSetCommand<object, double>(
                    Client,
                    id,
                    TraCIConstants.CMD_SET_GUI_VARIABLE,
                    TraCIConstants.VAR_VIEW_ZOOM,
                    zoom
                    );
        }

        /// <summary>
        /// Moves the center of the visible network to the given position
        /// </summary>
        /// <param name="id"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public TraCIResponse<object> SetOffset(string id, Position2D position)
        {
            return TraCICommandHelper.ExecuteSetCommand<object, Position2D>(
                    Client,
                    id,
                    TraCIConstants.CMD_SET_GUI_VARIABLE,
                    TraCIConstants.VAR_VIEW_OFFSET,
                    position
                    );
        }

        /// <summary>
        /// Sets the visualization scheme (e.g. "standard")
        /// </summary>
        /// <param name="id"></param>
        /// <param name="schema"></param>
        /// <returns></returns>
        public TraCIResponse<object> SetSchema(string id, string schema)
        {
            return TraCICommandHelper.ExecuteSetCommand<object, string>(
                    Client,
                    id,
                    TraCIConstants.CMD_SET_GUI_VARIABLE,
                    TraCIConstants.VAR_VIEW_SCHEMA,
                    schema
                    );
        }

        /// <summary>
        /// Moves the center of the visible network to the given position
        /// </summary>
        /// <param name="id"></param>
        /// <param name="boundaryBox"></param>
        /// <returns></returns>
        public TraCIResponse<object> SetBoundary(string id, Polygon boundaryBox)
        {
            return TraCICommandHelper.ExecuteSetCommand<object, Polygon>(
                    Client,
                    id,
                    TraCIConstants.CMD_SET_GUI_VARIABLE,
                    TraCIConstants.VAR_VIEW_BOUNDARY,
                    boundaryBox
                    );
        }

        /// <summary>
        /// Save a screenshot to the given file
        /// </summary>
        /// <param name="id"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public TraCIResponse<object> Screenshot(string id, string filename)
        {
            return TraCICommandHelper.ExecuteSetCommand<object, string>(
                    Client,
                    id,
                    TraCIConstants.CMD_SET_GUI_VARIABLE,
                    TraCIConstants.VAR_SCREENSHOT,
                    filename
                    );
        }

        /// <summary>
        /// tracks the given vehicle in the GUI
        /// </summary>
        /// <param name="id"></param>
        /// <param name="vehicleId"></param>
        /// <returns></returns>
        public TraCIResponse<object> TrackVehicle(string id, string vehicleId)
        {
            return TraCICommandHelper.ExecuteSetCommand<object, string>(
                    Client,
                    id,
                    TraCIConstants.CMD_SET_GUI_VARIABLE,
                    TraCIConstants.VAR_TRACK_VEHICLE,
                    vehicleId
                    );
        }

        public void Subscribe(string objectId, int beginTime, int endTime, List<byte> ListOfVariablesToSubsribeTo)
        {
            TraCICommandHelper.ExecuteSubscribeCommand(
                Client,
                beginTime,
                endTime,
                objectId,
                TraCIConstants.CMD_SUBSCRIBE_GUI_VARIABLE,
                ListOfVariablesToSubsribeTo);
        }
        #endregion // Public Methods

        #region Constructor

        public GuiCommands(TraCIClient client) : base(client)
		{
		}

		#endregion // Constructor
	}
}

