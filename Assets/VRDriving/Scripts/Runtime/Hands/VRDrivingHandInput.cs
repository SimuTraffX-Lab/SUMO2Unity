using UnityEngine;
using UnityEngine.InputSystem;

namespace VRDriving.Hands
{
    /// <summary>
    /// A component that manages inputs for a VR driving hand.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    [RequireComponent(typeof(VRDrivingHand))]
    public class VRDrivingHandInput : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("Contains information about the 'grab' input action.")]
        public InputActionProperty grabProperty;
        [Tooltip("Contains information about the 'release' input action.")]
        public InputActionProperty releaseProperty;

        /// <summary>A reference to the VRDrivingHand component that is driven by this component..</summary>
        public VRDrivingHand Hand { get; private set; }
        /// <summary>Returns true if no release input has been triggered since the last grab input, otherwise false.</summary>
        public bool IsGrabInputDown { get; private set; }

        // Unity callback(s).
        void Awake()
        {
            // Find VRDrivingHand reference.
            Hand = GetComponent<VRDrivingHand>();
        }

        void OnEnable()
        {
            // Subscribe to input(s).
            if (grabProperty != null && grabProperty.action != null && grabProperty.action.bindings.Count > 0)
                BindGrabProperty();
            if (releaseProperty != null && releaseProperty.action != null && releaseProperty.action.bindings.Count > 0)
                BindReleaseProperty();
        }

        void OnDisable()
        {
            // Unsubscribe from input(s).
            if (grabProperty != null && grabProperty.action != null && grabProperty.action.bindings.Count > 0)
                UnbindGrabProperty();
            if (releaseProperty != null && releaseProperty.action != null && releaseProperty.action.bindings.Count > 0)
                UnbindReleaseProperty();
        }

        // Private method(s).
        /// <summary>Binds the 'grabProperty' input.</summary>
        void BindGrabProperty()
        {
            grabProperty.action.Enable();
            grabProperty.action.started += OnGrabInput;
        }
        /// <summary>Unbinds the 'grabProperty' input.</summary>
        void UnbindGrabProperty()
        {
            grabProperty.action.Disable();
            grabProperty.action.started -= OnGrabInput;
        }

        /// <summary>Binds the 'releaseProperty' input.</summary>
        void BindReleaseProperty()
        {
            releaseProperty.action.Enable();
            releaseProperty.action.canceled += OnReleaseInput;
        }
        /// <summary>Unbinds the 'releaseProperty' input.</summary>
        void UnbindReleaseProperty()
        {
            releaseProperty.action.Disable();
            releaseProperty.action.canceled -= OnReleaseInput;
        }

        // Private callback(s).
        /// <summary>A callback that is invoked when this hands grab input is triggered.</summary>
        /// <param name="pContext"></param>
        void OnGrabInput(InputAction.CallbackContext pContext)
        {
            // Grab input is now 'down'.
            IsGrabInputDown = true;

            // Make hand attempt grab by palm trace.
            Hand.Grabber.TryGrab();
        }

        /// <summary>A callback that is invoked when this hands release input is triggered.</summary>
        /// <param name="pContext"></param>
        void OnReleaseInput(InputAction.CallbackContext pContext)
        {
            // Grab input no longer 'down'.
            IsGrabInputDown = false;

            // Release hand.
            Hand.Grabber.Release();
        }
    }
}
