using UnityEngine;

namespace VRDriving.Transformation
{
    /// <summary>
    /// A component that allows a non-parented gameObject to maintain it's position and rotation relative to another transform without being a child object.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class MaintainTransformOffset : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("The transform to follow.")]
        public Transform followTransform;

        /// <summary>The position offset of the follow transform at the last call to StorePositionOffset().</summary>
        protected Vector3 m_InitialFollowOffset;
        /// <summary>The rotation offset of the follow transform at the last call to StoreRotationOffset().</summary>
        protected Quaternion m_InitialFollowRotationOffset;

        // Unity callback(s).
        void Start()
        {
            // Ensure a follow transform reference is set.
            if (followTransform != null)
            {
                // Store initial position and rotation of follow transform.
                StoreOffsets();
            }
            else { Debug.LogWarning("No 'followTransform' set on MaintainTransformOffset component on gameObject '" + gameObject.name + "'.", gameObject); }
        }

        void Update()
        {
            // Calculate offset of follow transform since last frame, apply to this component's transform.    
            // Calculate rotation offset of follow transform since last frame, apply to this component's transform.
            transform.SetPositionAndRotation(followTransform.position + m_InitialFollowOffset, followTransform.rotation * m_InitialFollowRotationOffset);
        }

        // Public method(s)
        public void StoreOffsets()
        {
            StorePositionOffset();
            StoreRotationOffset();
        }

        /// <summary>Stores the current position offset relative to the followTransform.</summary>
        public void StorePositionOffset()
        {
            m_InitialFollowOffset = transform.position - followTransform.position;
        }

        /// <summary>Stores the current rotation offset relative to the followTransform.</summary>
        public void StoreRotationOffset()
        {
            m_InitialFollowRotationOffset = transform.rotation * Quaternion.Inverse(followTransform.rotation);
        }

        /// <summary>Applies the offset, pOffset, in world space to the initial follow offset.</summary>
        /// <param name="pOffset"></param>
        public void ApplyPositionOffset(Vector3 pOffset)
        {
            m_InitialFollowOffset += pOffset;
        }
    }
}
