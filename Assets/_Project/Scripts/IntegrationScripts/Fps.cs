using UnityEngine;
using System;
using System.IO;

/// <summary>
///   FPS recorder & on‑screen display.
///   Writes FPS_Report.txt into SUMO2Unity\SUMOData next to the Unity project.
/// </summary>
public class Fps : MonoBehaviour
{
    // ────────────────────────────────────────────────────────────────  FPS fields
    private float currentFps;
    private float smoothedFps;
    private float smoothingFactor = 0.1f;       // weight of recent frames
    [SerializeField] private int fontSize = 25;          // GUI font size

    // ────────────────────────────────────────────────────────────  logging fields
    private float logInterval;             // set from SimulationController
    private string filePath;                // full path to FPS_Report.txt
    private float timeAccum;               // time since last log

    private bool firstFpsTimestampLogged;
    private float firstFpsLoggedTime;

    // ───────────────────────────────────────────────────────────────  GUI fields
    private float latestSmoothedFps;
    private float displayedFps;
    private const float guiUpdateInterval = 0.5f;
    private float guiTimer;

    // ────────────────────────────────────────────────────────────  references
    private ExchangeData _ExchangeData;
    private SimulationController simController;

    // ─────────────────────────────────────────────────────────  helper: find path
    /// <summary>Walks up from Assets until it finds SUMO2UnityPY\SUMOData.</summary>
    /// <summary>Finds (or creates) SUMO2Unity\Results next to the project.</summary>
    private static string LocateOrCreateResultsFolder()
    {
        // projectRoot = folder that *contains* "Assets"
        string projectRoot = Directory.GetParent(Application.dataPath).FullName;
        DirectoryInfo dir = new DirectoryInfo(projectRoot);

        while (dir != null)
        {
            string candidate = Path.Combine(dir.FullName, "Results");
            if (Directory.Exists(candidate))
                return candidate;

            dir = dir.Parent;                       // walk upward
        }

        // Not found – create it next to the project
        string fallback = Path.Combine(projectRoot, "Results");
        Directory.CreateDirectory(fallback);
        return fallback;
    }


    // ────────────────────────────────────────────────────────────────────────────
    private void Start()
    {
        // --------------------------------------------------------  file location
        string sumoDataDir = LocateOrCreateResultsFolder();
        filePath = Path.Combine(sumoDataDir, "FPS_Report.txt");
        File.WriteAllText(filePath, "unity_time;FPS\n");

        // --------------------------------------------------------  other setup
        _ExchangeData = GetComponent<ExchangeData>() ?? gameObject.AddComponent<ExchangeData>();


        // --- get the SimulationController in the scene ----------------
        SimulationController sim = FindObjectOfType<SimulationController>();
        if (sim == null)
        {
            Debug.LogError("SimulationController not found!");
            return;
        }

        logInterval = sim.unityStepLength;   // value you set in Inspector


        displayedFps = 0f;           // avoid showing 0 initially
        GUI.depth = 2;
    }

    // ────────────────────────────────────────────────────────────────────────────
    private void Update()
    {
        currentFps = 1f / Time.unscaledDeltaTime;
        smoothedFps = (smoothingFactor * currentFps) + ((1f - smoothingFactor) * smoothedFps);

        // Reset smoothing whenever recording starts afresh
        if (RecordingManager.startRecordingFromZero && firstFpsTimestampLogged == false)
            smoothedFps = currentFps;

        latestSmoothedFps = smoothedFps;

        // Slow the GUI refresh rate a little
        guiTimer += Time.deltaTime;
        if (guiTimer >= guiUpdateInterval)
        {
            displayedFps = smoothedFps;
            guiTimer = 0f;
        }
    }

    // ────────────────────────────────────────────────────────────────────────────
    private void FixedUpdate()
    {
        timeAccum += Time.fixedDeltaTime;

        if (timeAccum >= logInterval - 0.002f)
        {
            if (RecordingManager.startRecordingFromZero)
                LogFpsToFile();

            timeAccum = 0f;
        }
    }

    // ────────────────────────────────────────────────────────────────────────────
    private void LogFpsToFile()
    {
        if (!RecordingManager.startRecordingFromZero)
            return;

        if (!firstFpsTimestampLogged)
        {
            firstFpsLoggedTime = Time.time;
            firstFpsTimestampLogged = true;
        }

        float offsetTime = Time.time - firstFpsLoggedTime;
        string logEntry = $"{offsetTime:F3};{latestSmoothedFps:F2}";
        File.AppendAllText(filePath, logEntry + "\n");
    }

    // ────────────────────────────────────────────────────────────────────────────
    private void OnGUI()
    {
        GUIStyle style = new GUIStyle
        {
            fontSize = fontSize,
            normal = { textColor = Color.black }
        };

        GUI.Label(new Rect(5, 40, 200, 25), "FPS: " + Mathf.Round(displayedFps), style);
    }
}