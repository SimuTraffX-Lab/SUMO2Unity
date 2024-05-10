using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace VRDriving.Transformation
{
    /// <summary>
    /// A class that positions menus at a set offset from the an anchor's position.
    /// The wait action is intended to ensure the transform is set to the appropriate headset when using a tracking-based transform like XR Helmet.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class AnchoredTransform : MonoBehaviour
    {
        // EulerAngles.
        [Serializable]
        public struct EulerAnglesBooleans
        {
            public bool x;
            public bool y;
            public bool z;
        }

        // TrackMode.
        public enum TrackMode
        {
            Once,
            OnEnable,
            Continuous
        }

        // AnchoredTransform.
        [Header("Settings")]
        [Tooltip("The anchor to positive the menu relative to.")]
        public Transform anchor;
        [Tooltip("The offset of the menu from the anchor.")]
        public Vector3 offset;
        [Tooltip("The mode used for tracking.")]
        public TrackMode trackMode;
        [Tooltip("What euler angles of the anchor (if any) should this component ignore?")]
        public EulerAnglesBooleans ignoreAngles;
        [Tooltip("(Optional) A reference to an action that has to be performed before the anchored transform should initialize.")]
        [SerializeField] private InputAction m_WaitForAction = null;

        private bool m_MovedToAnchor = false;
        private bool m_PerformedWaitAction = false;

        // Unity callback(s).
        void OnEnable()
        {
            // Bind wait action if one is set.
            if (m_WaitForAction.bindings.Count > 0)
            {
                m_WaitForAction.Enable();
                m_WaitForAction.performed += OnWaitAction;
            }
        }

        void OnDisable()
        {
            // Unbind wait action if one is set.
            if (m_WaitForAction.bindings.Count > 0 && !m_PerformedWaitAction)
            {
                m_WaitForAction.performed -= OnWaitAction;
                m_WaitForAction.Disable();
            }

            // If we're in OnEnable mode reset the m_MovedToAnchor boolean when the component is disabled.
            if (trackMode == TrackMode.OnEnable)
                m_MovedToAnchor = false;
        }

        void Update()
        {
            // Only move to the anchor if there is no wait action or it's been performed.
            if (m_WaitForAction.bindings.Count == 0 || m_PerformedWaitAction)
            {
                if (trackMode == TrackMode.Continuous || !m_MovedToAnchor)
                {
                    // Move the anchored transform to the anchor.
                    MoveToAnchor();
                }
            }
        }

        // Public method(s).
        /// <summary>
        /// Moves the gameobject's transform to the anchor.
        /// </summary>
        public void MoveToAnchor()
        {
            transform.position = anchor.transform.position + anchor.transform.TransformDirection(offset);
            transform.eulerAngles = new Vector3(
                ignoreAngles.x ? transform.eulerAngles.x : anchor.transform.eulerAngles.x,
                ignoreAngles.y ? transform.eulerAngles.y : anchor.transform.eulerAngles.y,
                ignoreAngles.z ? transform.eulerAngles.z : anchor.transform.eulerAngles.z
            );
            m_MovedToAnchor = true;
        }

        // Private callback(s).
        void OnWaitAction(InputAction.CallbackContext pContext)
        {
            m_PerformedWaitAction = true;
            m_WaitForAction.performed -= OnWaitAction;
        }
    }
}
