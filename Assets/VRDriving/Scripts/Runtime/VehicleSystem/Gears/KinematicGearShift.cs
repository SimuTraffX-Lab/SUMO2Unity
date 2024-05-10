using System;
using UnityEngine;
using UnityEngine.Events;
using VRDriving.Grabbing;
using VRDriving.Math;

namespace VRDriving.VehicleSystem
{
    /// <summary>
    /// A component that allows a Vehicle's gear to be modified using a kinematic gear shifter.
    /// </summary>
    /// Author: Mathew Aloisio
    public class KinematicGearShift : MonoBehaviour, IGrabbable
    {
        // GearShiftUnityEvent.
        /// <summary>
        /// Arg0: GearEntry - The GearEntry that was shifted from, or null.
        /// Arg1: GearEntry - The GearEntry htat was shifted to, or null.
        /// </summary>
        [Serializable]
        public class GearShiftUnityEvent : UnityEvent<GearEntry, GearEntry> { }

        // GearEntry.
        [Serializable]
        public class GearEntry
        {
            [Tooltip("The gear this entry represents.")]
            public Vehicle.Gear gear;
            [Tooltip("The target local position of the shifterPivot in local space when shifted to this gear.")]
            public Vector3 shifterPivotPosition;
            [Tooltip("The target local euler angles of the shifterPivot in local space when shifted to this gear. (-180f to 180f, same as seen in editor.)")]
            public Vector3 shifterPivotEulerAngles;

            [Tooltip("An event that is invoked when this gear entry is shifted to.")]
            public UnityEvent ShiftedTo;
            [Tooltip("An event that is invoked when this gear entry is shifted from.")]
            public UnityEvent ShiftedFrom;
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

        // KinematicGearShift.
        [Header("Settings - Vehicle")]
        [Tooltip("A reference to the Vehicle whose gear will be controlled.")]
        public Vehicle vehicle;
        [Tooltip("An array of possible GearEntrys this shifter can shift between.")]
        public GearEntry[] gears;

        [Header("Settings - Controller")]
        [Tooltip("Moving the controller along this direction shifts gears downs, moving it opposite of this direction shifts gears up. (Local space relative to ShifterDirectionTransform.)")]
        public Vector3 shiftControllerDirection = Vector3.forward;
        [Tooltip("The distance the shifting controller must move along the shiftControllerAxis to shift one gear.")]
        public float shiftControllerDistance = 0.05f;

        [Header("Settings - Shifter")]
        [Tooltip("The pivot for the gear shift.")]
        public Transform shifterPivot;
        [Tooltip("(Optional) The Transform that controls the direction for the shifter. If null 'transform' is used as a fallback.")]
        [SerializeField] Transform m_ShifterDirection;
        [Tooltip("The speed the gear shift moves to it's target position in units per second. (units/sec)")]
        public float shifterMoveRate = 0.2f;
        [Tooltip("The speed the gear shift rotates to it's target orientation in degrees per second. (degrees/sec)")]
        public float shifterRotationRate = 200f;
        [Tooltip("Allows shifterPivot rotations around certain axis to be disabled.")]
        public AxisSettings shifterDisableRotations;

        [Header("Eventts - Shifter")]
        [Tooltip("An event that is invoked when this shifter changes gears.\n\nArg0: GearEntry - The last GearEntry that was shifted from, or null.\nArg1: GearEntry - The GearEntry that was shifted to, or null.")]
        public GearShiftUnityEvent GearChanged;

        /// <summary>The local position (relative to ShifterDirectionTransform) of the last controller to grab the shifter, at the time it grabbed the shifter, or at the time of last shift whichever came last.</summary>
        public Vector3 ControllerLocalPositionLastShift { get; private set; }
        /// <summary>A reference to the Transform of the controller currently grabbing the gear shift, otherwise null.</summary>
        public Transform GrabControllerTransform { get; private set; }
        /// <summary>A reference to the Transform used as a reference for the shifters directions.</summary>
        public Transform ShifterDirectionTransform { get { return m_ShifterDirection != null ? m_ShifterDirection : transform; } }

        /// <summary>The index for the gear entry this shifter is currently on.</summary>
        public int CurrentGearIndex
        {
            get { return m_CurrentGearIndex; }
            set
            {
                // Store last gear entry and if non-null invoke relevant event(s).
                GearEntry lastGearEntry = CurrentGearEntry;
                if (lastGearEntry != null)
                    lastGearEntry.ShiftedFrom?.Invoke();

                // Update gear index.
                m_CurrentGearIndex = value;

                // Invoke the relevant event(s) on new gear.
                if (CurrentGearEntry != null)
                    CurrentGearEntry.ShiftedTo?.Invoke();

                // Invoke the 'OnGearChanged' callback if the gear actually changed.
                if (lastGearEntry != CurrentGearEntry)
                    OnGearChanged(lastGearEntry, CurrentGearEntry);
            }
        }
        /// <summary>Returns the GearEntry associated with the CurrentGearIndex or null if none.</summary>
        public GearEntry CurrentGearEntry { get { return gears != null && gears.Length > CurrentGearIndex ? gears[CurrentGearIndex] : null; } }

