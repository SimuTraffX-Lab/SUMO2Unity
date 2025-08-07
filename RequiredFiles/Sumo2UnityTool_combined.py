# ────────────────────────────────────────────────────────────────
#  Sumo2UnityTool_combined.py
#  GUI  +  SUMO ⇆ Unity simulation  (one file)
#  Version : Sumo2Unity v2.0.0
#  Author  : Ahmad Mohammadi, PhD – York University
#  License : MIT
# ────────────────────────────────────────────────────────────────
import os, sys, json, math, time, queue, threading, statistics
import tkinter as tk, webbrowser
from tkinter import ttk, messagebox
from PIL import Image, ImageTk   # pip install pillow
import zmq, logging              # pip install pyzmq

# ════════════════════════════════════════════════════════════════
#  DEFAULTS (shared by GUI & simulation)
# ════════════════════════════════════════════════════════════════
DEFAULTS = {
    "IntegrationStartTime": 540,
    "ExperimentStartTime" : 600,
    "ExperimentEndTime"   : 720,
    "steplength"          : 0.1,
    "lateral_resolution"  : 0.3,
    "zoom"                : 150.0,   # (bigger value → closer)
}
VERSION      = "Sumo2Unity v2.0.0"
LINKEDIN_URL = "https://www.linkedin.com/in/ahmadmohammadi1441/"

# ═════════ helper to reach packaged resources ═══════════════════
def resource_path(fname: str) -> str:
    if getattr(sys, "frozen", False):
        return os.path.join(sys._MEIPASS, fname)
    return os.path.join(os.path.abspath(os.path.dirname(__file__)), fname)

# ═════════════════ GUI  SET-UP ══════════════════════════════════
root = tk.Tk(); root.title("Sumo2Unity Tool"); root.resizable(True, True)

def load_resized(path: str, target_w: int) -> ImageTk.PhotoImage:
    img = Image.open(resource_path(path))
    r   = img.height / img.width
    return ImageTk.PhotoImage(img.resize((target_w, int(target_w*r))))

IMG_W = 600
banner_imgs = [load_resized("2.Integration.JPG", IMG_W),
               load_resized("2.Integration_B.JPG", IMG_W)]
banner_lbl  = tk.Label(root, image=banner_imgs[0])
banner_lbl.grid(row=0, column=0, columnspan=4, pady=(6, 12))
def swap(idx=[0]):
    idx[0] = (idx[0] + 1) % len(banner_imgs)
    banner_lbl.configure(image=banner_imgs[idx[0]]); root.after(2000, swap)
root.after(2000, swap)

root.columnconfigure(1, weight=1)
entries, row = {}, 1
for k, v in DEFAULTS.items():
    label_text = "zoom (bigger value → closer)" if k == "zoom" else k
    ttk.Label(root, text=label_text).grid(row=row, column=0,
                                          sticky="e", padx=6, pady=3)
    e = ttk.Entry(root); e.insert(0, str(v))
    e.grid(row=row, column=1, sticky="we", padx=6, pady=3)
    entries[k] = e; row += 1

# ── NEW OPTIONS ────────────────────────────────────────────────
use_gui_var  = tk.BooleanVar(value=True)
rtf_var      = tk.BooleanVar(value=True)

ttk.Checkbutton(root, text="Run SUMO with GUI",  variable=use_gui_var)\
   .grid(row=row, column=0, columnspan=2, sticky="w", padx=6, pady=3); row += 1
ttk.Checkbutton(root, text="Calculate RTF",      variable=rtf_var)\
   .grid(row=row, column=0, columnspan=2, sticky="w", padx=6, pady=3); row += 1
# ────────────────────────────────────────────────────────────────

def center(win):
    win.update_idletasks()
    w, h = win.winfo_width(), win.winfo_height()
    x = (win.winfo_screenwidth()-w)//2
    y = (win.winfo_screenheight()-h)//2
    win.geometry(f"{w}x{h}+{x}+{y}")

def pop_img(title, img_path, w):
    pop = tk.Toplevel(root); pop.title(title); pop.resizable(False, False)
    im  = load_resized(img_path, w); tk.Label(pop, image=im).pack()
    pop.im = im; ttk.Button(pop, text="Close", command=pop.destroy).pack(pady=6)
    center(pop)

