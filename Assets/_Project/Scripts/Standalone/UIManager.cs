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
    public GameObject setupPanel; // <-- ADD THIS
    public GameObject simulationPanel;
    public TMP_InputField pathInputField;
    public TMP_Text statusText;
    public Slider progressBar;

    [Header("Core References")]
    public RoadNetworkBuilder roadNetworkBuilder; // Drag your RoadNetworkBuilder instance here
    public FreeFlyCamera freeFlyCamera; // <-- ADD THIS (We will create this script next)
    //public SelectionManager selectionManager; // <-- ADD THIS (We will create this script next)

    public ObjectSelector objectSelector; // <-- ADD THIS (We will create this script next)

    private GameObject roadNetworkRoot;

    // Stores the path selected by the user
    private string sumoFolderPath;

    void Start()
    {
        // Set the initial state of the UI
        networkPanel.SetActive(true); 
        setupPanel.SetActive(false); // <-- ADD THIS
        simulationPanel.SetActive(false);
        progressBar.gameObject.SetActive(false); // Hide the progress bar at the start
        statusText.text = "Please select a SUMO scenario folder.";

        // Disable the editing scripts at the start
        if (freeFlyCamera) freeFlyCamera.enabled = false;
        //if (selectionManager) selectionManager.enabled = false;
    }

    private void Update()
    {
      
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

        // --- NEW WORKFLOW ---
        // Switch to the Setup panel instead of the Simulation panel
        progressBar.gameObject.SetActive(false);
        networkPanel.SetActive(false);
        setupPanel.SetActive(true); // <-- CHANGE THIS

        // Enable the camera and selection scripts
        UnityEngine.Debug.Log("Entering Setup Mode.");
        if (freeFlyCamera) freeFlyCamera.enabled = true;
        //if (selectionManager)
        //{
        //    selectionManager.enabled = true;
        //    // Give the selection manager the root object to work with
        //    selectionManager.SetRoadNetworkRoot(roadNetworkBuilder.gameObject.transform.root.gameObject);
        //}
        roadNetworkRoot = GameObject.Find("RoadNetworkRoot");
       
    }

    // This method is now linked to the NEW button on the SetupPanel
    public void OnFinalizeAndStartSimulation_Click()
    {
        // Disable the editing scripts before moving to the next scene
        if (freeFlyCamera) freeFlyCamera.enabled = false;
        //if (selectionManager) selectionManager.enabled = false;

        // Now call your existing simulation start logic
        OnStartSimulationButton_Click();
    }

    // This method will be linked to the "Start Co-Simulation" button
    public void OnStartSimulationButton_Click()
    {
        try
        {
            DontDestroyOnLoad(roadNetworkRoot);

            // This is crucial: it prevents the generated road network from being destroyed when we load the next scene
            DontDestroyOnLoad(roadNetworkBuilder.gameObject.transform.root.gameObject);

            // Load the simulation scene
            SceneManager.LoadScene("Scenario1");
        }
        catch (System.Exception ex)
        {
            statusText.text = $"Error launching SUMO tool: {ex.Message}";
        }
    }
}