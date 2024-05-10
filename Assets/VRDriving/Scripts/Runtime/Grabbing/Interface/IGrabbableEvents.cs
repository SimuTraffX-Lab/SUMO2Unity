using System;
using UnityEngine;
using UnityEngine.Events;

namespace VRDriving.Grabbing
{
    /// <summary>
    /// A component that provides C# events (editor events) for IGrabbable callbacks.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class IGrabbableEvents : MonoBehaviour, IGrabbable
    {
        // ControllerUnityEvent;
        /// <summary>
        /// Arg0: Transform - The controller transform.
        /// Arg1: ControllerSide - The side the controller is on.
        /// </summary>
        [Serializable]
        public class ControllerUnityEvent : UnityEvent<Transform, ControllerSide> { }

        // IGrabbableEvents.
        [Header("Events")]
        [Tooltip("Invoked when the IGrabbables 'OnGrabbed' callback is invoked.\n\nArg0: Transform - The controller that grabbed the grabbable.\nArg1: ControllerSide - The side of the controller that grabbed the grabbable.")]
        public ControllerUnityEvent Grabbed;
        [Tooltip("Invoked when the IGrabbables 'OnReleased' callback is invoked.\n\nArg0: Transform - The controller that released the grabbable.\nArg1: ControllerSide - The side of the controller that released the grabbable.")]
        public ControllerUnityEvent Released;

        // Public override method(s).
        /// <summary>Dispatches an event when IGrabbable's 'OnGrabbed' callback is invoked.</summary>
        /// <param name="pControllerTransform"></param>
        /// <param name="pControllerSide"></param>
        public void OnGrabbed(Transform pControllerTransform, ControllerSide pControllerSide)
        {
            // Invoke the 'Grabbed' Unity event.
            Grabbed?.Invoke(pControllerTransform, pControllerSide);
        }

        /// <summary>Dispatches an event when IGrabbable's 'OnReleased' callback is invoked.</summary>
        /// <param name="pControllerTransform"></param>
        /// <param name="pControllerSide"></param>
        public void OnReleased(Transform pControllerTransform, ControllerSide pControllerSide)
        {
            // Invoke the 'Released' Unity event.
            Released?.Invoke(pControllerTransform, pControllerSide);
        }
    }
}