def show_help():    pop_img("Help", "Help.JPG", 874)
def show_contact():
    pop = tk.Toplevel(root); pop.title("Contact / License")
    t   = tk.Text(pop, wrap="word", width=80, height=24)
    t.insert("1.0", f"{VERSION}\n\nContact: Ahmad Mohammadi\nLinkedIn: {LINKEDIN_URL}\n\nMIT License — see repository")
    t.config(state="disabled"); t.pack(expand=True, fill="both"); center(pop)
def show_pubs():
    pop = tk.Toplevel(root); pop.title("Publications")
    txt = tk.Text(pop, wrap="word", width=100, height=18)
    pubs = """\
1. Mohammadi, A., Park, P. Y., Nourinejad, M., Cherakkatil, M. S. B., & Park, H. S. (2024, June).
   SUMO2Unity: An Open-Source Traffic Co-Simulation Tool to Improve Road Safety.
   In 2024 IEEE Intelligent Vehicles Symposium (IV) (pp. 2523-2528). IEEE.

2. Mohammadi, A., Park, P. Y., Nourinejad, M., & Cherakkatil, M. S. B. (2025, May).
   Development of a Virtual Reality Traffic Simulation to Analyze Road User Behavior.
   In 2025 7th International Congress on Human-Computer Interaction, Optimization
   and Robotic Applications (ICHORA) (pp. 1-5). IEEE.
"""
    txt.insert("1.0", pubs); txt.config(state="disabled")
    txt.pack(expand=True, fill="both", padx=8, pady=8); center(pop)
