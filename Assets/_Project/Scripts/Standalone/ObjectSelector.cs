using UnityEngine;
using UnityEngine.EventSystems; // Be sure to include this namespace for the UI check

public class ObjectSelector : MonoBehaviour
{
    [Header("UI Settings")]
    public GameObject uiPanelPrefab;
    public Transform worldCanvasTransform;

    [Header("Positioning Offset")]
    public float horizontalOffset = 0.5f;
    public float verticalOffset = 0.5f;

    [Header("Scaling")]
    public bool adjustScaleForDistance = true;
    public float referenceDistance = 20f;
    public float scaleMultiplier = 1f;

    [Header("Hover Effect")]
    public bool enableHoverEffect = true;
    public Color hoverColor = new Color(0.5f, 0.8f, 1f, 1f);

    private GameObject currentHoveredObject;
    private Color originalColor;
    private Renderer hoveredRenderer;
    private GameObject currentUIPanel;

    void Update()
    {
        // We use the EventSystem to check if the mouse is currently over a UI element.
        bool isPointerOverUI = EventSystem.current.IsPointerOverGameObject();

        HandleHover(isPointerOverUI);
        HandleClick(isPointerOverUI);
    }

    /// <summary>
    /// Handles the hover effect. Now ignores hovering if the mouse is over any UI.
    /// </summary>
    private void HandleHover(bool isPointerOverUI)
    {
        if (!enableHoverEffect)
        {
            if (currentHoveredObject != null) ClearHighlight();
            return;
        }

        // --- FIX #1: Don't highlight objects if the mouse is over the UI panel ---
        if (isPointerOverUI)
        {
            ClearHighlight();
            return;
        }
        // -------------------------------------------------------------------------

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject roadNetworkRoot = GameObject.Find("RoadNetworkRoot");
            if (roadNetworkRoot != null && hit.transform.IsChildOf(roadNetworkRoot.transform))
            {
                if (currentHoveredObject != hit.transform.gameObject)
                {
                    ClearHighlight();
                    ApplyHighlight(hit.transform.gameObject);
                }
            }
            else
            {
                ClearHighlight();
            }
        }
        else
        {
            ClearHighlight();
        }
    }

    /// <summary>
    /// Handles the click logic. Now checks for UI before destroying the panel.
    /// </summary>
    // This is just the HandleClick method. The rest of your ObjectSelector script stays the same.

    private void HandleClick(bool isPointerOverUI)
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isPointerOverUI)
            {
                return;
            }

            if (currentUIPanel != null)
            {
                Destroy(currentUIPanel);
            }

            // Check if we are currently hovering over a valid object
            if (currentHoveredObject != null)
            {
                // --- NEW LOGIC STARTS HERE ---

                // 1. Try to get the LaneSegmentDecalController from the object we clicked on.
                var laneController = currentHoveredObject.GetComponent<LaneSegmentDecalController>();

                // 2. Only proceed if the object actually has that component (i.e., it's a lane, not a junction).
                if (laneController != null)
                {
                    // We still need to raycast to get the exact world position for the panel.
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit))
                    {
                        // --- This is all your existing code for positioning and scaling ---
                        Vector3 cameraRight = Camera.main.transform.right * horizontalOffset;
                        Vector3 cameraUp = Camera.main.transform.up * verticalOffset;
                        Vector3 offsetPosition = hit.point + cameraRight + cameraUp;

                        currentUIPanel = Instantiate(uiPanelPrefab, worldCanvasTransform);
                        currentUIPanel.transform.position = offsetPosition;
                        currentUIPanel.transform.rotation = Quaternion.LookRotation(currentUIPanel.transform.position - Camera.main.transform.position);

                        if (adjustScaleForDistance)
                        {
                            float distance = Vector3.Distance(Camera.main.transform.position, hit.point);
                            if (referenceDistance > 0.01f)
                            {
                                float scaleFactor = (distance / referenceDistance) * scaleMultiplier;
                                currentUIPanel.transform.localScale = Vector3.one * scaleFactor;
                            }
                        }
                        else
                        {
                            currentUIPanel.transform.localScale = Vector3.one * scaleMultiplier;
                        }

                        // --- 3. THE FINAL CONNECTION ---
                        // Get the UIPanelController from the newly created panel instance...
                        var panelController = currentUIPanel.GetComponent<UIPanelController>();
                        if (panelController != null)
                        {
                            // ...and pass it the reference to the laneController we found.
                            panelController.Initialize(laneController);
                        }
                    }
                }
                // If laneController is null (e.g., we clicked a junction), the panel won't be created.
                // You could add an 'else' here to show a different panel for junctions if you wanted.
            }
        }
    }

    private void ApplyHighlight(GameObject obj)
    {
        hoveredRenderer = obj.GetComponent<Renderer>();
        if (hoveredRenderer != null)
        {
            originalColor = hoveredRenderer.material.color;
            hoveredRenderer.material.color = hoverColor;
            currentHoveredObject = obj;
        }
    }

    private void ClearHighlight()
    {
        if (currentHoveredObject == null || hoveredRenderer == null) return;
        hoveredRenderer.material.color = originalColor;
        currentHoveredObject = null;
        hoveredRenderer = null;
    }
}