using System;
using UnityEngine;
using UnityEngine.Events;
using VRDriving.Grabbing;
using VRDriving.Math;

namespace VRDriving.Gadgets
{
    /// <summary>
    /// A component that allows you to make a switch that fires Unity events allowing you to assign any action to different LeverPositionEntrys.
    /// </summary>
    /// Author: Mathew Aloisio
    public class KinematicLeverGadget : MonoBehaviour, IGrabbable
    {
        // LeverPosShiftUnityEvent.
        /// <summary>
        /// Arg0: int - The index in the leverPositions array that was shifted from, or -1.
        /// Arg1: LeverPositionEntry - The LeverPositionEntry that was shifted from, or null.
        /// Arg2: int - The index in the leverPositions array that was shifted to, or -1.
        /// Arg3: LeverPositionEntry - The LeverPositionEntry that was shifted to, or null.
        /// LeverPositionEntry
        /// </summary>
        [Serializable]
        public class LeverUnityEvent : UnityEvent<int, LeverPositionEntry, int, LeverPositionEntry> { }

        // LeverPositionEntry.
        [Serializable]
        public class LeverPositionEntry
        {
            [Tooltip("The target local position of the leverPivot in local space when shifted to this gear.")]
            public Vector3 leverPivotPosition;
            [Tooltip("The target local euler angles of the leverPivot in local space when shifted to this gear. (-180f to 180f, same as seen in editor.)")]
            public Vector3 leverPivotEulerAngles;

            [Tooltip("An event that is invoked when this lever position is changed to.")]
            public UnityEvent ChangedTo;
            [Tooltip("An event that is invoked when this lever position is changed from.")]
            public UnityEvent ChangedFrom;
        }

        // AxisSettings.
        [Serializable]
        public class AxisSettings
        {
            [Tooltip("The setting for the x axis.")]
            public bool x;
            [Tooltip("The setting for the y axis.")]
            public bool y;
            [Tooltip("The setting for the z axis.")]
            public bool z;
        }

        // KinematicLeverShift.
        [Header("Settings - Lever")]
        [Tooltip("The lever position index the lever should start in.")]
        public int startPositionIndex = 0;
        [Tooltip("An array of possible LeverPositionEntrys this lever can shift between.")]
        public LeverPositionEntry[] leverPositions;
        [Tooltip("The pivot for the lever.")]
        public Transform leverPivot;
        [Tooltip("(Optional) The Transform that controls the direction for the lever. If null 'transform' is used as a fallback.")]
        [SerializeField] Transform m_LeverDirection;
        [Tooltip("The speed the gear shift moves to it's target position in units per second. (units/sec)")]
        public float leverMoveRate = 0.2f;
        [Tooltip("The speed the gear shift rotates to it's target orientation in degrees per second. (degrees/sec)")]
        public float leverRotationRate = 200f;
        [Tooltip("Allows leverPivot rotations around certain axis to be disabled.")]
        public AxisSettings leverDisableRotations;

        [Header("Settings - Lever Return")]
        [Tooltip("The lever position index to return to after being released. A value of -1 means do not return to any specific index -- maintain current state.")]
        public int returnToIndexOnRelease = -1;
        [Tooltip("The number of seconds it should take to 'return to index' after being released.")]
        public float returnToIndexAfterSeconds = 0f;

        [Header("Settings - Controller")]
        [Tooltip("Moving the controller along this direction shifts the lever down, moving it opposite of this direction shifts the lever up. (Local space relative to LeverDirectionTransform.)")]
        public Vector3 shiftControllerDirection = Vector3.forward;
        [Tooltip("The distance the shifting controller must move along the shiftControllerAxis to shift one lever position entry.")]
        public float shiftControllerDistance = 0.05f;

        [Header("Eventts - Shifter")]
        [Tooltip("An event that is invoked when this lever position changes.\n\nArg0: int - The index in the leverPositions array that was shifted from, or -1.\nArg2: int - The index in the leverPositions array that was shifted to, or -1.\nArg3: LeverPositionEntry - The last LeverPositionEntry that was shifted from, or null.\nArg2: LeverPositionEntry - The LeverPositionEntry that was shifted to, or null.")]
        public LeverUnityEvent LeverChanged;

