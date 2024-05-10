using UnityEngine;
using System.Linq;
using GrabSystem;
using VRDriving.Grabbing;

namespace VRDriving.Hands
{
    /// <summary>
    /// A component that controls the behaviour of the provided VR driving hands.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    [RequireComponent(typeof(ConditionalGrabber))]
    public class VRDrivingHand : MonoBehaviour
    {
        #region Editor Serialized Settings
        [Header("Settings - Hand")]
        [Tooltip("Enable debugging?")]
        public bool debug;
        [Tooltip("The handedness of this hand. (ControllerSide.Left or ControllerSide.Right)")]
        public ControllerSide handSide;

        [Header("Settings - Hand Movement")]
        [Tooltip("Should this hand follow the followTransform if a valid one is referenced?")]
        public bool followEnabled = true;

        [Header("Settings - Tracking")]
        [Tooltip("A reference to the Transform the VR driving hand follows.")]
        public Transform followTransform;
        #endregion
        #region Public Properties
        /// <summary>A reference to the object being grabbed by this hand, otherwise null if nothing being grabbed.</summary>
        public ConditionalGrabber Grabber { get; private set; }
        #endregion

        // Unity callback(s)
        #region Unity Callback(s)
        void Awake()
        {
            // Find 'Grabber' reference.
            Grabber = GetComponent<ConditionalGrabber>();
        }

        void Update()
        {
            // Handle hand following if it is enabled and there is a valid followTransform referenced.
            if (followEnabled && followTransform != null)
            {
                // If an object is not being grabbed follow the follow transforms.
                if (Grabber.Grabbing == null)
                {
                    // Set position and rotation to match the followTransform's.
                    transform.SetPositionAndRotation(
                        followTransform.position,
                        followTransform.rotation
                    );
                }
            }
        }

        void OnEnable()
        {
            // Subscribe to relevant event(s).
            Grabber.Grabbed.AddListener(OnGrabberGrabbed);
            Grabber.Released.AddListener(OnGrabberReleased);
        }

        void OnDisable()
        {
            // Unsbscribe from relevant event(s).
            Grabber.Grabbed.RemoveListener(OnGrabberGrabbed);
            Grabber.Released.RemoveListener(OnGrabberReleased);
        }
        #endregion

        // Public method(s).
        #region Serialized Setting Method(s)
        /// <summary>Sets the 'followEnabled' field of this component instance. Useful for use with Unity editor events.</summary>
        /// <param name="pEnabled"></param>
        public void SetFollowEnabled(bool pEnabled) { followEnabled = pEnabled; }
        /// <summary>Sets the 'followTransform' field of this component instance. Useful for use with Unity editor events.</summary>
        /// <param name="pFollow"></param>
        public void SetFollowTransform(Transform pFollow) { followTransform = pFollow; }
        /// <summary>Sets the 'followTransform' field of this component instance to null. Useful for use with Unity editor events.</summary>
        public void SetFollowTransformToNull() { followTransform = null; }
        #endregion

        // Private callback(s).
        #region Grabber Callback(s).
        /// <summary>Invoked by the 'Grabber.Grabbed' callback and passes the grab details onto any IGrabbable component found on the pGrabbable gameObject.</summary>
        /// <param name="pGrabber"></param>
        /// <param name="pGrabbable"></param>
        void OnGrabberGrabbed(Grabber pGrabber, GrabbableObject pGrabbable)
        {
            // Invoke 'grabbed' on any IGrabbables on the grabbable object.
            IGrabbable[] grabbables = InterfaceHelper.GetInterfaces<IGrabbable>(pGrabbable.gameObject).ToArray();
            if (grabbables != null && grabbables.Length > 0)
            {
                foreach (IGrabbable grabbable in grabbables)
                {
                    if (grabbable != null)
                        grabbable.OnGrabbed(followTransform != null ? followTransform : transform, handSide);
                }
            }
        }

        /// <summary>Invoked by the 'Grabber.Released' callback and passes the release details onto any IGrabbable component found on the pGrabbable gameObject.</summary>
        /// <param name="pGrabber"></param>
        /// <param name="pGrabbable"></param>
        void OnGrabberReleased(Grabber pGrabber, GrabbableObject pGrabbable)
        {
            // Invoke 'released' on any IGrabbables on the grabbable object.
            IGrabbable[] grabbables = InterfaceHelper.GetInterfaces<IGrabbable>(pGrabbable.gameObject).ToArray();
            if (grabbables != null && grabbables.Length > 0)
            {
                foreach (IGrabbable grabbable in grabbables)
                {
                    if (grabbable != null)
                        grabbable.OnReleased(followTransform != null ? followTransform : transform, handSide);
                }
            }
        }
        #endregion
    }
}
