using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Diagnostics;
using TMPro;

// Add this using statement for the file browser
using SimpleFileBrowser;

public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject networkPanel;
    public GameObject simulationPanel;
    public TMP_InputField pathInputField;
    public TMP_Text statusText;
    public Slider progressBar;

    [Header("Core References")]
    public RoadNetworkBuilder roadNetworkBuilder; // Drag your RoadNetworkBuilder instance here

    // Stores the path selected by the user
    private string sumoFolderPath;

    void Start()
    {
        // Set the initial state of the UI
        networkPanel.SetActive(true);
        simulationPanel.SetActive(false);
        progressBar.gameObject.SetActive(false); // Hide the progress bar at the start
        statusText.text = "Please select a SUMO scenario folder.";
    }

    // This method will be linked to the "Browse..." button
    public void OnBrowseButton_Click()
    {
        if (FileBrowser.IsOpen)
        {
            return; // Don't open a new dialog if one is already open
        }

        // Set the browser to only show folders
        FileBrowser.SetFilters(false);
        StartCoroutine(ShowFolderSelectCoroutine());
    }

    private IEnumerator ShowFolderSelectCoroutine()
    {
        // Show the folder selection dialog and wait for the user to respond
        yield return FileBrowser.WaitForLoadDialog(
            FileBrowser.PickMode.Folders,
            false,
            null,
            null,
            "Select SUMO Scenario Folder",
            "Select");

        // Check if the user selected a folder or cancelled
        if (FileBrowser.Success)
        {
            sumoFolderPath = FileBrowser.Result[0];
            pathInputField.text = sumoFolderPath;
            statusText.text = "Folder selected. Ready to generate the world.";
        }
        else
        {
            statusText.text = "Folder selection was cancelled.";
        }
    }

    // This method will be linked to the "Generate 3D World" button
    public void OnGenerateButton_Click()
    {
        if (string.IsNullOrEmpty(sumoFolderPath) || roadNetworkBuilder == null)
        {
            statusText.text = "Error: Please select a valid SUMO folder first.";
            return;
        }

        StartCoroutine(GenerateNetworkCoroutine());
    }

    private IEnumerator GenerateNetworkCoroutine()
    {
        progressBar.gameObject.SetActive(true);

        statusText.text = "Starting generation...";
        progressBar.value = 0;
        yield return null; // Wait a frame for UI to update

        statusText.text = "Loading SUMO XML files...";
        progressBar.value = 0.1f;
        roadNetworkBuilder.LoadSumoXmlFiles(sumoFolderPath);
        yield return null;

        statusText.text = "Generating roads and junctions...";
        progressBar.value = 0.5f;
        roadNetworkBuilder.GenerateRoadsAndJunctions();
        yield return null;

        statusText.text = "Generation Complete!";
        progressBar.value = 1f;

        yield return new WaitForSeconds(1.0f); // Let the user see the "Complete" message

        // Switch to the next panel
        progressBar.gameObject.SetActive(false);
        networkPanel.SetActive(false);
        simulationPanel.SetActive(true);
    }

    // This method will be linked to the "Start Co-Simulation" button
    public void OnStartSimulationButton_Click()
    {
        string sumoToolPath = System.IO.Path.Combine(sumoFolderPath, "Sumo2UnityTools.exe");

        if (!System.IO.File.Exists(sumoToolPath))
        {
            statusText.text = $"Error: Sumo2UnityTools.exe not found in the selected folder!";
            return;
        }

        try
        {
            Process.Start(sumoToolPath);

            // This is crucial: it prevents the generated road network from being destroyed when we load the next scene
            DontDestroyOnLoad(roadNetworkBuilder.gameObject.transform.root.gameObject);

            // Load the simulation scene
            SceneManager.LoadScene("SimulationScene");
        }
        catch (System.Exception ex)
        {
            statusText.text = $"Error launching SUMO tool: {ex.Message}";
        }
    }
}