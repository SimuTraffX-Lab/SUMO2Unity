using UnityEngine;
using VRDriving.Steering;

namespace VRDriving.VehicleSystem
{
    /// <summary>
    /// A component that is attached to the same GameObject as a Vehicle component.
    /// The VehicleKinematicSteering component controls the relevant vehicle's steering angle using a VehicleSteeringBase component.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    [RequireComponent(typeof(Vehicle))]
    public class VehicleKinematicSteering : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("A reference to the VehicleSteeringBase component associated with this component.")]
        public VehicleSteeringBase steering;

        /// <summary>A reference to the Vehicle component associated with this component.</summary>
        public Vehicle Vehicle { get; private set; }

        // Unity callback(s).
        void Awake()
        {
            // Find component reference(s).
            Vehicle = GetComponent<Vehicle>();
        }

        void Update()
        {
            // Update the vehicle's steering angle.
            Vehicle.steeringAngle = Vehicle.maxSteeringAngle * steering.SteeringAngleMultiplier;
        }
    }
}
