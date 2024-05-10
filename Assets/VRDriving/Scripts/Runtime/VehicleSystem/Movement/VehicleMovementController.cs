using UnityEngine;
using VRDriving.Movement;

namespace VRDriving.VehicleSystem
{
    /// <summary>The movement controller class for the VRDriving demo.</summary>
    /// Author: Intuitive Gaming Solutions
    public class VehicleMovementController : IMovementController
    {
        [Header("Settings")]
        [Tooltip("A reference to the default mover for this movement controller.")]
        public IMover defaultMover;

        // Unity callback(s).
        void Awake()
        {
            // Push default mover to mover stack.
            if (defaultMover != null)
                PushMover(defaultMover);
        }

        void Reset()
        {
            // Look for default mover.
            if (defaultMover == null)
                defaultMover = GetComponent<IMover>();
        }
    }
}
