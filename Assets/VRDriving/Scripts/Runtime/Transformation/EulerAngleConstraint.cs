using System;
using UnityEngine;

namespace VRDriving.Transformation
{
    /// <summary>
    /// A component that constraints the eulerAngles of a transform.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class EulerAngleConstraint : MonoBehaviour
    {
        // ConstraintSpace.
        [Serializable]
        public enum ConstraintSpace
        {
            Local,
            World
        }

        // FloatMinMax.
        [Serializable]
        public struct FloatMinMax
        {
            [Tooltip("The minimum value in the range.")]
            public float minimum;
            [Tooltip("The maximum value in the range.")]
            public float maximum;
        }

        // PositionConstraint.
        [Header("Settings")]
        [Tooltip("The transform to constrain.")]
        [SerializeField] protected Transform m_Transform;

        [Header("Constraints")]
        [Tooltip("What space should the eulerAngles be constrained in?")]
        public ConstraintSpace constraintSpace;
        [Tooltip("Should the x-axis angle be constrained?")]
        public bool constrainX;
        [Tooltip("The minimum and maximum value for the transform's x-axis angle.")]
        public FloatMinMax xLimits;
        [Tooltip("Should the y-axis angle be constrained?")]
        public bool constrainY;
        [Tooltip("The minimum and maximum value for the transform's y-axis angle.")]
        public FloatMinMax yLimits;
        [Tooltip("Should the z-axis angle be constrained?")]
        public bool constrainZ;
        [Tooltip("The minimum and maximum value for the transform's z-axis angle.")]
        public FloatMinMax zLimits;

        // Unity callback(s).
        void FixedUpdate()
        {
            // Constraint the transform's position.
            switch (constraintSpace)
            {
                case ConstraintSpace.Local:
                    transform.localEulerAngles = new Vector3(
                        constrainX ? Mathf.Clamp(transform.localEulerAngles.x, xLimits.minimum, xLimits.maximum) : transform.localEulerAngles.x,
                        constrainY ? Mathf.Clamp(transform.localEulerAngles.y, yLimits.minimum, yLimits.maximum) : transform.localEulerAngles.y,
                        constrainZ ? Mathf.Clamp(transform.localEulerAngles.z, zLimits.minimum, zLimits.maximum) : transform.localEulerAngles.z
                    );
                    break;
                case ConstraintSpace.World:
                    transform.position = new Vector3(
                        constrainX ? Mathf.Clamp(transform.eulerAngles.x, xLimits.minimum, xLimits.maximum) : transform.eulerAngles.x,
                        constrainY ? Mathf.Clamp(transform.eulerAngles.y, yLimits.minimum, yLimits.maximum) : transform.eulerAngles.y,
                        constrainZ ? Mathf.Clamp(transform.eulerAngles.z, zLimits.minimum, zLimits.maximum) : transform.eulerAngles.z
                    );
                    break;
                default:
                    Debug.LogWarning("Unhandled constraint space '" + constraintSpace.ToString() + "'!");
                    break;
            }
        }
    }
}