# ═════════════════ SIMULATION (run_sim) ═════════════════════════
def run_sim(cfg: dict):
    import traci
    from traci.constants import VAR_POSITION3D, VAR_ANGLE, VAR_TYPE

    # ---------- apply GUI parameters ----------
    IntegrationStartTime = cfg["IntegrationStartTime"]
    ExperimentStartTime  = cfg["ExperimentStartTime"]
    ExperimentEndTime    = cfg["ExperimentEndTime"]
    steplength           = cfg["steplength"]
    lateral_resolution   = cfg["lateral_resolution"]
    zoom_level           = cfg["zoom"]          # ← new
    use_gui              = cfg["use_gui"]
    calc_rtf             = cfg["calc_rtf"]

    # ---------- logging ----------
    logging.basicConfig(level=logging.INFO,
                        format="%(asctime)s - %(levelname)s - %(message)s")
    logger = logging.getLogger(__name__)

    # ---------- SUMO paths ----------
    if 'SUMO_HOME' not in os.environ:
        sys.exit("Set SUMO_HOME env variable.")
    sys.path.append(os.path.join(os.environ['SUMO_HOME'], 'tools'))

    base_dir = os.path.dirname(sys.executable) if getattr(sys,"frozen",False) \
               else os.path.dirname(__file__)
    sumo_cfg = os.path.join(base_dir, "Sumo2Unity.sumocfg")
    sumo_bin = "sumo-gui" if use_gui else "sumo"
    sumo_cmd = [sumo_bin,"-c",sumo_cfg,"--step-length",str(steplength),
                "--lateral-resolution",str(lateral_resolution)]
    if use_gui:
        sumo_cmd += ["--delay","0"]   # keep 0-delay only when GUI present

    # ---------- connect TraCI ----------
    traci.start(sumo_cmd)

    # ---------- gui camera helper ----------
    ego = "f_0.0"

    if use_gui:
        view_id = "View #0"
        traci.gui.trackVehicle(view_id, ego)
        traci.gui.setSchema(view_id, "real world")

    # ★ updated helper uses GUI-selected zoom
    def cam_follow(view_id, veh_id):
        try:
            traci.gui.trackVehicle(view_id, veh_id)
            traci.gui.setZoom(view_id, zoom_level)
        except traci.TraCIException:
            pass
    # ------------------------------------------------------------

    # ---------- TraCI context subscription ----------
    traci.vehicle.subscribeContext(
        ego,
        traci.constants.CMD_GET_VEHICLE_VARIABLE,
        250,
        [VAR_POSITION3D, VAR_ANGLE, VAR_TYPE]
    )
    # ---------- ZMQ sockets ----------
    ctx  = zmq.Context()
    pub  = ctx.socket(zmq.PUB);    pub.bind("tcp://*:5556")
    rout = ctx.socket(zmq.ROUTER); rout.bind("tcp://*:5557")

    # ---------- background Unity RX ----------
    u_q = queue.Queue()
    def rx_unity():
        while True:
            try:
                _ident, msg = rout.recv_multipart()
                u_q.put(json.loads(msg.decode()))
            except Exception: logger.exception("Unity RX")
    threading.Thread(target=rx_unity, daemon=True).start()

    # ---------- helpers ----------
    WINDOW = 10
    last_pos, last_pos_z, rw_hist = {}, {}, {}
    prof = {k: [] for k in ("Unity","Step","Collect","Send","DataProc","Total")}
    def sleep_precise(d):
        t0 = time.perf_counter()
        while (rem := d - (time.perf_counter()-t0)) > 0:
            if rem > 0.002: time.sleep(0.001)

    # ---------- results dir / RTF file ----------
    if calc_rtf:
        res_dir = os.path.join(os.path.abspath(os.path.join(base_dir, os.pardir)),
                               "Results")
        os.makedirs(res_dir, exist_ok=True)
        rtf_f = open(os.path.join(res_dir,"rtf_report.txt"),"w",encoding="utf-8")
        rtf_f.write("Time(s);RTF\n")
    else:
        rtf_f = None

    # ---------- containers ----------
    last_send = None;   current_sec = 0
    send_int, sim_speeds = [], []
    start_rec_sent = False
    start_sim_t = start_wall_t = None
    rtf_started  = False; last_sim, last_wall = 0, 0

    # ---------- warm-up ----------
    while traci.simulation.getTime() < IntegrationStartTime:
        traci.simulationStep(); cam_follow("View #0", ego) if use_gui else None

    # ---------- main loop ----------
    STEP = steplength
    next_step = time.perf_counter() + STEP
    TL_INT = 1.0; last_tl_t = 0.0

    try:
        while traci.simulation.getMinExpectedNumber() > 0 \
              and traci.simulation.getTime() < ExperimentEndTime:

            loop_t0 = time.perf_counter()
            sim_t   = traci.simulation.getTime()

            # ❶ Unity → SUMO positions
            t0 = time.perf_counter()
            while not u_q.empty():
                for v in u_q.get().get("vehicles", []):
                    if v["vehicle_id"] == ego:
                        traci.vehicle.moveToXY(ego,"",0,
                            float(v["position"][0]), float(v["position"][1]),
                            float(v["angle"]), keepRoute=2)
            prof["Unity"].append(time.perf_counter()-t0)

            # ❷ SUMO step
            t0 = time.perf_counter(); traci.simulationStep()
            prof["Step"].append(time.perf_counter()-t0)
            if use_gui: cam_follow("View #0", ego)

            # ❸ send START_RECORDING after warm-up (independent of RTF)
            if sim_t >= ExperimentStartTime and not start_rec_sent:
                pub.send_string(json.dumps(
                    {"type":"command","command":"START_RECORDING"}))
                start_rec_sent = True

            # ❹ initialise RTF after warm-up (only if enabled)
            if calc_rtf and (not rtf_started) and sim_t >= ExperimentStartTime:
                rtf_started = True
                start_sim_t  = sim_t
                start_wall_t = time.perf_counter()
                last_sim, last_wall = sim_t, start_wall_t

            # ❺ collect ego + context vehicles
            t0 = time.perf_counter()
            vlist = traci.vehicle.getIDList()
            vdata = []
            if ego in vlist:
                x,y,z   = traci.vehicle.getPosition3D(ego)
                ang     = traci.vehicle.getAngle(ego)
                vtype   = traci.vehicle.getTypeID(ego)
                vdata.append({"vehicle_id":ego,
                              "position":(round(x,2),round(y,2),round(z,2)),
                              "angle":round(ang,2),"type":vtype,
                              "timestamp":round(time.time(),2)})
                ctx_res = traci.vehicle.getContextSubscriptionResults(ego)
                if ctx_res:
                    for vid in ctx_res.keys():
                        if vid==ego: continue
                        x,y,z = traci.vehicle.getPosition3D(vid)
                        ang   = traci.vehicle.getAngle(vid)
                        vtype = traci.vehicle.getTypeID(vid)
                        vlong = traci.vehicle.getSpeed(vid)
                        vlat  = traci.vehicle.getLateralSpeed(vid)
                        if vid in last_pos_z:
                            pz,pt = last_pos_z[vid]; dt=sim_t-pt
                            vvert = (z-pz)/dt if dt>0 else 0.0
                        else: vvert=0.0
                        last_pos_z[vid]=(z,sim_t)
                        vdata.append({"vehicle_id":vid,
                                      "position":(round(x,3),round(y,3),round(z,3)),
                                      "angle":round(ang,3),"type":vtype,
                                      "long_speed":round(vlong,2),
                                      "vert_speed":round(vvert,3),
                                      "lat_speed":round(vlat,2)})
            vjson = json.dumps({"type":"vehicles","vehicles":vdata},
                               separators=(',',':'))
            prof["Collect"].append(time.perf_counter()-t0)

            # ❻ traffic lights once per second
            if sim_t-last_tl_t >= TL_INT:
                tls = [{"junction_id":tl,
                        "state":traci.trafficlight.getRedYellowGreenState(tl)}
                       for tl in traci.trafficlight.getIDList()]
                pub.send_string(json.dumps({"type":"trafficlights","lights":tls},
                                           separators=(',',':')))
                last_tl_t = sim_t

            # ❼ publish vehicles
            t0 = time.perf_counter(); pub.send_string(vjson)
            prof["Send"].append(time.perf_counter()-t0)

            # ❽ incremental RTF (if enabled)
            if calc_rtf and rtf_started and sim_t >= ExperimentStartTime:
                now = time.perf_counter()
                if sim_t == ExperimentStartTime:
                    rtf_f.write("0.00;0.00\n")
                else:
                    sim_d  = sim_t - last_sim
                    real_d = now   - last_wall
                    rtf_f.write(f"{sim_t-ExperimentStartTime:.2f};{sim_d/real_d:.2f}\n")
                last_sim, last_wall = sim_t, now

            # ❾ step pacing
            sleep_precise(max(0.0, next_step - time.perf_counter()))
            next_step += STEP
            prof["Total"].append(time.perf_counter()-loop_t0)

    except KeyboardInterrupt:
        logger.info("Interrupted by user.")
    finally:
        # overall RTF
        if calc_rtf and rtf_started:
            total_w   = time.perf_counter() - start_wall_t
            total_sim = traci.simulation.getTime() - start_sim_t
            if total_w>0: logger.info("RTF overall %.2f", total_sim/total_w)
        if start_rec_sent:
            pub.send_string(json.dumps({"type":"command","command":"STOP_RECORDING"}))
        if rtf_f: rtf_f.close()
        traci.close(); pub.close(); rout.close(); ctx.term()
        logger.info("Finished, connections closed.")

# ═════════════════════ GUI → START BTN ═════════════════════════
def start_clicked():
    try:
        cfg = {k: (int(v.get()) if "Time" in k else float(v.get()))
               for k,v in entries.items()}
        cfg["use_gui"] = bool(use_gui_var.get())
        cfg["calc_rtf"] = bool(rtf_var.get())
    except ValueError:
        messagebox.showerror("Invalid input","Please enter numeric values.")
        return
    root.destroy(); run_sim(cfg)

# buttons
ttk.Button(root,text="Help",command=show_help)        .grid(row=row,column=0,pady=12,padx=6,sticky="w")
ttk.Button(root,text="Contact / License",command=show_contact)\
                                                     .grid(row=row,column=1,pady=12,padx=6,sticky="w")
ttk.Button(root,text="Publications",command=show_pubs)\
                                                     .grid(row=row,column=2,pady=12,padx=6,sticky="w")
ttk.Button(root,text="Start simulation",command=start_clicked)\
                                                     .grid(row=row,column=3,pady=12,padx=6,sticky="e")

root.update_idletasks()
root.geometry("+{}+{}".format((root.winfo_screenwidth()-root.winfo_width())//2,
                              (root.winfo_screenheight()-root.winfo_height())//2))
root.mainloop()
