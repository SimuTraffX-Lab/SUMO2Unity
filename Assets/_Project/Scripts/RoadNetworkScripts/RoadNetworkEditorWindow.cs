// ============================== 
// RoadNetworkEditorWindow.cs
// (full version incl. 2‑slide banners for windows 1–3)
// ==============================
using System;
using System.IO;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86;

internal static class Sumo2UnityGuiConsts
{
    public const float WindowWidth = 936f;
    public const float WindowHeight = 585f;
    public const float BannerWidth = 910f;
    public const float BannerHeight = 286f;

    public static void DrawBanner(Texture2D tex)
    {
        if (!tex) return;

        Rect r = GUILayoutUtility.GetRect(BannerWidth, BannerHeight,
                                          GUILayout.ExpandWidth(false),
                                          GUILayout.ExpandHeight(false));
        float xOffset = (WindowWidth - BannerWidth) * 0.5f;
        r.x = xOffset;
        GUI.DrawTexture(r, tex, ScaleMode.ScaleToFit);
    }
}

/// <summary>
/// Simple helper to draw & auto‑advance a banner slideshow in EditorWindows.
/// </summary>
internal static class BannerSlideHelper
{
    public static void DrawSlide(Texture2D[] slides,
                                 ref int current,
                                 ref double lastSwap,
                                 float intervalSeconds)
    {
        if (slides == null || slides.Length == 0 || slides[current] == null) return;

        // Auto‑advance every intervalSeconds
        double now = EditorApplication.timeSinceStartup;
        if (slides.Length > 1 && now - lastSwap > intervalSeconds)
        {
            current = (current + 1) % slides.Length;
            lastSwap = now;
        }

        // Draw the current texture centred
        Rect r = GUILayoutUtility.GetRect(Sumo2UnityGuiConsts.BannerWidth,
                                          Sumo2UnityGuiConsts.BannerHeight,
                                          GUILayout.ExpandWidth(false),
                                          GUILayout.ExpandHeight(false));
        float xOffset = (Sumo2UnityGuiConsts.WindowWidth - Sumo2UnityGuiConsts.BannerWidth) * 0.5f;
        r.x = xOffset;
        GUI.DrawTexture(r, slides[current], ScaleMode.ScaleToFit);

        // Manual controls (optional)
        using (new GUILayout.HorizontalScope())
        {
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("◀", GUILayout.Width(24)))
            {
                current = (current - 1 + slides.Length) % slides.Length;
                lastSwap = now;
            }
            if (GUILayout.Button("▶", GUILayout.Width(24)))
            {
                current = (current + 1) % slides.Length;
                lastSwap = now;
            }
            GUILayout.FlexibleSpace();
        }
    }
}

// ───────────────────────────────────────────────────────────────  Window 1
public class RoadNetworkEditorWindow : EditorWindow
{
    private static string sumoXmlFolderPath;

    // slideshow fields
    private Texture2D[] demoSlides;
    private int slideIndex;
    private double lastSlideSwap;
    private const float slideInterval = 3f;

    private static string LocateSumoDataFolder()
    {
        string projectRoot = Directory.GetParent(Application.dataPath).FullName;
        DirectoryInfo dir = new DirectoryInfo(projectRoot);

        while (dir != null)
        {
            string candidate = Path.Combine(dir.FullName, "Sumo2Unity", "Scenario1");
            if (Directory.Exists(candidate)) return candidate;
            dir = dir.Parent;
        }
        return Path.Combine(Application.dataPath, "SumoFiles");
    }

    [MenuItem("Sumo2Unity/1. Create Road Network")]
    public static void OpenWindow()
    {
        RoadNetworkEditorWindow w = GetWindow<RoadNetworkEditorWindow>("Sumo2Unity – Road Network");
        w.minSize = new Vector2(Sumo2UnityGuiConsts.WindowWidth, Sumo2UnityGuiConsts.WindowHeight);
        w.maxSize = w.minSize;
        sumoXmlFolderPath = LocateSumoDataFolder();
    }

    private void OnEnable()
    {
        demoSlides = new[]
        {
            AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/_Project/Icons/1.CreateRoadNetwork.png"),
            AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/_Project/Icons/1.CreateRoadNetwork_B.png")
        };
        slideIndex = 0;
        lastSlideSwap = EditorApplication.timeSinceStartup;
    }

    private void OnGUI()
    {
        BannerSlideHelper.DrawSlide(demoSlides, ref slideIndex, ref lastSlideSwap, slideInterval);
        GUILayout.Space(10);

        GUILayout.Label("Import Sumo Files and Generate Network", EditorStyles.boldLabel);
        GUILayout.Space(5);

        sumoXmlFolderPath = EditorGUILayout.TextField(
            new GUIContent("Sumo Files Folder",
            "Directory containing Sumo .xml files (e.g., map.net.xml)."),
            sumoXmlFolderPath);

        if (GUILayout.Button("Select Sumo Files Folder"))
        {
            string chosen = EditorUtility.OpenFolderPanel("Choose the folder", sumoXmlFolderPath, "");
            if (!string.IsNullOrEmpty(chosen)) sumoXmlFolderPath = chosen;
        }

        GUILayout.Space(15);

        if (GUILayout.Button("Start"))
        {
            try
            {
                GameObject cam = GameObject.Find("Main Camera");
                if (cam) cam.SetActive(false);
            }
            catch (Exception ex) { Debug.LogError(ex); }

            // Ensure we have a fresh builder instance
            RoadNetworkBuilder builder = FindFirstObjectByType<RoadNetworkBuilder>();
            if (builder == null)
                builder = new GameObject("RoadNetworkBuilder").AddComponent<RoadNetworkBuilder>();

            builder.InitializeInEditMode();

            EditorUtility.DisplayProgressBar("Generation Progress", "Loading Sumo XML Files", 0f);
            builder.LoadSumoXmlFiles(sumoXmlFolderPath);

            EditorUtility.DisplayProgressBar("Generation Progress", "Generating Road Network", 0.2f);
            builder.GenerateRoadsAndJunctions();

            EditorUtility.ClearProgressBar();
            Close();
        }
    }

