#Sumo2unity tool is developed by Ahmad Mohammadi, PhD in Transportation Engineering from York University, Canada.
#For any question, please join to our discord: https://discord.gg/REyhZhszAU

import os
import sys
import time
import json
import math
import threading
import statistics
import queue
import zmq
import logging

#Initial Variables 
IntegrationStartTime = 540 #At this time you connect Sumo and Unity and put participant in the Sumo2unity tool; but, the experiment will start in FromTime = 600--> basically, 600 - 540 = 60seconds is just for running unity
ExperimentStartTime = 600 #Experiment Start time ---> Until this time, Sumo is running without connecting to Unity --> This allows network pouplated with cars (known as warm-up period in traffic simulation softwares)
ExperimentEndTime = 720 #Experiment End time --> If your experiment is for example 2 min (120 seconds) then add that value to 600 seconds = 720 second --> In this second, the connection terminiated
steplength = 0.1 #Sumo step lenght --> This should match with Unity step lenght in simulation controller script in Unity

# Flags to control computation and logging of various results
COMPUTE_CONTEXT_CAR_SIM_SPEEDS = False
COMPUTE_CONTEXT_CAR_REAL_WORLD_SPEEDS = False
COMPUTE_F1_SPEEDS = False
COMPUTE_ALL_CARS = False  # Example if you had computations for "all cars"
COMPUTE_PROFILING_SUMMARY = False
COMPUTE_PROFILING_TIMES = False
COMPUTE_CARS_IN_CONTEXT_STATS = False
COMPUTE_MSGS_BYTES_STATS = False
COMPUTE_RTF = True

# Configure logging
logging.basicConfig(
    level=logging.DEBUG,  # Set to DEBUG for verbose output
    format='%(asctime)s - %(levelname)s - %(message)s',
    handlers=[
        #logging.FileHandler('sumo_unity.log'),
        logging.StreamHandler()
    ]
)
logger = logging.getLogger(__name__)

# SUMO setup
if 'SUMO_HOME' in os.environ:
    sys.path.append(os.path.join(os.environ['SUMO_HOME'], 'tools'))
else:
    sys.exit("Please declare environment variable 'SUMO_HOME'")

import traci
from traci.constants import VAR_POSITION3D, VAR_ANGLE, VAR_TYPE

# This gets the path where your script lives
script_dir  = os.path.dirname(os.path.abspath(__file__))
config_path = os.path.join(script_dir, "Sumo2Unity.sumocfg")

# SUMO configuration
Sumo_config = [
    'sumo-gui',
    '-c', config_path,
    '--step-length', str(steplength),
    '--delay', '0',
    '--lateral-resolution', '0.3',
]

traci.start(Sumo_config)
traci.vehicle.subscribeContext("f_0.0", traci.constants.CMD_GET_VEHICLE_VARIABLE, 250,
                               [VAR_POSITION3D, VAR_ANGLE, VAR_TYPE])

# Warm-Up Period -------
# Run through the first 280 seconds without sending data or adjusting GUI:
# 20 seconds for start Unity, all becomes 300 seconds
while traci.simulation.getTime() < IntegrationStartTime:
    traci.simulationStep()
# Do not send data to Unity here, just collect what you need internally.

traci.simulationStep()
traci.simulationStep()

viewport_id = 'View #0'
vehicle_id = 'f_0.0'
zoom_level = 100
x_offset = 221
y_offset = 106

traci.gui.trackVehicle(viewport_id, vehicle_id)
traci.gui.setZoom(viewport_id, zoom_level)
traci.gui.setOffset(viewport_id, x_offset, y_offset)
traci.gui.setSchema(viewport_id, "real world")

# ZMQ setup
context_zmq = zmq.Context()
pub_socket = context_zmq.socket(zmq.PUB)
pub_socket.bind("tcp://*:5556")
router_socket = context_zmq.socket(zmq.ROUTER)
router_socket.bind("tcp://*:5557")

