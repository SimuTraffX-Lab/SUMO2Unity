using UnityEngine;
using UnityEngine.UI;

public class UIPanelController : MonoBehaviour
{
    public Toggle leftToggle;
    public Toggle rightToggle;
    private LaneSegmentDecalController targetController;

    public void Initialize(LaneSegmentDecalController controller)
    {
        targetController = controller;
        if (targetController != null)
        {
            leftToggle.isOn = targetController.brokenLeft;
            rightToggle.isOn = targetController.brokenRight;
            leftToggle.onValueChanged.AddListener(OnLeftToggleChanged);
            rightToggle.onValueChanged.AddListener(OnRightToggleChanged);
        }
    }

    public void OnLeftToggleChanged(bool newValue)
    {
        if (targetController != null)
        {
            targetController.brokenLeft = newValue;

            // --- ADD THIS LINE ---
            // Explicitly tell the controller to update its appearance.
            targetController.UpdateVisuals();
        }
    }

    public void OnRightToggleChanged(bool newValue)
    {
        if (targetController != null)
        {
            targetController.brokenRight = newValue;

            // --- ADD THIS LINE ---
            // Explicitly tell the controller to update its appearance.
            targetController.UpdateVisuals();
        }
    }

    private void OnDestroy()
    {
        if (leftToggle != null) leftToggle.onValueChanged.RemoveListener(OnLeftToggleChanged);
        if (rightToggle != null) rightToggle.onValueChanged.RemoveListener(OnRightToggleChanged);
    }
}