using UnityEngine;

namespace VRDriving.Transformation
{
    /// <summary>
    /// A component that allows a non-parented gameObject to follow/copy another's transform.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class FollowTransform : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("The transform to follow.")]
        public Transform followTransform;

        // Unity callback(s).
        void Start()
        {
            // Ensure a follow transform reference is set.
            if (followTransform == null)
                Debug.LogWarning("No 'followTransform' set on FollowTransform component on gameObject '" + gameObject.name + "'.", gameObject);
        }

        void Update()
        {
            // Update position and rotation.
            transform.SetPositionAndRotation(followTransform.position, followTransform.rotation);
        }
    }
}