        /// <summary>The hidden backing field for the 'CurrentGearIndex' property.</summary>
        int m_CurrentGearIndex;

        // Unity callback(s).
        void Awake()
        {
            // Ensure shifterPivot is not null.
            if (shifterPivot == null)
                shifterPivot = transform;

            // Set default 'current gear index'.
            CurrentGearIndex = 0;
        }
        
        void Start()
        {
            // If starting with a valid gear entry...
            if (CurrentGearEntry != null)
            {
                // Start shifter with correct rotation.
                Vector3 newLocalEulerAngles = VectorMath.VectorFrom180To360(CurrentGearEntry.shifterPivotEulerAngles);
                shifterPivot.transform.localEulerAngles = new Vector3(
                    shifterDisableRotations.x ? shifterPivot.transform.localEulerAngles.x : newLocalEulerAngles.x,
                    shifterDisableRotations.y ? shifterPivot.transform.localEulerAngles.y : newLocalEulerAngles.y,
                    shifterDisableRotations.z ? shifterPivot.transform.localEulerAngles.z : newLocalEulerAngles.z
                );

                // Start lever in correct position.
                shifterPivot.transform.localPosition = CurrentGearEntry.shifterPivotPosition;
            }

            // Manually invoke the first 'OnGearChanged' callback.
            OnGearChanged(null, CurrentGearEntry);
        }

        void Update()
        {
            // Handle shifting if being grabbed.
            if (GrabControllerTransform != null)
            {
                // Ensure controller has moved enough in shift direction (or opposite to it) to shift.
                Vector3 localGrabControllerPos = ShifterDirectionTransform.InverseTransformPoint(GrabControllerTransform.position);
                float signedDistance = FloatMath.GetSignedDistanceInDirection(shiftControllerDirection, localGrabControllerPos, ControllerLocalPositionLastShift);
                if (Mathf.Abs(signedDistance) >= shiftControllerDistance)
                {
                    // Check if a shift can be performed in the desired direction.
                    if ((signedDistance > 0 && CurrentGearIndex > 0) || (signedDistance < 0 && CurrentGearIndex < gears.Length - 1))
                    {
                        // Shift gear in desired direction.
                        if (signedDistance > 0)
                            --CurrentGearIndex;
                        else if (signedDistance < 0)
                            ++CurrentGearIndex;

                        // Update last shift local position.
                        ControllerLocalPositionLastShift = localGrabControllerPos;
                    }
                }
            }

            // Rotate shifterPivot to target rotation at shifterRotationRate degrees per second.
            Vector3 newLocalEulerAngles = Vector3.RotateTowards(shifterPivot.transform.localEulerAngles, VectorMath.VectorFrom180To360(CurrentGearEntry.shifterPivotEulerAngles), (shifterRotationRate * Mathf.Deg2Rad) * Time.deltaTime, float.PositiveInfinity);
            shifterPivot.transform.localEulerAngles = new Vector3(
                shifterDisableRotations.x ? shifterPivot.transform.localEulerAngles.x : newLocalEulerAngles.x,
                shifterDisableRotations.y ? shifterPivot.transform.localEulerAngles.y : newLocalEulerAngles.y,
                shifterDisableRotations.z ? shifterPivot.transform.localEulerAngles.z : newLocalEulerAngles.z
            );

            // Move shifterPivot to target position at shifterMoveRotate units per second.
            shifterPivot.transform.localPosition = Vector3.MoveTowards(shifterPivot.transform.localPosition, CurrentGearEntry.shifterPivotPosition, shifterMoveRate * Time.deltaTime);
        }

        // Public method(s).
        /// <summary>A public method to set the 'CurrentGearIndex' parameter of this component.</summary>
        /// <param name="pIndex"></param>
        public void SetGearIndex(int pIndex)
        {
            CurrentGearIndex = pIndex;
        }

        // IGrabbable interface implementation.
        public void OnGrabbed(Transform pControllerTransform, ControllerSide pControllerSide)
        {
            GrabControllerTransform = pControllerTransform;
            ControllerLocalPositionLastShift = ShifterDirectionTransform.InverseTransformPoint(pControllerTransform.position);
        }

        public void OnReleased(Transform pControllerTransform, ControllerSide pControllerSide)
        {
            GrabControllerTransform = null;
        }

        // Private callback(s).
        /// <summary>Invoked when this shifter changes gears.</summary>
        /// <param name="pLastGear">The last GearEntry that was shifted from, or null.</param>
        /// <param name="pNewGear">The GearEntry that was shifted to, or null.</param>
        void OnGearChanged(GearEntry pLastGear, GearEntry pNewGear)
        {
            // Invoke the 'gear changed' unity event.
            GearChanged?.Invoke(pLastGear, pNewGear);

            // Set vehicle gear.
            if (vehicle != null)
                vehicle.gear = pNewGear.gear;
        }
    }
}