last_send_time = None
current_second = 0
send_intervals = []
speed_values = []
real_world_speed_values = []  # For f_1.0 only
overall_simulation_speeds = []
overall_real_world_speeds = []  # For f_1.0 only
data_points = []  # For f_1.0 data
car_count_values = []
context_car_count_values = []
cumulative_context_car_counts = []
all_simulation_speeds_list = []
all_real_world_speeds_list = []
last_positions = {}
cumulative_car_counts = []
last_positions_z = {}

profiling_times = {
    'UnityDataCollectionTime': [],
    'SimulationStepTime': [],
    'SumoDataCollectionTime': [],
    'SocketSendTime': [],
    'DataProcessingTime': [],
    'TotalLoopTime': [],
    'IntervalBetweenSends': []
}

sent_messages_count = 0
received_messages_count = 0
sent_bytes_count = 0
received_bytes_count = 0
total_sent_messages_count = 0
total_received_messages_count = 0
total_sent_bytes_count = 0
total_received_bytes_count = 0

startRecordingSent = False  # New variable to indicate if we have sent the start recording signal to Unity

WINDOW_SIZE = 10  # Rolling average window size

real_world_speed_history = {}

def precise_sleep(duration):
    start = time.perf_counter()
    end = start + duration
    while True:
        now = time.perf_counter()
        remaining = end - now
        if remaining <= 0:
            break
        elif remaining > 0.002:
            time.sleep(0.001)

unity_data_queue = queue.Queue()

def receive_unity_data():
    global received_messages_count, received_bytes_count
    global total_received_messages_count, total_received_bytes_count
    while True:
        try:
            frames = router_socket.recv_multipart()
            if len(frames) == 2:
                identity, message = frames
                message_str = message.decode('utf-8')
                if not message_str:
                    logger.warning("Received empty message from Unity.")
                
                unity_data_wrapper = json.loads(message_str)
                unity_data_queue.put(unity_data_wrapper)
                
                received_messages_count += 1
                received_bytes_count += len(message)
                total_received_messages_count += 1
                total_received_bytes_count += len(message)
            else:
                logger.warning(f"Unexpected number of frames received: {len(frames)}")
        except Exception as e:
            logger.error(f"Error processing Unity data: {e}", exc_info=True)
            continue

unity_data_thread = threading.Thread(target=receive_unity_data, daemon=True)
unity_data_thread.start()

def sumoDataCollection():
    data_list = []
    vehicles_list = traci.vehicle.getIDList()
    speed_f1 = None
    position_f1 = None

    # For 'f_1.0'
    if 'f_1.0' in vehicles_list:
        speed_f1 = traci.vehicle.getSpeed('f_1.0')
        position_f1 = traci.vehicle.getPosition3D('f_1.0')

    current_sim_time = traci.simulation.getTime()

    if "f_0.0" in vehicles_list:
        try:
            x_veh1, y_veh1, z_veh1 = traci.vehicle.getPosition3D("f_0.0")
            angle_veh1 = traci.vehicle.getAngle("f_0.0")
            vehicle_type = traci.vehicle.getTypeID("f_0.0")

            rounded_position = (
                round(x_veh1, 2),
                round(y_veh1, 2),
                round(z_veh1, 2)
            )
            rounded_angle = round(angle_veh1, 2)
            current_timestamp = round(time.time(), 2)

            data_list.append({
                "vehicle_id": "f_0.0",
                "position": rounded_position,
                "angle": rounded_angle,
                "type": vehicle_type,
                "timestamp": current_timestamp
            })
           
            context_subscription = traci.vehicle.getContextSubscriptionResults("f_0.0")
            if context_subscription:
                vehicles_list = traci.vehicle.getIDList()
                for obj_id in context_subscription.keys():
                    if obj_id != "f_0.0" and obj_id in vehicles_list:
                        try:
                            x, y, z = traci.vehicle.getPosition3D(obj_id)
                            angle = traci.vehicle.getAngle(obj_id)
                            vehicle_type = traci.vehicle.getTypeID(obj_id)
                            Long_speed = traci.vehicle.getSpeed(obj_id)
                            lat_speed = traci.vehicle.getLateralSpeed(obj_id)

                            # Compute vertical speed for context vehicles
                            if obj_id in last_positions_z:
                                prev_z, prev_time = last_positions_z[obj_id]
                                dt = current_sim_time - prev_time
                                vert_speed = (z - prev_z) / dt if dt > 0 else 0.0
                            else:
                                vert_speed = 0.0

                            last_positions_z[obj_id] = (z, current_sim_time)

                            rounded_position = (
                                round(x, 3),
                                round(y, 3),
                                round(z, 3)
                            )
                            rounded_angle = round(angle, 3)
                            long_speed = round(Long_speed, 2)
                            vert_speed = round(vert_speed, 3)
                            lat_speed = round(lat_speed, 2)

                            data_list.append({
                                "vehicle_id": obj_id,
                                "position": rounded_position,
                                "angle": rounded_angle,
                                "type": vehicle_type,
                                "long_speed": long_speed,
                                "vert_speed": vert_speed,
                                "lat_speed": lat_speed
                            })
                        except traci.TraCIException as e:
                            logger.error(f"Error retrieving data for '{obj_id}': {e}")
            else:
                logger.debug("No vehicles within context subscription radius.")
        except traci.TraCIException as e:
            logger.error(f"Error retrieving data for 'f_0.0': {e}")
            return data_list, speed_f1, position_f1

    return data_list, speed_f1, position_f1

