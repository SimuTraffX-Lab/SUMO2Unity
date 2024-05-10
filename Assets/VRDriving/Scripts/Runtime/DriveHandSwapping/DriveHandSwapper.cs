using UnityEngine;
using GrabSystem;
using VRDriving.VehicleSystem;

namespace VRDriving.DriveHandSwapper
{
    /// <summary>
    /// A component that swaps driving controls to the 'free hand' when using the grab system in conjunction with the vehicle system.
    /// </summary>
    /// Author: Mathew Aloisio
    public class DriveHandSwapper : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("Should driving inputs be disabled while both hands are grabbing non-ignored grabbables?")]
        public bool disableOnBothGrab;
        [Tooltip("Should inputs be restored to the prefered driving hand when both hands have released their grabbables?")]
        public bool usePrefOnBothRelease;

        [Header("References")]
        [Tooltip("The vehicle mover that steering is being controlled for.")]
        public VehicleMover vehicleMover;
        [Tooltip("A reference to any GrabbableObjects that should be ignored and not override the 'driving hand' when grabbed. Grabs of these do not override the driving inputs hand. (Example: GrabbableObjects that are part of the steering system.)")]
        public GrabbableObject[] ignoreGrabbables;
        [Tooltip("A reference to the left hand Grabber component.")]
        public Grabber leftGrabber;
        [Tooltip("A reference to thje right hand Grabber component.")]
        public Grabber rightGrabber;

        /// <summary>Returns true if driving inputs have been disabled by this component, otherwise false.</summary>
        public bool DrivingInputsDisabled { get; protected set; }

        // Unity callback(s).
        #region Unity Callback(s)
        void Awake()
        {
            // Ensure a vehicleMover reference is set.
            if (vehicleMover == null)
                Debug.LogWarning("Field 'vehicleMover' is null for DriveHandSwapper component!", gameObject);

            // Ensure left and right grabber references are set.
            if (leftGrabber == null)
                Debug.LogWarning("Field 'leftGrabber' is null for DriveHandSwapper component!", gameObject);
            if (rightGrabber == null)
                Debug.LogWarning("Field 'rightGrabber' is null for DriveHandSwapper component!", gameObject);
        }

        void OnEnable()
        {
            // Subscribe to relevant event(s).
            leftGrabber.Grabbed.AddListener(OnLeftGrabberGrabbed);
            rightGrabber.Grabbed.AddListener(OnRightGrabberGrabbed);
            leftGrabber.Released.AddListener(OnLeftGrabberReleased);
            rightGrabber.Released.AddListener(OnRightGrabberReleased);
        }

        void OnDisable()
        {
            // Unsubscribe from relevant event(s).
            leftGrabber.Grabbed.RemoveListener(OnLeftGrabberGrabbed);
            rightGrabber.Grabbed.RemoveListener(OnRightGrabberGrabbed);
            leftGrabber.Released.RemoveListener(OnLeftGrabberReleased);
            rightGrabber.Released.RemoveListener(OnRightGrabberReleased);
        }
        #endregion

        // Public method(s).
        #region Public Method(s)
        /// <summary>Sets whether or not driving inputs for the relevant VehicleMover are disabled by this component.</summary>
        /// <param name="pDisabled"></param>
        public void SetDrivingInputsDisabled(bool pDisabled)
        {
            vehicleMover.simulateInputs = !pDisabled;
            DrivingInputsDisabled = pDisabled;
        }
        #endregion

        // Protected callback(s).
        #region Grab & Release Protected Callback(s)
        /// <summary>Invoked whenever the 'left grabber' grabs something.</summary>
        /// <param name="pGrabber"></param>
        /// <param name="pGrabbable"></param>
        protected void OnLeftGrabberGrabbed(Grabber pGrabber, GrabbableObject pGrabbable) { OnGrabbedCallback(ControllerSide.Left, pGrabber, pGrabbable); }

        /// <summary>Invoked whenever the 'right grabber' grabs something.</summary>
        /// <param name="pGrabber"></param>
        /// <param name="pGrabbable"></param>
        protected void OnRightGrabberGrabbed(Grabber pGrabber, GrabbableObject pGrabbable) { OnGrabbedCallback(ControllerSide.Right, pGrabber, pGrabbable); }

        /// <summary>Invoked when either the left or right controller grabs something.</summary>
        /// <param name="pControllerSide"></param>
        /// <param name="pGrabber"></param>
        /// <param name="pGrabbable"></param>
        protected void OnGrabbedCallback(ControllerSide pControllerSide, Grabber pGrabber, GrabbableObject pGrabbable)
        {
            // Check if the pGrabbable is ignored.
            if (ignoreGrabbables != null && ignoreGrabbables.Length > 0)
            {
                // If pGrabbable is found in the array return immediately.
                foreach (GrabbableObject ignoredGrabbable in ignoreGrabbables)
                {
                    if (ignoredGrabbable == pGrabbable)
                        return;
                }
            }

            // Invoke the 'OnGrabbed' callback.
            OnGrabbed(pControllerSide, pGrabber, pGrabbable);
        }

        /// <summary>Invoked whenever the 'left grabber' releases something.</summary>
        /// <param name="pGrabber"></param>
        /// <param name="pGrabbable"></param>
        protected void OnLeftGrabberReleased(Grabber pGrabber, GrabbableObject pGrabbable) { OnReleasedCallback(ControllerSide.Left, pGrabber, pGrabbable); }

        /// <summary>Invoked whenever the 'right grabber' releases something.</summary>
        /// <param name="pGrabber"></param>
        /// <param name="pGrabbable"></param>
        protected void OnRightGrabberReleased(Grabber pGrabber, GrabbableObject pGrabbable) { OnReleasedCallback(ControllerSide.Right, pGrabber, pGrabbable); }

        /// <summary>Invoked when either the left or right controller releases something.</summary>
        /// <param name="pControllerSide"></param>
        /// <param name="pGrabber"></param>
        /// <param name="pGrabbable"></param>
        protected void OnReleasedCallback(ControllerSide pControllerSide, Grabber pGrabber, GrabbableObject pGrabbable)
        {
            // Check if the pGrabbable is ignored.
            if (ignoreGrabbables != null && ignoreGrabbables.Length > 0)
            {
                // If pGrabbable is found in the array return immediately.
                foreach (GrabbableObject ignoredGrabbable in ignoreGrabbables)
                {
                    if (ignoredGrabbable == pGrabbable)
                        return;
                }
            }

            // Invoke the 'OnReleased' callback.
            OnReleased(pControllerSide, pGrabber, pGrabbable);
        }
        #endregion

        // Protected virtual callback(s).
        #region Protected Virtual Callback(s)
        /// <summary>
        /// Invoked when either the left or right controller grabs something that is in the 'ignoreGrabbables' array.
        /// This overrideable callback contains the behaviour for drive hand swapping when either the left or right grabber grab non-ignored grabbable object.
        /// </summary>
        /// <param name="pControllerSide"></param>
        /// <param name="pGrabber"></param>
        /// <param name="pGrabbable"></param>
        protected virtual void OnGrabbed(ControllerSide pControllerSide, Grabber pGrabber, GrabbableObject pGrabbable)
        {
            // If the current 'driving hand' has grabbed something then switch driving hand inputs.
            // We can assume that something that was not in the 'ingoreGrabbables' array was grabbed as this callback is only invoked on grabs that pass prerequisite checks like this one.
            if (vehicleMover.useDrivingHand == pControllerSide)
            {
                // Current driving hand has grabbed a non-ignored grabbable! Switch to other hand.
                vehicleMover.useDrivingHand = GetOppositeControllerSide(vehicleMover.useDrivingHand);
            }

            // If both the left and right hand are grabbing use the prefered driving hand.
            if (leftGrabber.Grabbing != null && rightGrabber.Grabbing != null)
            {
                vehicleMover.useDrivingHand = vehicleMover.preferredDrivingHand;

                // If set to 'disable on both grab' then disable driving inputs.
                if (disableOnBothGrab)
                    SetDrivingInputsDisabled(true);
            }
        }

        /// <summary>
        /// Invoked when either the left or right controller releases something that is in the 'ignoreGrabbables' array.
        /// This overrideable callback contains the behaviour for drive hand swapping when either the left or right grabber release non-ignored grabbable object.
        /// </summary>
        /// <param name="pControllerSide"></param>
        /// <param name="pGrabber"></param>
        /// <param name="pGrabbable"></param>
        protected virtual void OnReleased(ControllerSide pControllerSide, Grabber pGrabber, GrabbableObject pGrabbable)
        {
            // If driving inputs are disabled by this component re-enable them.
            if (DrivingInputsDisabled)
            {
                // Set driving hand to the released hand.
                vehicleMover.useDrivingHand = pControllerSide;

                // Enable driving inputs.
                SetDrivingInputsDisabled(true);
            }

            // If both 'grabbers' are not grabbing anything and 'usePrefOnBothRelease' is enabled then restore driving hand to prefered hand.
            if (leftGrabber.Grabbing == null && rightGrabber.Grabbing == null && usePrefOnBothRelease)
            {
                // Restore prefered driving hand.
                vehicleMover.useDrivingHand = vehicleMover.preferredDrivingHand;
            }
        }
        #endregion

        #region Public Static Method(s)
        /// <summary>Given a ControllerSide, pSide, returns the opposite controller side.</summary>
        /// <param name="pSide"></param>
        /// <returns>the ControllerSide that is opposite to pSide.</returns>
        public static ControllerSide GetOppositeControllerSide(ControllerSide pSide)
        {
            return pSide == ControllerSide.Left ? ControllerSide.Right : ControllerSide.Left;
        }
        #endregion
    }
}
