using UnityEngine;

namespace VRDriving.VehicleSystem.Braking
{
    /// <summary>
    /// A component that provides public methods to control a Vehicle component's wheels' braking based on separate left and right side braking inputs.
    /// 
    /// NOTE: This component IS NOT intended to work with any 'VehicleMover' component as they already manage braking.
    /// </summary>
    [RequireComponent(typeof(Vehicle))]
    public class VehicleLeftRightBraking : MonoBehaviour
    {
        /// <summary>A reference to the Vehicle component associated with this component.</summary>
        public Vehicle Vehicle { get; private set; }

        // Unity callback(s).
        void Awake()
        {
            Vehicle = GetComponent<Vehicle>();
        }

        // Public method(s).
        /// <summary>Sets the braking input for all wheels in a Vehicle that have their wheelSide set to VehicleWheel.Side.Left</summary>
        /// <param name="pBraking">0f to 1f</param>
        public void SetLeftBrakingInput(float pBraking)
        {
            foreach (Vehicle.WheelInfo wheelInfo in Vehicle.wheels)
            {
                if (wheelInfo.component.wheelSide == VehicleWheel.Side.Left)
                {
                    Vehicle.BrakeByInput(wheelInfo, pBraking);
                }
            }
        }

        /// <summary>Sets the braking input for all wheels in a Vehicle that have their wheelSide set to VehicleWheel.Side.Right</summary>
        /// <param name="pBraking">0f to 1f</param>
        public void SetRightBrakingInput(float pBraking)
        {
            foreach (Vehicle.WheelInfo wheelInfo in Vehicle.wheels)
            {
                if (wheelInfo.component.wheelSide == VehicleWheel.Side.Right)
                {
                    Vehicle.BrakeByInput(wheelInfo, pBraking);
                }
            }
        }
    }
}