start_simulation_time = None
start_wall_time = None

# ─────────────────────────────────────────────────────────────────────────────
#  NEW helper – collect every TL's state each step
# ─────────────────────────────────────────────────────────────────────────────
def collectTrafficLights():
    """Return list of {'junction_id':<id>,'state':<string>}."""
    tl_list = []
    for tl_id in traci.trafficlight.getIDList():
        state = traci.trafficlight.getRedYellowGreenState(tl_id)  # e.g. "rrryg"
        tl_list.append({"junction_id": tl_id, "state": state})
    return tl_list


# Variables for incremental RTF measurement within the specified interval
rtf_measurement_started = False
last_sim_time = 0.0
last_real_time = 0.0

# Open the file for writing RTF results (overwrite each run)
#   parent_dir … SUMO2Unity/       (one level above SUMOData)
parent_dir   = os.path.abspath(os.path.join(os.path.dirname(__file__), os.pardir))
results_dir  = os.path.join(parent_dir, "Results")
os.makedirs(results_dir, exist_ok=True)

rtf_file = open(os.path.join(results_dir, "rtf_report.txt"), "w")
rtf_file.write("Time(s);RTF\n")

#Receive and send traffic state in each second instead of each tenth second
TL_UPDATE_INTERVAL = 1.0        # seconds
last_tl_time        = 0.0

