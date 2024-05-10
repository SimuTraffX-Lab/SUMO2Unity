using UnityEngine;

namespace VRDriving.Movement
{
    /// <summary>
    /// A generic interface that defines the structure of a 'mover' type.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public abstract class IMover : MonoBehaviour
    {
        [Header("Input Management")]
        [Tooltip("Should this mover simulate inputs?")]
        public bool simulateInputs = true;

        // Public overrideable method(s).
        /// <summary>
        /// Given a set of inputs, simulates the movement for the inputs and invokes a 'Moved' event to allow things, like a network movement manager for example, access the inputs.
        /// </summary>
        /// <param name="pInputs"></param>
        public abstract void Move(MovementInputs pInputs);

        /// <summary>
        /// Carries out movement of the MovementController based on the given inputs.
        /// </summary>
        /// <returns>
        /// Vector3, the movement that was applied to the MovementController simulation.
        /// </returns>
        public abstract Vector3 Simulate(MovementInputs pInputs);

        /// <summary>
        /// Gathers movement inputs for the mover.
        /// </summary>
        /// <returns>MovementInputs containing the movement inputs for the mover.</returns>
        public abstract MovementInputs GatherMovementInputs();
    }
}