        /// <summary>The local position of the last controller to grab the lever, at the time it grabbed the lever, or at the time of last shift whichever came last.</summary>
        public Vector3 ControllerLocalPositionLastShift { get; private set; }
        /// <summary>A reference to the Transform of the controller currently grabbing the gear shift, otherwise null.</summary>
        public Transform GrabControllerTransform { get; private set; }
        /// <summary>A reference to the Transform used as a reference for the levers directions.</summary>
        public Transform LeverDirectionTransform { get { return m_LeverDirection != null ? m_LeverDirection : transform; } }

        /// <summary>The index for the gear entry this lever is currently on.</summary>
        public int CurrentIndex
        {
            get { return m_CurrentIndex; }
            set
            {
                // Store last position index and invoke any relevant event(s).
                LeverPositionEntry lastLeverPositionEntry = CurrentLeverPositionEntry;
                int lastLeverPositionIndex = m_CurrentIndex;
                if (lastLeverPositionEntry != null)
                    lastLeverPositionEntry.ChangedFrom?.Invoke();

                // Set 'current index'.
                m_CurrentIndex = value;

                // Invoke the relevant event(s) for the new lever position.
                if (CurrentLeverPositionEntry != null)
                    CurrentLeverPositionEntry.ChangedTo?.Invoke();

                // Invoke the 'OnLeverPosChanged' callback if the gear actually changed.
                if (lastLeverPositionEntry != CurrentLeverPositionEntry)
                    OnLeverChanged(lastLeverPositionIndex, lastLeverPositionEntry, m_CurrentIndex, CurrentLeverPositionEntry);
            }
        }
        /// <summary>Returns the LeverPositionEntry associated with the CurrentIndex or null if none.</summary>
        public LeverPositionEntry CurrentLeverPositionEntry { get { return leverPositions != null && leverPositions.Length > CurrentIndex ? leverPositions[CurrentIndex] : null; } }
        /// <summary>The next Time.time the lever will return to it's 'return to' position if configured while not being grabbed and has not yet returned.</summary>
        public float LeverReturnTime { get; private set; }

        /// <summary>The hidden backing field for the 'CurrentIndex' property.</summary>
        int m_CurrentIndex;

        // Unity callback(s).
        void Awake()
        {
            // Ensure leverPivot is not null.
            if (leverPivot == null)
                leverPivot = transform;

            // Validate 'startLeverPositionIndex'.
            if (startPositionIndex < 0 || startPositionIndex >= leverPositions.Length)
            {
                Debug.LogWarning("The 'startPositionINdex' field of the KinematicLeverGadget component was out-of-bounds! Defaulting to index 0.", gameObject);
                startPositionIndex = 0;
            }

            // Set default 'current index'.
            CurrentIndex = startPositionIndex;
        }

        void Start()
        {
            // If starting with a valid lever position entry...
            if (CurrentLeverPositionEntry != null)
            {
                // Start lever with correct rotation.
                Vector3 newLocalEulerAngles = VectorMath.VectorFrom180To360(CurrentLeverPositionEntry.leverPivotEulerAngles);
                leverPivot.transform.localEulerAngles = new Vector3(
                    leverDisableRotations.x ? leverPivot.transform.localEulerAngles.x : newLocalEulerAngles.x,
                    leverDisableRotations.y ? leverPivot.transform.localEulerAngles.y : newLocalEulerAngles.y,
                    leverDisableRotations.z ? leverPivot.transform.localEulerAngles.z : newLocalEulerAngles.z
                );

                // Start lever in correct position.
                leverPivot.transform.localPosition = CurrentLeverPositionEntry.leverPivotPosition;
            }

            // Manually invoke the first 'OnLeverPosChanged' callback.
            OnLeverChanged(-1, null, CurrentIndex, CurrentLeverPositionEntry);
        }