    private void OnInspectorUpdate() => Repaint();
}

// ───────────────────────────────────────────────────────────────  Window 2
public class Sumo2UnityIntegrationWindow : EditorWindow
{
    private Texture2D[] demoSlides;
    private int slideIndex;
    private double lastSlideSwap;
    private const float slideInterval = 3f;

    // custom styles
    private GUIStyle headerStyle;
    private GUIStyle helpStyle;

    [MenuItem("Sumo2Unity/2. Run Sumo2Unity Integration")]
    public static void OpenWindow()
    {
        Sumo2UnityIntegrationWindow w = GetWindow<Sumo2UnityIntegrationWindow>("Sumo2Unity – Integration");
        w.minSize = new Vector2(Sumo2UnityGuiConsts.WindowWidth, Sumo2UnityGuiConsts.WindowHeight);
        w.maxSize = w.minSize;
    }

    private void OnEnable()
    {
        demoSlides = new[]
        {
            AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/_Project/Icons/2.Integration.png"),
            AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/_Project/Icons/2.Integration_B.png")
        };
        slideIndex = 0;
        lastSlideSwap = EditorApplication.timeSinceStartup;

        // build styles once
        headerStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 16,
            alignment = TextAnchor.MiddleCenter
        };

        helpStyle = new GUIStyle(EditorStyles.helpBox)
        {
            fontSize = 13,
            richText = true,
            wordWrap = true
        };
    }

    private void OnGUI()
    {
        BannerSlideHelper.DrawSlide(demoSlides, ref slideIndex, ref lastSlideSwap, slideInterval);
        GUILayout.Space(10);

        GUILayout.Label("Run Sumo‑to‑Unity Integration", headerStyle);
        GUILayout.Space(5);

        EditorGUILayout.LabelField(
            "<b>Instructions:</b>\n\n" +
            "1. Go To Scenarios Folder\n" +
            "2. Run <i>Sumo2UnityTools.exe</i>.\n" +
            "3. Select Parameters and Start Simulation.\n" +
            "4. Wait until <b>IntegrationStartTime</b> (e.g., 540 sec)\n" +
            "5. Click <b>Play</b> in Unity to start streaming vehicles / signals.\n" +
            "6. Press <b>Stop</b> to end the session.",
            helpStyle);
    }

    private void OnInspectorUpdate() => Repaint();
}

// ───────────────────────────────────────────────────────────────  Window 3
public class PerformanceFunctionsWindow : EditorWindow
{
    private Texture2D[] demoSlides;
    private int slideIndex;
    private double lastSlideSwap;
    private const float slideInterval = 3f;

    // custom styles
    private GUIStyle headerStyle;
    private GUIStyle helpStyle;

    [MenuItem("Sumo2Unity/3. Performance Functions")]
    public static void OpenWindow()
    {
        PerformanceFunctionsWindow w = GetWindow<PerformanceFunctionsWindow>("Sumo2Unity – Performance");
        w.minSize = new Vector2(Sumo2UnityGuiConsts.WindowWidth, Sumo2UnityGuiConsts.WindowHeight);
        w.maxSize = w.minSize;
    }

    private void OnEnable()
    {
        demoSlides = new[]
        {
            AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/_Project/Icons/3.PerformanceFunctions.png"),
            AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/_Project/Icons/3.PerformanceFunctions_B.png")
        };
        slideIndex = 0;
        lastSlideSwap = EditorApplication.timeSinceStartup;

        headerStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 16,
            alignment = TextAnchor.MiddleCenter
        };

        helpStyle = new GUIStyle(EditorStyles.helpBox)
        {
            fontSize = 13,
            richText = true,
            wordWrap = true
        };
    }

    private void OnGUI()
    {
        BannerSlideHelper.DrawSlide(demoSlides, ref slideIndex, ref lastSlideSwap, slideInterval);
        GUILayout.Space(10);

        GUILayout.Label("Performance Functions", headerStyle);
        GUILayout.Space(5);

        EditorGUILayout.LabelField(
            "<b>Performance Functions:</b>\n\n" +
            "• <b>FPS Monitor Result:</b>  Results/FPS_Report.txt\n" +
            "• <b>RTF Monitor Result:</b>  Results/rtf_report.txt\n" +
            "• <b>Vehicle Trajectories:</b>  Results/vehicle_data_report.txt",
            helpStyle);
    }

    private void OnInspectorUpdate() => Repaint();
}
