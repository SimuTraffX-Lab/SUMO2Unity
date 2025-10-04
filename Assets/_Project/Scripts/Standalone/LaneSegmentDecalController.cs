using UnityEngine;
using System; // <-- ADD THIS LINE for StringComparison
using UnityEngine.Rendering.Universal; // <-- ADD THIS LINE for DecalProjector
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
[DisallowMultipleComponent]
public class LaneSegmentDecalController : MonoBehaviour
{
    [Tooltip("Broken Lines in LEFT lane marking (acts on objects named *Right* after swap).")]
    public bool brokenLeft;
    [Tooltip("Broken Lines in RIGHT lane marking (acts on objects named *Left* after swap).")]
    public bool brokenRight;

    [HideInInspector] public float solidDepth = 3f;
    [HideInInspector] public float brokenDepth = 1.5f;

    private void OnValidate()
    {
        UpdateVisuals();
    }

    /// <summary>
    /// This is our new public method that contains the update logic.
    /// It can be called from anywhere, at any time.
    /// </summary>
    public void UpdateVisuals()
    {
        SetDepthForSide("LaneMarking_Right_Decal", brokenLeft);
        SetDepthForSide("LaneMarking_Left_Decal", brokenRight);
    }

    private void SetDepthForSide(string prefix, bool broken)
    {
        float targetDepth = broken ? brokenDepth : solidDepth;

        // This line will now compile correctly
        var decals = GetComponentsInChildren<DecalProjector>(true);
        foreach (var d in decals)
        {
            // This line will now compile correctly
            if (!d.name.StartsWith(prefix, StringComparison.Ordinal)) continue;

            Vector3 s = d.size;
            if (Mathf.Approximately(s.z, targetDepth)) continue;

            s.z = targetDepth;
            d.size = s;

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                EditorUtility.SetDirty(d);
            }
#endif
        }
    }
}