        void Update()
        {
            // Handle shifting if being grabbed.
            if (GrabControllerTransform != null)
            {
                // Ensure controller has moved enough in shift direction (or opposite to it) to shift.
                Vector3 localGrabControllerPos = LeverDirectionTransform.InverseTransformPoint(GrabControllerTransform.position);
                float signedDistance = FloatMath.GetSignedDistanceInDirection(shiftControllerDirection, localGrabControllerPos, ControllerLocalPositionLastShift);
                if (Mathf.Abs(signedDistance) >= shiftControllerDistance)
                {
                    // Check if a shift can be performed in the desired direction.
                    if ((signedDistance > 0 && CurrentIndex > 0) || (signedDistance < 0 && CurrentIndex < leverPositions.Length - 1))
                    {
                        // Shift gear in desired direction.
                        if (signedDistance > 0)
                            --CurrentIndex;
                        else if (signedDistance < 0)
                            ++CurrentIndex;

                        // Update last shift local position.
                        ControllerLocalPositionLastShift = localGrabControllerPos;
                    }
                }
            }
            // Handle not being grabbed...
            else
            {
                // Handle 'return to' if configured.
                if (returnToIndexOnRelease >= 0 && LeverReturnTime != float.NegativeInfinity)
                {
                    // Check if it is time for the lever to return.
                    if (Time.time >= LeverReturnTime)
                    {
                        // Set return position.
                        CurrentIndex = returnToIndexOnRelease;

                        // Never return since it just did.
                        LeverReturnTime = float.NegativeInfinity;
                    }
                }
            }

            // Rotate leverPivot to target rotation at leverRotationRate degrees per second.
            Vector3 newLocalEulerAngles = Vector3.RotateTowards(leverPivot.transform.localEulerAngles, VectorMath.VectorFrom180To360(CurrentLeverPositionEntry.leverPivotEulerAngles), (leverRotationRate * Mathf.Deg2Rad) * Time.deltaTime, float.PositiveInfinity);
            leverPivot.transform.localEulerAngles = new Vector3(
                leverDisableRotations.x ? leverPivot.transform.localEulerAngles.x : newLocalEulerAngles.x,
                leverDisableRotations.y ? leverPivot.transform.localEulerAngles.y : newLocalEulerAngles.y,
                leverDisableRotations.z ? leverPivot.transform.localEulerAngles.z : newLocalEulerAngles.z
            );

            // Move leverPivot to target position at leverMoveRotate units per second.
            leverPivot.transform.localPosition = Vector3.MoveTowards(leverPivot.transform.localPosition, CurrentLeverPositionEntry.leverPivotPosition, leverMoveRate * Time.deltaTime);
        }

        // Public method(s).
        /// <summary>A public method to set the 'CurrentIndex' parameter of this component.</summary>
        /// <param name="pIndex"></param>
        public void SetLeverPosIndex(int pIndex)
        {
            CurrentIndex = pIndex;
        }

        // IGrabbable interface implementation.
        public void OnGrabbed(Transform pControllerTransform, ControllerSide pControllerSide)
        {
            GrabControllerTransform = pControllerTransform;
            ControllerLocalPositionLastShift = LeverDirectionTransform.InverseTransformPoint(pControllerTransform.position);
        }

        public void OnReleased(Transform pControllerTransform, ControllerSide pControllerSide)
        {
            GrabControllerTransform = null;

            // Handle 'return to'.
            if (returnToIndexOnRelease >= 0)
            {
                // Handle timed return to.
                if (returnToIndexAfterSeconds > 0)
                {
                    // Time delayed return.
                    LeverReturnTime = Time.time + returnToIndexAfterSeconds;
                }
                else
                {
                    // Instant return.
                    CurrentIndex = returnToIndexOnRelease;
                }
            }
        }

        // Private callback(s).
        /// <summary>Invoked when this lever changes gears.</summary>
        /// <param name="pLastLeverPosIndex"></param>
        /// <param name="pLastLeverPos">The last LeverPositionEntry that was shifted from, or null.</param>
        /// <param name="pNewLeverPosIndex"></param>
        /// <param name="pNewLeverPos">The LeverPositionEntry that was shifted to, or null.</param>
        void OnLeverChanged(int pLastLeverPosIndex, LeverPositionEntry pLastLeverPos, int pNewLeverPosIndex, LeverPositionEntry pNewLeverPos)
        {
            // Invoke the 'lever changed' unity event.
            LeverChanged?.Invoke(pLastLeverPosIndex, pLastLeverPos, pNewLeverPosIndex, pNewLeverPos);
        }
    }
}