try:
    SIM_STEP = steplength
    next_step_time = time.perf_counter() + SIM_STEP

    while traci.simulation.getMinExpectedNumber() > 0 and traci.simulation.getTime() < ExperimentEndTime:
        loop_start_time = time.perf_counter()
        sim_t = traci.simulation.getTime()

        # Unity Data Processing
        start_time = time.perf_counter()
        if not unity_data_queue.empty():
            while not unity_data_queue.empty():
                unity_data_wrapper = unity_data_queue.get()
                unity_data = unity_data_wrapper.get("vehicles", [])
                for vehicle in unity_data:
                    if vehicle["vehicle_id"] == "f_0.0":
                        x = float(vehicle["position"][0])
                        y = float(vehicle["position"][1])
                        angle = float(vehicle["angle"])
                        traci.vehicle.moveToXY(
                            vehicle["vehicle_id"],
                            "",
                            0,
                            x,
                            y,
                            angle,
                            keepRoute=2
                        )
        UnityDataCollectionTime = time.perf_counter() - start_time
        profiling_times['UnityDataCollectionTime'].append(UnityDataCollectionTime)

        # Simulation Step
        start_time = time.perf_counter()
        traci.simulationStep()
        #traci.executeMove()
        SimulationStepTime = time.perf_counter() - start_time
        profiling_times['SimulationStepTime'].append(SimulationStepTime)

        current_sim_time = traci.simulation.getTime()

        # Start measuring RTF once we pass FromTime if enabled
        if COMPUTE_RTF and not rtf_measurement_started and current_sim_time >= ExperimentStartTime:
            rtf_measurement_started = True

            start_simulation_time = current_sim_time
            start_wall_time = time.perf_counter()

            last_sim_time = current_sim_time
            last_real_time = time.perf_counter()  # reset baseline

            # Also send a special message to Unity to start recording from zero
            if not startRecordingSent:
                # Just send a simple JSON message indicating start
                # You can define a simple protocol: {"command": "START_RECORDING"}
                start_message = json.dumps({"type":"command", "command":"START_RECORDING"})
                pub_socket.send_string(start_message)
                startRecordingSent = True

                
        # Now handle RTF logging if we have started measurement and are within the time window
        if COMPUTE_RTF and rtf_measurement_started and current_sim_time >= ExperimentStartTime and current_sim_time < ExperimentEndTime :
            current_real_time = time.perf_counter()

            if current_sim_time == ExperimentStartTime:
                # First RTF entry: we have no interval yet, so just log 0.
                #logger.info(f"Incremental RTF (From {FromTime}s to {ToTime}s) at sim second {current_sim_time}: 0.0000")
                adjusted_time = current_sim_time - ExperimentStartTime
                rtf_file.write(f"{adjusted_time};0.0000\n")

                # Initialize your baseline now for the next interval
                last_sim_time = current_sim_time
                last_real_time = current_real_time
            else:
                # For subsequent intervals:
                sim_delta = current_sim_time - last_sim_time
                real_delta = current_real_time - last_real_time

                if real_delta > 0:
                    incremental_rtf = sim_delta / real_delta
                    #logger.info(f"Incremental RTF (From {FromTime}s to {ToTime}s) at sim second {current_sim_time}: {incremental_rtf:.4f}")

                    # Subtract FromTime from current_sim_time to start from 0
                    adjusted_time = current_sim_time - ExperimentStartTime
                    rtf_file.write(f"{adjusted_time:.2f};{incremental_rtf:.2f}\n")

                last_sim_time = current_sim_time
                last_real_time = current_real_time

        # SUMO Data Collection
        start_time = time.perf_counter()
        vehicle_data, speed_f1, position_f1 = sumoDataCollection()
        vehicle_data_json = json.dumps({"type":"vehicles", "vehicles": vehicle_data}, separators=(',', ':'))


        SumoDataCollectionTime = time.perf_counter() - start_time
        profiling_times['SumoDataCollectionTime'].append(SumoDataCollectionTime)

        # only update/send TLs every 1 s
        if sim_t - last_tl_time >= TL_UPDATE_INTERVAL:
            tl_list = collectTrafficLights()
            pub_socket.send_string(json.dumps({
                "type":   "trafficlights",
                "lights": tl_list
            }, separators=(",",":")))
            last_tl_time = sim_t

        number_of_cars_in_context = len([v for v in vehicle_data if v['vehicle_id'] != 'f_0.0'])
        context_car_count_values.append(number_of_cars_in_context)
        cumulative_context_car_counts.append(number_of_cars_in_context)

        # Socket Send
        start_time = time.perf_counter()
        pub_socket.send_string(vehicle_data_json)
        sent_messages_count += 1
        sent_bytes_count += len(vehicle_data_json.encode('utf-8'))
        total_sent_messages_count += 1
        total_sent_bytes_count += len(vehicle_data_json.encode('utf-8'))
        SocketSendTime = time.perf_counter() - start_time
        profiling_times['SocketSendTime'].append(SocketSendTime)

        send_time = time.perf_counter()
        if last_send_time is not None:
            interval = send_time - last_send_time
            profiling_times['IntervalBetweenSends'].append(interval)
            send_intervals.append(interval)
        last_send_time = send_time

        start_time = time.perf_counter()

        # Record simulation speed for f_1.0
        if speed_f1 is not None:
            speed_values.append(speed_f1)
            overall_simulation_speeds.append(speed_f1)

        current_car_count = len(traci.vehicle.getIDList())
        car_count_values.append(current_car_count)
        cumulative_car_counts.append(current_car_count)

        # Determine which cars are in context
        context_cars = [v["vehicle_id"] for v in vehicle_data if v["vehicle_id"] != "f_0.0"]

        # Only record simulation speeds for context cars after cutoff time if enabled
        if COMPUTE_CONTEXT_CAR_SIM_SPEEDS:
            CUTOFF_TIME = ExperimentStartTime
            vehicles_list = traci.vehicle.getIDList()
            step_all_simulation_speeds = []
            for vid in vehicles_list:
                if vid in context_cars:
                    vspeed = traci.vehicle.getSpeed(vid)
                    step_all_simulation_speeds.append(vspeed)
            if current_sim_time >= CUTOFF_TIME:
                all_simulation_speeds_list.extend(step_all_simulation_speeds)

        current_real_world_time = send_time

        # Compute real-world speeds for context cars if enabled
        for vdata in vehicle_data:
            vid = vdata['vehicle_id']
            if 'position' in vdata:
                x, y, _ = vdata['position']
                if vid in last_positions:
                    prev_x, prev_y, prev_sim_t, prev_real_t = last_positions[vid]
                    delta_x = x - prev_x
                    delta_y = y - prev_y
                    delta_s = math.hypot(delta_x, delta_y)
                    dt_real = (current_real_world_time - prev_real_t)

                    if dt_real > 0:
                        raw_v_real = delta_s / dt_real

                        if vid != 'f_1.0':
                            if vid not in real_world_speed_history:
                                real_world_speed_history[vid] = []
                            real_world_speed_history[vid].append(raw_v_real)

                            if len(real_world_speed_history[vid]) > WINDOW_SIZE:
                                smoothed_v_real = statistics.mean(real_world_speed_history[vid][-WINDOW_SIZE:])
                            else:
                                smoothed_v_real = statistics.mean(real_world_speed_history[vid])

                            if COMPUTE_CONTEXT_CAR_REAL_WORLD_SPEEDS and current_sim_time >= ExperimentStartTime and vid in context_cars:
                                all_real_world_speeds_list.append(smoothed_v_real)

                last_positions[vid] = (x, y, current_sim_time, current_real_world_time)

        # f_1.0 Real-world speed calculation if enabled
        if position_f1 is not None and COMPUTE_F1_SPEEDS:
            data_point = {
                'simulation_time': current_sim_time,
                'real_world_time': send_time,
                'position': position_f1,
                'speed_simulation': speed_f1
            }
            data_points.append(data_point)

            if len(data_points) > 1:
                prev_point = data_points[-2]
                curr_point = data_points[-1]

                delta_x = curr_point['position'][0] - prev_point['position'][0]
                delta_y = curr_point['position'][1] - prev_point['position'][1]
                delta_s = math.hypot(delta_x, delta_y)

                dt_real = curr_point['real_world_time'] - prev_point['real_world_time']
                v_real = delta_s / dt_real if dt_real > 0 else None

                if v_real is not None:
                    real_world_speed_values.append(v_real)
                    if len(real_world_speed_values) > WINDOW_SIZE:
                        smoothed_v_real = statistics.mean(real_world_speed_values[-WINDOW_SIZE:])
                    else:
                        smoothed_v_real = statistics.mean(real_world_speed_values)

                    overall_real_world_speeds.append(smoothed_v_real)

        DataProcessingTime = time.perf_counter() - start_time
        profiling_times['DataProcessingTime'].append(DataProcessingTime)

        total_loop_time = time.perf_counter() - loop_start_time
        profiling_times['TotalLoopTime'].append(total_loop_time)

        simulation_second = int(current_sim_time)
        if simulation_second != current_second:
            # Only compute and log RTF if enabled and we have started measurement
            # Also ensure current_sim_time > FromTime so the first logged RTF isn't zero

            current_second = simulation_second
            send_intervals = []
            speed_values = []
            car_count_values = []
            context_car_count_values = []
            sent_messages_count = 0
            received_messages_count = 0
            sent_bytes_count = 0
            received_bytes_count = 0

        current_time = time.perf_counter()
        sleep_duration = next_step_time - current_time

        if sleep_duration > 0:
            precise_sleep(sleep_duration)
        else:
            #logger.warning(f"Loop is running behind by {-sleep_duration:.6f} seconds")
            pass

        next_step_time += SIM_STEP

    # Final Statistics for f_1.0 if enabled
    if COMPUTE_F1_SPEEDS and overall_real_world_speeds:
        filtered_simulation_speeds = [
            dp['speed_simulation'] for dp in data_points if dp['simulation_time'] >= ExperimentStartTime
        ]

        filtered_real_world_speeds = [
            rw_speed for dp, rw_speed in zip(data_points, overall_real_world_speeds)
            if dp['simulation_time'] >= ExperimentStartTime
        ]

        if filtered_simulation_speeds:
            overall_avg_simulation_speed = statistics.mean(filtered_simulation_speeds)
            overall_std_simulation_speed = statistics.stdev(filtered_simulation_speeds) if len(filtered_simulation_speeds) > 1 else 0

            logger.info("\n--- Final Statistics (Excluding First 900 Seconds) ---")
            logger.info(f"Overall Average Simulation Speed of 'f_1.0': {overall_avg_simulation_speed:.2f} m/s")
            logger.info(f"Overall STD Simulation Speed of 'f_1.0': {overall_std_simulation_speed:.2f} m/s")
        else:
            logger.warning("No simulation speed data points after the cutoff time for 'f_1.0'.")

        if filtered_real_world_speeds:
            overall_avg_real_world_speed = statistics.mean(filtered_real_world_speeds)
            overall_std_real_world_speed = statistics.stdev(filtered_real_world_speeds) if len(filtered_real_world_speeds) > 1 else 0
            logger.info(f"Overall Average Real-World Speed of 'f_1.0': {overall_avg_real_world_speed:.2f} m/s")
            logger.info(f"Overall STD Real-World Speed of 'f_1.0': {overall_std_real_world_speed:.2f} m/s")
        else:
            logger.warning("No real-world speed data points after the cutoff time for 'f_1.0'.")

    # Final Statistics for context cars only if enabled
    if COMPUTE_CONTEXT_CAR_SIM_SPEEDS:
        if all_simulation_speeds_list:
            avg_all_sim_speeds = statistics.mean(all_simulation_speeds_list)
            std_all_sim_speeds = statistics.stdev(all_simulation_speeds_list) if len(all_simulation_speeds_list) > 1 else 0
            logger.info(f"Overall Average Simulation Speed of Context Cars: {avg_all_sim_speeds:.2f} m/s")
            logger.info(f"Overall STD Simulation Speed of Context Cars: {std_all_sim_speeds:.2f} m/s")
        else:
            logger.warning("No simulation speed data for context cars after cutoff.")

    if COMPUTE_CONTEXT_CAR_REAL_WORLD_SPEEDS:
        if all_real_world_speeds_list:
            avg_all_real_speeds = statistics.mean(all_real_world_speeds_list)
            std_all_real_speeds = statistics.stdev(all_real_world_speeds_list) if len(all_real_world_speeds_list) > 1 else 0
            logger.info(f"Overall Average Real-World Speed of Context Cars: {avg_all_real_speeds:.2f} m/s")
            logger.info(f"Overall STD Real-World Speed of Context Cars: {std_all_real_speeds:.2f} m/s")
        else:
            logger.warning("No real-world speed data for context cars after cutoff.")

    if COMPUTE_ALL_CARS:
        if cumulative_car_counts:
            overall_avg_car_count = statistics.mean(cumulative_car_counts)
            overall_std_car_count = statistics.stdev(cumulative_car_counts) if len(cumulative_car_counts) > 1 else 0
            logger.info(f"Average Number of All Cars: {overall_avg_car_count:.2f}")
            logger.info(f"STD Number of All Cars: {overall_std_car_count:.2f}")

    num_loops = len(profiling_times['TotalLoopTime'])
    if COMPUTE_PROFILING_SUMMARY and num_loops > 0:
        any_profiling_info = (COMPUTE_PROFILING_TIMES or COMPUTE_CARS_IN_CONTEXT_STATS or COMPUTE_MSGS_BYTES_STATS)
        if any_profiling_info:
            logger.info("\n--- Profiling Summary ---")
            if COMPUTE_PROFILING_TIMES:
                for key in profiling_times:
                    if profiling_times[key]:
                        avg = sum(profiling_times[key]) / len(profiling_times[key])
                        std = statistics.stdev(profiling_times[key]) if len(profiling_times[key]) > 1 else 0
                        logger.info(f"Average {key}: {avg:.6f} seconds (STD: {std:.6f})")

            total_simulation_time = traci.simulation.getTime()
            avg_sent_messages_per_second = total_sent_messages_count / total_simulation_time if total_simulation_time > 0 else 0
            avg_sent_bytes_per_second = total_sent_bytes_count / total_simulation_time if total_simulation_time > 0 else 0
            avg_received_messages_per_second = total_received_messages_count / total_simulation_time if total_simulation_time > 0 else 0
            avg_received_bytes_per_second = total_received_bytes_count / total_simulation_time if total_simulation_time > 0 else 0

            if COMPUTE_CARS_IN_CONTEXT_STATS and cumulative_context_car_counts:
                overall_avg_context_car_count = statistics.mean(cumulative_context_car_counts)
                overall_std_context_car_count = statistics.stdev(cumulative_context_car_counts) if len(cumulative_context_car_counts) > 1 else 0
                logger.info(f"Average Number of Cars in Context: {overall_avg_context_car_count:.2f}")
                logger.info(f"STD Number of Cars in Context: {overall_std_context_car_count:.2f}")

            if COMPUTE_MSGS_BYTES_STATS:
                logger.info(f"Average Messages Sent to Unity: {avg_sent_messages_per_second:.2f} messages/s")
                logger.info(f"Average Bytes Sent to Unity: {avg_sent_bytes_per_second:.2f} B/s")
                logger.info(f"Average Messages Received from Unity: {avg_received_messages_per_second:.2f} messages/s")
                logger.info(f"Average Bytes Received from Unity: {avg_received_bytes_per_second:.2f} B/s")

