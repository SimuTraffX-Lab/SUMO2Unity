using System;
using UnityEngine;

namespace VRDriving.VehicleSystem
{
    /// <summary>
    /// A component that allows for information about a wheel to be defined.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class VehicleWheel : MonoBehaviour
    {
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

        // Side.
        [Serializable]
        public enum Side
        {
            Center,
            Left,
            Right
        }

        // VehicleWheel.
        [Tooltip("Does this wheel have a motor?")]
        public bool motor;
        [Tooltip("Does this wheel steer?")]
        public bool steering;
        [Tooltip("The VehicleWheel.Side the wheel is on.")]
        public Side wheelSide;
        [Min(0f)]
        [Tooltip("A multiplier for the brake strength for the given wheel.")]
        public float brakeStrengthMultiplier = 1f;
        [Tooltip("An array of references to wheel visual Transforms.")]
        public Transform[] wheelTransforms;
        [Tooltip("A reference to this wheel's collider.")]
        public WheelCollider wheelCollider;
        [Tooltip("Allows you to disable visual y-axis position modifications by the wheel collider. (x, z positions are already ignored by default.)")]
        public bool disableYPosition;
        [Tooltip("Allows you to disable visual rotations around specified axes by the wheel collider.")]
        public AxisSettings disableRotations;
    }
}
