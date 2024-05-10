using UnityEngine;

namespace VRDriving.Transformation
{
    /// <summary>
    /// A component that makes a gameObject set it's localPosition and localRotation to exactly match that of another gameObject.
    /// (Initial intended use: HandgunSlide Grabbale object.)
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class CopyLocalTransform : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("The transform to copy.")]
        public Transform copyTransform;

        // Unity callback(s).
        void Start()
        {
            if (copyTransform == null)
                Debug.LogWarning("No 'copyTransform' set on CopyLocalTransform component on gameObject '" + gameObject.name + "'.");
        }

        void Update()
        {
            // Maintain copyTransform's local position/rotation.
            transform.localPosition = copyTransform.localPosition;
            transform.localRotation = copyTransform.localRotation;
        }
    }
}