except KeyboardInterrupt:
    logger.info("Simulation interrupted by user.")
except Exception as e:
    logger.error(f"An unexpected error occurred: {e}", exc_info=True)
finally:
    if COMPUTE_RTF:
        # Compute the Real-Time Factor (RTF) at the end of the run, considering only from FromTime
        end_wall_time = time.perf_counter()
        end_simulation_time = traci.simulation.getTime()
        
        if start_simulation_time is not None and start_wall_time is not None:
            total_simulated_duration = end_simulation_time - start_simulation_time
            total_real_duration = end_wall_time - start_wall_time
            
            if total_real_duration > 0:
                rtf = total_simulated_duration / total_real_duration
                logger.info(f"Real-Time Factor (RTF) from {ExperimentStartTime}s to end of run: {rtf:.2f}")
            else:
                logger.warning("Real-world duration is zero or negative, cannot compute RTF.")
        else:
            logger.warning("Did not record start times for RTF measurement.")
    

    # Send STOP_RECORDING if we started recording before
    if startRecordingSent:
        stop_message = json.dumps({"type":"command", "command":"STOP_RECORDING"})
        pub_socket.send_string(stop_message)
        logger.info("Sent STOP_RECORDING command to Unity")

    rtf_file.close()
    traci.close()
    pub_socket.close()
    router_socket.close()
    context_zmq.term()
    logger.info("TraCI connection closed.")