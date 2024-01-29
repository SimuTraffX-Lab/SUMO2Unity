namespace CodingConnected.TraCI.NET
{
    /// <summary>
    /// Based on TraCIConst
    /// </summary>
    public class TraCIConstants
    {

        // ****************************************
        // VERSION
        // ****************************************
        public const int TRACI_VERSION = 18;

        // ****************************************
        // COMMANDS
        // ****************************************
        // command: get version
        public const byte CMD_GETVERSION = 0x00;

        // command: load
        public const byte CMD_LOAD = 0x01;

        // command: simulation step
        public const byte CMD_SIMSTEP = 0x02;

        // command: set connection priority (execution order)
        public const byte CMD_SETORDER = 0x03;

        // command: stop node
        public const byte CMD_STOP = 0x12;

        // command: reroute to parking area
        public const byte CMD_REROUTE_TO_PARKING = 0xc2;

        // command: Resume from parking
        public const byte CMD_RESUME = 0x19;

        // command: set lane
        public const byte CMD_CHANGELANE = 0x13;

        // command: slow down
        public const byte CMD_SLOWDOWN = 0x14;

        // command: set sublane (vehicle)
        public const byte CMD_CHANGESUBLANE = 0x15;

        // command: change target
        public const byte CMD_CHANGETARGET = 0x31;

        // command: close sumo
        public const byte CMD_CLOSE = 0x7F;

        // command: add subscription filter
        public const byte CMD_ADD_SUBSCRIPTION_FILTER = 0x7e;


        // command: subscribe induction loop (e1) context
        public const byte CMD_SUBSCRIBE_INDUCTIONLOOP_CONTEXT = 0x80;
        // response: subscribe induction loop (e1) context
        public const byte RESPONSE_SUBSCRIBE_INDUCTIONLOOP_CONTEXT = 0x90;
        // command: get induction loop (e1) variable
        public const byte CMD_GET_INDUCTIONLOOP_VARIABLE = 0xa0;
        // response: get induction loop (e1) variable
        public const byte RESPONSE_GET_INDUCTIONLOOP_VARIABLE = 0xb0;
        // command: subscribe induction loop (e1) variable
        public const byte CMD_SUBSCRIBE_INDUCTIONLOOP_VARIABLE = 0xd0;
        // response: subscribe induction loop (e1) variable
        public const byte RESPONSE_SUBSCRIBE_INDUCTIONLOOP_VARIABLE = 0xe0;

        // command: subscribe multi-entry/multi-exit detector (e3) context
        public const byte CMD_SUBSCRIBE_MULTIENTRYEXIT_CONTEXT = 0x81;
        // response: subscribe multi-entry/multi-exit detector (e3) context
        public const byte RESPONSE_SUBSCRIBE_MULTIENTRYEXIT_CONTEXT = 0x91;
        // command: get multi-entry/multi-exit detector (e3) variable
        public const byte CMD_GET_MULTIENTRYEXIT_VARIABLE = 0xa1;
        // response: get multi-entry/multi-exit detector (e3) variable
        public const byte RESPONSE_GET_MULTIENTRYEXIT_VARIABLE = 0xb1;
        // command: subscribe multi-entry/multi-exit detector (e3) variable
        public const byte CMD_SUBSCRIBE_MULTIENTRYEXIT_VARIABLE = 0xd1;
        // response: subscribe multi-entry/multi-exit detector (e3) variable
        public const byte RESPONSE_SUBSCRIBE_MULTIENTRYEXIT_VARIABLE = 0xe1;

        // command: subscribe traffic lights context
        public const byte CMD_SUBSCRIBE_TL_CONTEXT = 0x82;
        // response: subscribe traffic lights context
        public const byte RESPONSE_SUBSCRIBE_TL_CONTEXT = 0x92;
        // command: get traffic lights variable
        public const byte CMD_GET_TL_VARIABLE = 0xa2;
        // response: get traffic lights variable
        public const byte RESPONSE_GET_TL_VARIABLE = 0xb2;
        // command: set traffic lights variable
        public const byte CMD_SET_TL_VARIABLE = 0xc2;
        // command: subscribe traffic lights variable
        public const byte CMD_SUBSCRIBE_TL_VARIABLE = 0xd2;
        // response: subscribe traffic lights variable
        public const byte RESPONSE_SUBSCRIBE_TL_VARIABLE = 0xe2;

        // command: subscribe lane context
        public const byte CMD_SUBSCRIBE_LANE_CONTEXT = 0x83;
        // response: subscribe lane context
        public const byte RESPONSE_SUBSCRIBE_LANE_CONTEXT = 0x93;
        // command: get lane variable
        public const byte CMD_GET_LANE_VARIABLE = 0xa3;
        // response: get lane variable
        public const byte RESPONSE_GET_LANE_VARIABLE = 0xb3;
        // command: set lane variable
        public const byte CMD_SET_LANE_VARIABLE = 0xc3;
        // command: subscribe lane variable
        public const byte CMD_SUBSCRIBE_LANE_VARIABLE = 0xd3;
        // response: subscribe lane variable
        public const byte RESPONSE_SUBSCRIBE_LANE_VARIABLE = 0xe3;

        // command: subscribe vehicle context
        public const byte CMD_SUBSCRIBE_VEHICLE_CONTEXT = 0x84;
        // response: subscribe vehicle context
        public const byte RESPONSE_SUBSCRIBE_VEHICLE_CONTEXT = 0x94;
        // command: get vehicle variable
        public const byte CMD_GET_VEHICLE_VARIABLE = 0xa4;
        // response: get vehicle variable
        public const byte RESPONSE_GET_VEHICLE_VARIABLE = 0xb4;
        // command: set vehicle variable
        public const byte CMD_SET_VEHICLE_VARIABLE = 0xc4;
        // command: subscribe vehicle variable
        public const byte CMD_SUBSCRIBE_VEHICLE_VARIABLE = 0xd4;
        // response: subscribe vehicle variable
        public const byte RESPONSE_SUBSCRIBE_VEHICLE_VARIABLE = 0xe4;

        // command: subscribe vehicle type context
        public const byte CMD_SUBSCRIBE_VEHICLETYPE_CONTEXT = 0x85;
        // response: subscribe vehicle type context
        public const byte RESPONSE_SUBSCRIBE_VEHICLETYPE_CONTEXT = 0x95;
        // command: get vehicle type variable
        public const byte CMD_GET_VEHICLETYPE_VARIABLE = 0xa5;
        // response: get vehicle type variable
        public const byte RESPONSE_GET_VEHICLETYPE_VARIABLE = 0xb5;
        // command: set vehicle type variable
        public const byte CMD_SET_VEHICLETYPE_VARIABLE = 0xc5;
        // command: subscribe vehicle type variable
        public const byte CMD_SUBSCRIBE_VEHICLETYPE_VARIABLE = 0xd5;
        // response: subscribe vehicle type variable
        public const byte RESPONSE_SUBSCRIBE_VEHICLETYPE_VARIABLE = 0xe5;

        // command: subscribe route context
        public const byte CMD_SUBSCRIBE_ROUTE_CONTEXT = 0x86;
        // response: subscribe route context
        public const byte RESPONSE_SUBSCRIBE_ROUTE_CONTEXT = 0x96;
        // command: get route variable
        public const byte CMD_GET_ROUTE_VARIABLE = 0xa6;
        // response: get route variable
        public const byte RESPONSE_GET_ROUTE_VARIABLE = 0xb6;
        // command: set route variable
        public const byte CMD_SET_ROUTE_VARIABLE = 0xc6;
        // command: subscribe route variable
        public const byte CMD_SUBSCRIBE_ROUTE_VARIABLE = 0xd6;
        // response: subscribe route variable
        public const byte RESPONSE_SUBSCRIBE_ROUTE_VARIABLE = 0xe6;

        // command: subscribe poi context
        public const byte CMD_SUBSCRIBE_POI_CONTEXT = 0x87;
        // response: subscribe poi context
        public const byte RESPONSE_SUBSCRIBE_POI_CONTEXT = 0x97;
        // command: get poi variable
        public const byte CMD_GET_POI_VARIABLE = 0xa7;
        // response: get poi variable
        public const byte RESPONSE_GET_POI_VARIABLE = 0xb7;
        // command: set poi variable
        public const byte CMD_SET_POI_VARIABLE = 0xc7;
        // command: subscribe poi variable
        public const byte CMD_SUBSCRIBE_POI_VARIABLE = 0xd7;
        // response: subscribe poi variable
        public const byte RESPONSE_SUBSCRIBE_POI_VARIABLE = 0xe7;

        // command: subscribe polygon context
        public const byte CMD_SUBSCRIBE_POLYGON_CONTEXT = 0x88;
        // response: subscribe polygon context
        public const byte RESPONSE_SUBSCRIBE_POLYGON_CONTEXT = 0x98;
        // command: get polygon variable
        public const byte CMD_GET_POLYGON_VARIABLE = 0xa8;
        // response: get polygon variable
        public const byte RESPONSE_GET_POLYGON_VARIABLE = 0xb8;
        // command: set polygon variable
        public const byte CMD_SET_POLYGON_VARIABLE = 0xc8;
        // command: subscribe polygon variable
        public const byte CMD_SUBSCRIBE_POLYGON_VARIABLE = 0xd8;
        // response: subscribe polygon variable
        public const byte RESPONSE_SUBSCRIBE_POLYGON_VARIABLE = 0xe8;

        // command: subscribe junction context
        public const byte CMD_SUBSCRIBE_JUNCTION_CONTEXT = 0x89;
        // response: subscribe junction context
        public const byte RESPONSE_SUBSCRIBE_JUNCTION_CONTEXT = 0x99;
        // command: get junction variable
        public const byte CMD_GET_JUNCTION_VARIABLE = 0xa9;
        // response: get junction variable
        public const byte RESPONSE_GET_JUNCTION_VARIABLE = 0xb9;
        // command: set junction variable
        public const byte CMD_SET_JUNCTION_VARIABLE = 0xc9;
        // command: subscribe junction variable
        public const byte CMD_SUBSCRIBE_JUNCTION_VARIABLE = 0xd9;
        // response: subscribe junction variable
        public const byte RESPONSE_SUBSCRIBE_JUNCTION_VARIABLE = 0xe9;

        // command: subscribe edge context
        public const byte CMD_SUBSCRIBE_EDGE_CONTEXT = 0x8a;
        // response: subscribe edge context
        public const byte RESPONSE_SUBSCRIBE_EDGE_CONTEXT = 0x9a;
        // command: get edge variable
        public const byte CMD_GET_EDGE_VARIABLE = 0xaa;
        // response: get edge variable
        public const byte RESPONSE_GET_EDGE_VARIABLE = 0xba;
        // command: set edge variable
        public const byte CMD_SET_EDGE_VARIABLE = 0xca;
        // command: subscribe edge variable
        public const byte CMD_SUBSCRIBE_EDGE_VARIABLE = 0xda;
        // response: subscribe edge variable
        public const byte RESPONSE_SUBSCRIBE_EDGE_VARIABLE = 0xea;

        // command: subscribe simulation context
        public const byte CMD_SUBSCRIBE_SIM_CONTEXT = 0x8b;
        // response: subscribe simulation context
        public const byte RESPONSE_SUBSCRIBE_SIM_CONTEXT = 0x9b;
        // command: get simulation variable
        public const byte CMD_GET_SIM_VARIABLE = 0xab;
        // response: get simulation variable
        public const byte RESPONSE_GET_SIM_VARIABLE = 0xbb;
        // command: set simulation variable
        public const byte CMD_SET_SIM_VARIABLE = 0xcb;
        // command: subscribe simulation variable
        public const byte CMD_SUBSCRIBE_SIM_VARIABLE = 0xdb;
        // response: subscribe simulation variable
        public const byte RESPONSE_SUBSCRIBE_SIM_VARIABLE = 0xeb;

        // command: subscribe GUI context
        public const byte CMD_SUBSCRIBE_GUI_CONTEXT = 0x8c;
        // response: subscribe GUI context
        public const byte RESPONSE_SUBSCRIBE_GUI_CONTEXT = 0x9c;
        // command: get GUI variable
        public const byte CMD_GET_GUI_VARIABLE = 0xac;
        // response: get GUI variable
        public const byte RESPONSE_GET_GUI_VARIABLE = 0xbc;
        // command: set GUI variable
        public const byte CMD_SET_GUI_VARIABLE = 0xcc;
        // command: subscribe GUI variable
        public const byte CMD_SUBSCRIBE_GUI_VARIABLE = 0xdc;
        // response: subscribe GUI variable
        public const byte RESPONSE_SUBSCRIBE_GUI_VARIABLE = 0xec;

        // command: subscribe areal detector (e2) context
        public const byte CMD_SUBSCRIBE_LANEAREA_CONTEXT = 0x8d;
        // response: subscribe areal detector (e2) context
        public const byte RESPONSE_SUBSCRIBE_LANEAREA_CONTEXT = 0x9d;
        // command: get areal detector (e2) variable
        public const byte CMD_GET_LANEAREA_VARIABLE = 0xad;
        // response: get areal detector (e2) variable
        public const byte RESPONSE_GET_LANEAREA_VARIABLE = 0xbd;
        // command: subscribe areal detector (e2) variable
        public const byte CMD_SUBSCRIBE_LANEAREA_VARIABLE = 0xdd;
        // response: subscribe areal detector (e2) variable
        public const byte RESPONSE_SUBSCRIBE_LANEAREA_VARIABLE = 0xed;

        // command: subscribe person context
        public const byte CMD_SUBSCRIBE_PERSON_CONTEXT = 0x8e;
        // response: subscribe person context
        public const byte RESPONSE_SUBSCRIBE_PERSON_CONTEXT = 0x9e;
        // command: get person variable
        public const byte CMD_GET_PERSON_VARIABLE = 0xae;
        // response: get person variable
        public const byte RESPONSE_GET_PERSON_VARIABLE = 0xbe;
        // command: set person variable
        public const byte CMD_SET_PERSON_VARIABLE = 0xce;
        // command: subscribe person variable
        public const byte CMD_SUBSCRIBE_PERSON_VARIABLE = 0xde;
        // response: subscribe person variable
        public const byte RESPONSE_SUBSCRIBE_PERSON_VARIABLE = 0xee;


        // ****************************************
        // POSITION REPRESENTATIONS
        // ****************************************
        // Position in geo-coordinates
        public const byte POSITION_LON_LAT = 0x00;
        // 2D cartesian coordinates
        public const byte POSITION_2D = 0x01;
        // Position in geo-coordinates with altitude
        public const byte POSITION_LON_LAT_ALT = 0x02;
        // 3D cartesian coordinates
        public const byte POSITION_3D = 0x03;
        // Position on road map
        public const byte POSITION_ROADMAP = 0x04;


        // ****************************************
        // DATA TYPES
        // ****************************************
        // Boundary Box (4 doubles)
        public const byte TYPE_BOUNDINGBOX = 0x05;
        // Polygon (2*n doubles)
        public const byte TYPE_POLYGON = 0x06;
        // unsigned byte
        public const byte TYPE_UBYTE = 0x07;
        // signed byte
        public const byte TYPE_BYTE = 0x08;
        // 32 bit signed integer
        public const byte TYPE_INTEGER = 0x09;
        // float
        public const byte TYPE_FLOAT = 0x0A;
        // double
        public const byte TYPE_DOUBLE = 0x0B;
        // 8 bit ASCII string
        public const byte TYPE_STRING = 0x0C;
        // list of traffic light phases
        public const byte TYPE_TLPHASELIST = 0x0D;
        // list of strings
        public const byte TYPE_STRINGLIST = 0x0E;
        // compound object
        public const byte TYPE_COMPOUND = 0x0F;
        // color (four ubytes)
        public const byte TYPE_COLOR = 0x11;


        // ****************************************
        // DATA SIZE
        // ****************************************

        public const byte BYTE_SIZE = 1;
        public const byte UBYTE_SIZE = 1;
        public const byte INTEGER_SIZE = 4;
        public const byte FLOAT_SIZE = 4;
        public const byte DOUBLE_SIZE = 8;

        // ****************************************
        // RESULT TYPES
        // ****************************************
        // result type: Ok
        public const byte RTYPE_OK = 0x00;
        // result type: not implemented
        public const byte RTYPE_NOTIMPLEMENTED = 0x01;
        // result type: error
        public const byte RTYPE_ERR = 0xFF;

        // return value for invalid queries (especially vehicle is not on the road), see Position::INVALID
        public const double INVALID_DOUBLE_VALUE = -1073741824;
        // return value for invalid queries (especially vehicle is not on the road), see Position::INVALID
        public const double INVALID_INT_VALUE = -1073741824;
        // maximum value for client ordering (2 ^ 30)
        public const double MAX_ORDER = 1073741824;

        // ****************************************
        // TRAFFIC LIGHT PHASES
        // ****************************************
        // red phase
        public const byte TLPHASE_RED = 0x01;
        // yellow phase
        public const byte TLPHASE_YELLOW = 0x02;
        // green phase
        public const byte TLPHASE_GREEN = 0x03;
        // tl is blinking
        public const byte TLPHASE_BLINKING = 0x04;
        // tl is off and not blinking
        public const byte TLPHASE_NOSIGNAL = 0x05;


        // ****************************************
        // DIFFERENT DISTANCE REQUESTS
        // ****************************************
        // air distance
        public const byte REQUEST_AIRDIST = 0x00;
        // driving distance
        public const byte REQUEST_DRIVINGDIST = 0x01;


        // ****************************************
        // VEHICLE REMOVAL REASONS
        // ****************************************
        // vehicle started teleport
        public const byte REMOVE_TELEPORT = 0x00;
        // vehicle removed while parking
        public const byte REMOVE_PARKING = 0x01;
        // vehicle arrived
        public const byte REMOVE_ARRIVED = 0x02;
        // vehicle was vaporized
        public const byte REMOVE_VAPORIZED = 0x03;
        // vehicle finished route during teleport
        public const byte REMOVE_TELEPORT_ARRIVED = 0x04;

        // ****************************************
        // PERSON/CONTAINER STAGES
        // ****************************************
        // person / container stopping
        public const byte STAGE_WAITING_FOR_DEPART = 0x00;
        // person / container stopping
        public const byte STAGE_WAITING = 0x01;
        // person walking / container transhiping
        public const byte STAGE_WALKING = 0x02;
        // person riding / container being transported
        public const byte STAGE_DRIVING = 0x03;

        // ****************************************
        // Stop Flags
        // ****************************************
        public const byte STOP_DEFAULT = 0x00;
        public const byte STOP_PARKING = 0x01;
        public const byte STOP_TRIGGERED = 0x02;
        public const byte STOP_CONTAINER_TRIGGERED = 0x04;
        public const byte STOP_BUS_STOP = 0x08;
        public const byte STOP_CONTAINER_STOP = 0x10;
        public const byte STOP_CHARGING_STATION = 0x20;
        public const byte STOP_PARKING_AREA = 0x40;

        // ****************************************
        // Departure Flags
        // ****************************************
        public const short DEPARTFLAG_TRIGGERED = -0x01;
        public const short DEPARTFLAG_CONTAINER_TRIGGERED = -0x02;
        public const short DEPARTFLAG_NOW = -0x03;

        public const short DEPARTFLAG_SPEED_RANDOM = -0x02;
        public const short DEPARTFLAG_SPEED_MAX = -0x03;

        public const short DEPARTFLAG_LANE_RANDOM = -0x02;
        public const short DEPARTFLAG_LANE_FREE = -0x03;
        public const short DEPARTFLAG_LANE_ALLOWED_FREE = -0x04;
        public const short DEPARTFLAG_LANE_BEST_FREE = -0x05;
        public const short DEPARTFLAG_LANE_FIRST_ALLOWED = -0x06;

        public const short DEPARTFLAG_POS_RANDOM = -0x02;
        public const short DEPARTFLAG_POS_FREE = -0x03;
        public const short DEPARTFLAG_POS_BASE = -0x04;
        public const short DEPARTFLAG_POS_LAST = -0x05;
        public const short DEPARTFLAG_POS_RANDOM_FREE = -0x06;

        public const short ARRIVALFLAG_LANE_CURRENT = -0x02;
        public const short ARRIVALFLAG_SPEED_CURRENT = -0x02;

        public const short ARRIVALFLAG_POS_RANDOM = -0x02;
        public const short ARRIVALFLAG_POS_MAX = -0x03;

        // ****************************************
        // Routing modes
        // ****************************************
        // use custom weights if available, fall back to loaded weights and then to free-flow speed
        public const byte ROUTING_MODE_DEFAULT = 0x00;
        // use aggregated travel times from device.rerouting
        public const byte ROUTING_MODE_AGGREGATED = 0x01;
        // use loaded efforts
        public const byte ROUTING_MODE_EFFORT = 0x02;
        // use combined costs
        public const byte ROUTING_MODE_COMBINED = 0x03;

        // ****************************************
        // FILTER TYPES (for context subscription filters)
        // ****************************************

        // Reset all filters
        public const byte FILTER_TYPE_NONE = 0x00;

        // Filter by list of lanes relative to ego vehicle
        public const byte FILTER_TYPE_LANES = 0x01;

        // Exclude vehicles on opposite (and other) lanes from context subscription result
        public const byte FILTER_TYPE_NOOPPOSITE = 0x02;

        // Specify maximal downstream distance for vehicles in context subscription result
        public const byte FILTER_TYPE_DOWNSTREAM_DIST = 0x03;

        // Specify maximal upstream distance for vehicles in context subscription result
        public const byte FILTER_TYPE_UPSTREAM_DIST = 0x04;

        // Only return leader and follower in context subscription result
        public const byte FILTER_TYPE_CF_MANEUVER = 0x05;

        // Only return leader and follower on ego and neighboring lane in context subscription result
        public const byte FILTER_TYPE_LC_MANEUVER = 0x06;

        // Only return foes on upcoming junction in context subscription result
        public const byte FILTER_TYPE_TURN_MANEUVER = 0x07;

        // Only return vehicles of the given vClass in context subscription result
        public const byte FILTER_TYPE_VCLASS = 0x08;

        // Only return vehicles of the given vType in context subscription result
        public const byte FILTER_TYPE_VTYPE = 0x09;





        // ****************************************
        // VARIABLE TYPES (for CMD_GET_*_VARIABLE)
        // ****************************************
        // list of instances' ids (get: all)
        public const byte ID_LIST = 0x00;

        // count of instances (get: all)
        public const byte ID_COUNT = 0x01;

        // subscribe object variables (get: all)
        public const byte AUTOMATIC_VARIABLES_SUBSCRIPTION = 0x02;

        // subscribe context variables (get: all)
        public const byte AUTOMATIC_CONTEXT_SUBSCRIPTION = 0x03;

        // generic attributes (get/set: all)
        public const byte GENERIC_ATTRIBUTE = 0x03;

        // last step vehicle number (get: induction loops, multi-entry/multi-exit detector, lanes, edges)
        public const byte LAST_STEP_VEHICLE_NUMBER = 0x10;

        // last step vehicle number (get: induction loops, multi-entry/multi-exit detector, lanes, edges)
        public const byte LAST_STEP_MEAN_SPEED = 0x11;

        // last step vehicle list (get: induction loops, multi-entry/multi-exit detector, lanes, edges)
        public const byte LAST_STEP_VEHICLE_ID_LIST = 0x12;

        // last step occupancy (get: induction loops, lanes, edges)
        public const byte LAST_STEP_OCCUPANCY = 0x13;

        // last step vehicle halting number (get: multi-entry/multi-exit detector, lanes, edges)
        public const byte LAST_STEP_VEHICLE_HALTING_NUMBER = 0x14;

        // last step mean vehicle length (get: induction loops, lanes, edges)
        public const byte LAST_STEP_LENGTH = 0x15;

        // last step time since last detection (get: induction loops)
        public const byte LAST_STEP_TIME_SINCE_DETECTION = 0x16;

        // entry times
        public const byte LAST_STEP_VEHICLE_DATA = 0x17;

        // last step jam length in vehicles
        public const byte JAM_LENGTH_VEHICLE = 0x18;

        // last step jam length in meters
        public const byte JAM_LENGTH_METERS = 0x19;

        // last step person list (get: edges, vehicles)
        public const byte LAST_STEP_PERSON_ID_LIST = 0x1a;

        // street name of given edge
        public const byte VAR_STREET_NAME = 0x1b;

        // traffic light states, encoded as rRgGyYoO tuple (get: traffic lights)
        public const byte TL_RED_YELLOW_GREEN_STATE = 0x20;

        // index of the phase (set: traffic lights)
        public const byte TL_PHASE_INDEX = 0x22;

        // traffic light program (set: traffic lights)
        public const byte TL_PROGRAM = 0x23;

        // phase duration (set: traffic lights)
        public const byte TL_PHASE_DURATION = 0x24;

        // controlled lanes (get: traffic lights)
        public const byte TL_CONTROLLED_LANES = 0x26;

        // controlled links (get: traffic lights)
        public const byte TL_CONTROLLED_LINKS = 0x27;

        // index of the current phase (get: traffic lights)
        public const byte TL_CURRENT_PHASE = 0x28;

        // name of the current program (get: traffic lights)
        public const byte TL_CURRENT_PROGRAM = 0x29;

        // controlled junctions (get: traffic lights)
        public const byte TL_CONTROLLED_JUNCTIONS = 0x2a;

        // complete definition (get: traffic lights)
        public const byte TL_COMPLETE_DEFINITION_RYG = 0x2b;

        // complete program (set: traffic lights)
        public const byte TL_COMPLETE_PROGRAM_RYG = 0x2c;

        // assumed time to next switch (get: traffic lights)
        public const byte TL_NEXT_SWITCH = 0x2d;

        // current state, using external signal names (get: traffic lights)
        public const byte TL_EXTERNAL_STATE = 0x2e;

        // outgoing link number (get: lanes)
        public const byte LANE_LINK_NUMBER = 0x30;

        // id of parent edge (get: lanes)
        public const byte LANE_EDGE_ID = 0x31;

        // outgoing link definitions (get: lanes)
        public const byte LANE_LINKS = 0x33;

        // list of allowed vehicle classes (get&set: lanes)
        public const byte LANE_ALLOWED = 0x34;

        // list of not allowed vehicle classes (get&set: lanes)
        public const byte LANE_DISALLOWED = 0x35;

        // list of foe lanes (get: lanes)
        public const byte VAR_FOES = 0x37;

        // slope (get: edge, lane, vehicle, person)
        public const byte VAR_SLOPE = 0x36;

        // speed (get: vehicle)
        public const byte VAR_SPEED = 0x40;

        // maximum allowed/possible speed (get: vehicle types, lanes, set: edges, lanes)
        public const byte VAR_MAXSPEED = 0x41;

        // position (2D) (get: vehicle, poi, inductionloop, areadetector; set: poi)
        public const byte VAR_POSITION = 0x42;

        // position (3D) (get: vehicle, poi, set: poi)
        public const byte VAR_POSITION3D = 0x39;

        // angle (get: vehicle)
        public const byte VAR_ANGLE = 0x43;

        // angle (get: vehicle types, lanes, arealdetector, set: lanes)
        public const byte VAR_LENGTH = 0x44;

        // color (get: vehicles, vehicle types, polygons, pois)
        public const byte VAR_COLOR = 0x45;

        // max. acceleration (get: vehicles, vehicle types)
        public const byte VAR_ACCEL = 0x46;

        // max. comfortable deceleration (get: vehicles, vehicle types)
        public const byte VAR_DECEL = 0x47;

        // max. (physically possible) deceleration (get: vehicles, vehicle types)
        public const byte VAR_EMERGENCY_DECEL = 0x7b;

        // apparent deceleration (get: vehicles, vehicle types)
        public const byte VAR_APPARENT_DECEL = 0x7c;

        // action step length (get: vehicles, vehicle types)
        public const byte VAR_ACTIONSTEPLENGTH = 0x7d;

        // last action time (get: vehicles)
        public const byte VAR_LASTACTIONTIME = 0x7f;

        // driver's desired headway (get: vehicle types)
        public const byte VAR_TAU = 0x48;

        // vehicle class (get: vehicle types)
        public const byte VAR_VEHICLECLASS = 0x49;

        // emission class (get: vehicle types)
        public const byte VAR_EMISSIONCLASS = 0x4a;

        // shape class (get: vehicle types)
        public const byte VAR_SHAPECLASS = 0x4b;

        // minimum gap (get: vehicle types)
        public const byte VAR_MINGAP = 0x4c;

        // width (get: vehicle types, lanes)
        public const byte VAR_WIDTH = 0x4d;

        // shape (get: polygons)
        public const byte VAR_SHAPE = 0x4e;

        // type id (get: vehicles, polygons, pois)
        public const byte VAR_TYPE = 0x4f;

        // road id (get: vehicles)
        public const byte VAR_ROAD_ID = 0x50;

        // lane id (get: vehicles, inductionloop, arealdetector)
        public const byte VAR_LANE_ID = 0x51;

        // lane index (get: vehicle, edge)
        public const byte VAR_LANE_INDEX = 0x52;

        // route id (get & set: vehicles)
        public const byte VAR_ROUTE_ID = 0x53;

        // edges (get: routes, vehicles)
        public const byte VAR_EDGES = 0x54;

        // update bestLanes (set: vehicle)
        public const byte VAR_UPDATE_BESTLANES = 0x6a;

        // filled? (get: polygons)
        public const byte VAR_FILL = 0x55;

        // position (1D along lane) (get: vehicle)
        public const byte VAR_LANEPOSITION = 0x56;

        // route (set: vehicles)
        public const byte VAR_ROUTE = 0x57;

        // travel time information (get&set: vehicle)
        public const byte VAR_EDGE_TRAVELTIME = 0x58;

        // effort information (get&set: vehicle)
        public const byte VAR_EDGE_EFFORT = 0x59;

        // last step travel time (get: edge, lane)
        public const byte VAR_CURRENT_TRAVELTIME = 0x5a;

        // signals state (get/set: vehicle)
        public const byte VAR_SIGNALS = 0x5b;

        // new lane/position along (set: vehicle)
        public const byte VAR_MOVE_TO = 0x5c;

        // driver imperfection (set: vehicle)
        public const byte VAR_IMPERFECTION = 0x5d;

        // speed factor (set: vehicle)
        public const byte VAR_SPEED_FACTOR = 0x5e;

        // speed deviation (set: vehicle)
        public const byte VAR_SPEED_DEVIATION = 0x5f;

        // routing mode (get/set: vehicle)
        public const byte VAR_ROUTING_MODE = 0x89;

        // speed without TraCI influence (get: vehicle)
        public const byte VAR_SPEED_WITHOUT_TRACI = 0xb1;

        // best lanes (get: vehicle)
        public const byte VAR_BEST_LANES = 0xb2;

        // how speed is set (set: vehicle)
        public const byte VAR_SPEEDSETMODE = 0xb3;

        // move vehicle to explicit (remote controlled) position (set: vehicle)
        public const byte MOVE_TO_XY = 0xb4;

        // is the vehicle stopped, and if so parked and/or triggered?
        // value = stopped + 2 * parking + 4 * triggered
        public const byte VAR_STOPSTATE = 0xb5;

        // how lane changing is performed (get/set: vehicle)
        public const byte VAR_LANECHANGE_MODE = 0xb6;

        // maximum speed regarding max speed on the current lane and speed factor (get: vehicle)
        public const byte VAR_ALLOWED_SPEED = 0xb7;

        // position (1D lateral position relative to center of the current lane) (get: vehicle)
        public const byte VAR_LANEPOSITION_LAT = 0xb8;

        // get/set prefered lateral alignment within the lane (vehicle)
        public const byte VAR_LATALIGNMENT = 0xb9;

        // get/set maximum lateral speed (vehicle, vtypes)
        public const byte VAR_MAXSPEED_LAT = 0xba;

        // get/set minimum lateral gap (vehicle, vtypes)
        public const byte VAR_MINGAP_LAT = 0xbb;

        // get/set vehicle height (vehicle, vtypes)
        public const byte VAR_HEIGHT = 0xbc;

        // get/set vehicle line
        public const byte VAR_LINE = 0xbd;

        // get/set vehicle via
        public const byte VAR_VIA = 0xbe;

        // current CO2 emission of a node (get: vehicle, lane, edge)
        public const byte VAR_CO2EMISSION = 0x60;

        // current CO emission of a node (get: vehicle, lane, edge)
        public const byte VAR_COEMISSION = 0x61;

        // current HC emission of a node (get: vehicle, lane, edge)
        public const byte VAR_HCEMISSION = 0x62;

        // current PMx emission of a node (get: vehicle, lane, edge)
        public const byte VAR_PMXEMISSION = 0x63;

        // current NOx emission of a node (get: vehicle, lane, edge)
        public const byte VAR_NOXEMISSION = 0x64;

        // current fuel consumption of a node (get: vehicle, lane, edge)
        public const byte VAR_FUELCONSUMPTION = 0x65;

        // current noise emission of a node (get: vehicle, lane, edge)
        public const byte VAR_NOISEEMISSION = 0x66;

        // current person number (get: vehicle)
        public const byte VAR_PERSON_NUMBER = 0x67;

        // number of persons waiting at a defined bus stop (get: simulation)
        public const byte VAR_BUS_STOP_WAITING = 0x67;

        // current leader together with gap (get: vehicle)
        public const byte VAR_LEADER = 0x68;

        // edge index in current route (get: vehicle)
        public const byte VAR_ROUTE_INDEX = 0x69;

        // current waiting time (get: vehicle, lane)
        public const byte VAR_WAITING_TIME = 0x7a;

        // current waiting time (get: vehicle)
        public const byte VAR_ACCUMULATED_WAITING_TIME = 0x87;

        // upcoming traffic lights (get: vehicle)
        public const byte VAR_NEXT_TLS = 0x70;

        // upcoming stops (get: vehicle)
        public const byte VAR_NEXT_STOPS = 0x73;

        // current acceleration (get: vehicle)
        public const byte VAR_ACCELERATION = 0x72;

        // current time in seconds (get: simulation)
        public const byte VAR_TIME = 0x66;

        // current time step (get: simulation)
        public const byte VAR_TIME_STEP = 0x70;

        // current electricity consumption of a node (get: vehicle, lane, edge)
        public const byte VAR_ELECTRICITYCONSUMPTION = 0x71;

        // number of loaded vehicles (get: simulation)
        public const byte VAR_LOADED_VEHICLES_NUMBER = 0x71;

        // loaded vehicle ids (get: simulation)
        public const byte VAR_LOADED_VEHICLES_IDS = 0x72;

        // number of departed vehicle (get: simulation)
        public const byte VAR_DEPARTED_VEHICLES_NUMBER = 0x73;

        // departed vehicle ids (get: simulation)
        public const byte VAR_DEPARTED_VEHICLES_IDS = 0x74;

        // number of vehicles starting to teleport (get: simulation)
        public const byte VAR_TELEPORT_STARTING_VEHICLES_NUMBER = 0x75;

        // ids of vehicles starting to teleport (get: simulation)
        public const byte VAR_TELEPORT_STARTING_VEHICLES_IDS = 0x76;

        // number of vehicles ending to teleport (get: simulation)
        public const byte VAR_TELEPORT_ENDING_VEHICLES_NUMBER = 0x77;

        // ids of vehicles ending to teleport (get: simulation)
        public const byte VAR_TELEPORT_ENDING_VEHICLES_IDS = 0x78;

        // number of arrived vehicles (get: simulation)
        public const byte VAR_ARRIVED_VEHICLES_NUMBER = 0x79;

        // ids of arrived vehicles (get: simulation)
        public const byte VAR_ARRIVED_VEHICLES_IDS = 0x7a;

        // delta t (get: simulation)
        public const byte VAR_DELTA_T = 0x7b;

        // bounding box (get: simulation)
        public const byte VAR_NET_BOUNDING_BOX = 0x7c;

        // minimum number of expected vehicles (get: simulation)
        public const byte VAR_MIN_EXPECTED_VEHICLES = 0x7d;

        // number of vehicles starting to park (get: simulation)
        public const byte VAR_STOP_STARTING_VEHICLES_NUMBER = 0x68;

        // ids of vehicles starting to park (get: simulation)
        public const byte VAR_STOP_STARTING_VEHICLES_IDS = 0x69;

        // number of vehicles ending to park (get: simulation)
        public const byte VAR_STOP_ENDING_VEHICLES_NUMBER = 0x6a;

        // ids of vehicles ending to park (get: simulation)
        public const byte VAR_STOP_ENDING_VEHICLES_IDS = 0x6b;

        // number of vehicles starting to park (get: simulation)
        public const byte VAR_PARKING_STARTING_VEHICLES_NUMBER = 0x6c;

        // ids of vehicles starting to park (get: simulation)
        public const byte VAR_PARKING_STARTING_VEHICLES_IDS = 0x6d;

        // number of vehicles ending to park (get: simulation)
        public const byte VAR_PARKING_ENDING_VEHICLES_NUMBER = 0x6e;

        // ids of vehicles ending to park (get: simulation)
        public const byte VAR_PARKING_ENDING_VEHICLES_IDS = 0x6f;

        // number of vehicles involved in a collision (get: simulation)
        public const byte VAR_COLLIDING_VEHICLES_NUMBER = 0x80;

        // ids of vehicles involved in a collision (get: simulation)
        public const byte VAR_COLLIDING_VEHICLES_IDS = 0x81;

        // number of vehicles involved in a collision (get: simulation)
        public const byte VAR_EMERGENCYSTOPPING_VEHICLES_NUMBER = 0x89;

        // ids of vehicles involved in a collision (get: simulation)
        public const byte VAR_EMERGENCYSTOPPING_VEHICLES_IDS = 0x8a;

        // clears the simulation of all not inserted vehicles (set: simulation)
        public const byte CMD_CLEAR_PENDING_VEHICLES = 0x94;

        // triggers saving simulation state (set: simulation)
        public const byte CMD_SAVE_SIMSTATE = 0x95;

        // sets/retrieves abstract parameter
        public const byte VAR_PARAMETER = 0x7e;


        // add an instance (poi, polygon, vehicle, person, route)
        public const byte ADD = 0x80;

        // remove an instance (poi, polygon, vehicle, person)
        public const byte REMOVE = 0x81;

        // copy an instance (vehicle type, other TBD.)
        public const byte COPY = 0x88;

        // convert coordinates
        public const byte POSITION_CONVERSION = 0x82;

        // distance between points or vehicles
        public const byte DISTANCE_REQUEST = 0x83;

        // the current driving distance
        public const byte VAR_DISTANCE = 0x84;

        // add a fully specified instance (vehicle)
        public const byte ADD_FULL = 0x85;

        // find a car based route
        public const byte FIND_ROUTE = 0x86;

        // find an intermodal route
        public const byte FIND_INTERMODAL_ROUTE = 0x87;

        // force rerouting based on travel time (vehicles)
        public const byte CMD_REROUTE_TRAVELTIME = 0x90;

        // force rerouting based on effort (vehicles)
        public const byte CMD_REROUTE_EFFORT = 0x91;

        // validates current route (vehicles)
        public const byte VAR_ROUTE_VALID = 0x92;

        // retrieve information regarding the current person/container stage
        public const byte VAR_STAGE = 0xc0;

        // retrieve information regarding the next edge including crossings and walkingAreas (pedestrians only)
        public const byte VAR_NEXT_EDGE = 0xc1;

        // retrieve information regarding the number of remaining stages
        public const byte VAR_STAGES_REMAINING = 0xc2;

        // retrieve the current vehicle id for the driving stage (person, container)
        public const byte VAR_VEHICLE = 0xc3;

        // append a person stage (person)
        public const byte APPEND_STAGE = 0xc4;

        // append a person stage (person)
        public const byte REMOVE_STAGE = 0xc5;

        // zoom
        public const byte VAR_VIEW_ZOOM = 0xa0;

        // view position
        public const byte VAR_VIEW_OFFSET = 0xa1;

        // view schema
        public const byte VAR_VIEW_SCHEMA = 0xa2;

        // view by boundary
        public const byte VAR_VIEW_BOUNDARY = 0xa3;

        // screenshot
        public const byte VAR_SCREENSHOT = 0xa5;

        // track vehicle
        public const byte VAR_TRACK_VEHICLE = 0xa6;

        // presence of view
        public const byte VAR_HAS_VIEW = 0xa7;

    }
}