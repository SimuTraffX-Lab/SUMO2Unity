using UnityEngine;
using VRDriving.Steering;

namespace VRDriving.Demo
{
    /// <summary>
    /// A demo component that shows how steering mechanisms and levers can be used for non-driving related things.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class ClawGameManager : MonoBehaviour
    {
        [Header("Settings - Steering")]
        [Tooltip("A reference to the VehicleGrabbableSteeringBase component associated with this component.")]
        public VehicleGrabbableSteeringBase steering;

        [Header("Settings - Claw")]
        [Tooltip("The maximum movement speed of the claw in units per second. (units/sec)")]
        public float maxClawSpeed = 1f;
        [Tooltip("A reference to the claws Transform.")]
        public Transform clawTransform;
        [Tooltip("A reference to the Transform that is the 'upper bound' position of the claw.")]
        public Transform clawUpperBoundTransform;
        [Tooltip("A reference to the Transform that is the 'lower bound' position of the claw.")]
        public Transform clawLowerBoundTransform;

        [Header("Settings - Claw Assembly")]
        [Tooltip("The maximum movement speed of the claw assembly in units per second. (units/sec)")]
        public float maxClawAsmSpeed = 1f;
        [Tooltip("A reference to the claw assemblys Transform.")]
        public Transform clawAsmTransform;
        [Tooltip("A reference to the Transform that is the 'upper bound' position of the claw assembly.")]
        public Transform clawAsmUpperBoundTransform;
        [Tooltip("A reference to the Transform that is the 'lower bound' position of the claw assembly.")]
        public Transform clawAsmLowerBoundTransform;

        /// <summary>Tracks the move direction for the claw assembly. (0 - no movement, 1 - towards upper bound, -1 - towards lower bound)</summary>
        public int ClawAssemblyMoveDirection { get; private set; } = 0;

        // Unity callback(s).
        void Update()
        {
            // Only move the claw if the steering mechanism is being held.
            if (steering.GrabbingControllersCount > 0)
            {
                // Only move the claw if the wheel is turned some way.
                if (steering.SteeringAngleMultiplier != 0)
                {
                    // Move the claw in the appropriate direction and speed.
                    if (steering.SteeringAngleMultiplier > 0)
                    {
                        // Move the claw towards lower bound.
                        clawTransform.position = Vector3.MoveTowards(clawTransform.position, clawLowerBoundTransform.position, (maxClawSpeed * steering.SteeringAngleMultiplier) * Time.deltaTime);
                    }
                    else
                    {
                        // Move the claw towards upper bound.
                        clawTransform.position = Vector3.MoveTowards(clawTransform.position, clawUpperBoundTransform.position, (maxClawSpeed * Mathf.Abs(steering.SteeringAngleMultiplier)) * Time.deltaTime);
                    }
                }
            }

            // Only handle claw assembly movement if move direction is non-zero.
            if (ClawAssemblyMoveDirection != 0)
            {
                if (ClawAssemblyMoveDirection > 0)
                {
                    // Move the claw assembly towards lower bound.
                    clawAsmTransform.position = Vector3.MoveTowards(clawAsmTransform.position, clawAsmLowerBoundTransform.position, maxClawAsmSpeed * Time.deltaTime);
                }
                else
                {
                    // Move the claw assembly towards upper bound.
                    clawAsmTransform.position = Vector3.MoveTowards(clawAsmTransform.position, clawAsmUpperBoundTransform.position, maxClawAsmSpeed * Time.deltaTime);
                }
            }
        }

        // Public method(s).
        /// <summary>Makes the claw assembly start moving the forward direction.</summary>
        public void MoveClawAssemblyForward() { ClawAssemblyMoveDirection = 1; }
        /// <summary>Makes the claw assembly start moving the backward direction.</summary>
        public void MoveClawAssemblyBackward() { ClawAssemblyMoveDirection = -1; }
        /// <summary>Stops moving the claw assembly.</summary>
        public void StopClawAssembly() { ClawAssemblyMoveDirection = 0; }
    }
}
