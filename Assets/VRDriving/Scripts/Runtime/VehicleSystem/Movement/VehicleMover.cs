using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using VRDriving.Movement;

namespace VRDriving.VehicleSystem
{
    /// <summary>
    /// The basic vehicle mover type.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class VehicleMover : IMover
    {
        // MovementInputUnityEvent.
        [Serializable]
        public class MovementInputsUnityEvent : UnityEvent<MovementInputs> { }

        // VehicleMover.
        [Header("Settings")]
		[Tooltip("The vehicle being moved.")]
        public Vehicle vehicle;

        [Header("Control Settings")]
        [Tooltip("The driving hand currently being used for this vehicle mover.")]
        public ControllerSide useDrivingHand;
        [Tooltip("The 'prefered' driving hand, this should be set to the opposite of the hand a player prefers shooting with.")]
        public ControllerSide preferredDrivingHand;
        [Tooltip("When true holding the brake while not moving forwards will cause the vehicle to reverse.")]
        public bool allowBrakeToReverse = true;

        [Header("Inputs - Right Hand")]
        [Tooltip("The input the movement controller will use for acceleration.")]
        [SerializeField] protected InputActionProperty m_RightHandAccelerateInput;
        [Tooltip("The input the movement controller will use for braking.")]
        [SerializeField] protected InputActionProperty m_RightHandBrakeInput;
        [Header("Inputs - Left Hand")]
        [Tooltip("The input the movement controller will use for acceleration.")]
        [SerializeField] protected InputActionProperty m_LeftHandAccelerateInput;
        [Tooltip("The input the movement controller will use for braking.")]
        [SerializeField] protected InputActionProperty m_LeftHandBrakeInput;

        [Header("Events")]
        public MovementInputsUnityEvent InputsGathered;

        /// <summary>
        /// A reference to the InputActionProperty currently being used for acceleration.
        /// </summary>
        public InputActionProperty AccelerateInput { get; protected set; }

        /// <summary>
        /// A reference to the InputActionProperty currently being used for braking.
        /// </summary>
        public InputActionProperty BrakeInput { get; protected set; }

        MovementInputs m_Inputs = new MovementInputs();
        Vector3 m_LastVehiclePosition;
        ControllerSide m_LastBoundDrivingInputs;
        bool m_IsBrakeToReversing;

        // Unity callback(s).
        void Awake()
        {
            // Set 'useDrivingHand' to preferred hnad by default.
            useDrivingHand = preferredDrivingHand;
        }

        void Start()
        {
            if (vehicle != null)
            {
                m_LastVehiclePosition = vehicle.transform.position;
            }
            else { Debug.LogWarning("No 'vehicle' set for VehicleMover attached to gameObject '" + gameObject.name + "'."); }
        }
        void OnEnable()
        {
            // Bind actions.
            BindActions();
        }

        void OnDisable()
        {
            // Unbind actions.
            UnbindActions();
        }

        void Update()
        {
			// Determine 'accelerationInput'.
			float accelerationInput = m_Inputs.accelerate;
            if (vehicle.gear == Vehicle.Gear.Reverse) // If the vehicle is in the gear 'reverse' flip the acceleration input.
            {
                accelerationInput *= -(vehicle.reverseAccelerationMultiplier);
            }
            else
            {
                // Brake key reverse.
                if (allowBrakeToReverse)
                {
                    // Decide whether to enter brake-to-reverse mode or not.
                    if (m_IsBrakeToReversing)
                    {
                        // Check if we're still in brake to reverse mode.
                        if (vehicle.LocalVelocity.z < -0.025f)
                        {
                            // Set acceleration input to the negative value of brake input to go backwards.
                            accelerationInput = -(m_Inputs.brake * vehicle.reverseAccelerationMultiplier);

                            // Since we're brake-to-reversing zero the brake input for this frame.
                            m_Inputs.brake = m_Inputs.accelerate;
                        }
                        else { m_IsBrakeToReversing = false; }                       
                    }
                    else if (vehicle.LocalVelocity.z < 0.05f && m_Inputs.brake > 0f) // Check if the paramters are met to enter brake to revese mode.
                    {
                        // Enter brake to reverse mode.
                        m_IsBrakeToReversing = true;

                        // Set acceleration input to the negative value of brake input to go backwards.
                        accelerationInput = -(m_Inputs.brake * vehicle.reverseAccelerationMultiplier);

                        // Since we're brake-to-reversing zero the brake input for this frame.
                        m_Inputs.brake = m_Inputs.accelerate;
                    }
                }
            }

            // Determine motor torque.
            float motor = vehicle.maxMotorTorque * accelerationInput;

            // Apply (or unapply) brakes to all wheels at once, outside of the loop, if the parking brake is not enabled.
            if (!vehicle.enableParkingBrake)
                vehicle.BrakeByInput(m_Inputs.brake);

            // Update vehicle acceleration.
            vehicle.Accelerate(motor);
        }

        // Public method(s).
        /// <summary>
        /// A public method that will unapply all vehicle acceleration and braking inputs and allow the vehicle to begin decelerating.
        /// This can be used with things like unity events (for example to make the vehicle stop when the player dies).
        /// </summary>
        public void UnapplyInputs()
        {
            // Reset braking.
            vehicle.BrakeByInput(0f);

            // Reset wheel colliders.
            foreach (Vehicle.WheelInfo wheel in vehicle.wheels)
            {
                vehicle.Accelerate(wheel, 0); // Disable acceleration for wheel.
            }
        }

        // Public overrided method(s).
        public override void Move(MovementInputs pInputs)
        {
            if (vehicle != null)
                Simulate(pInputs);
        }

        public override Vector3 Simulate(MovementInputs pInputs)
        {
            // Set m_Inputs.
            m_Inputs = pInputs;

            // Calculate the vehicle position delta (change in vehicle position since last simulation).
            Vector3 vehiclePositionDelta = vehicle.transform.position - m_LastVehiclePosition;
            m_LastVehiclePosition = vehicle.transform.position;

            return vehiclePositionDelta;
        }

        public override MovementInputs GatherMovementInputs()
        {
            // Re-assign driving inputs if they weren't set last time.
            if (m_LastBoundDrivingInputs != useDrivingHand)
            {
                AssignDrivingInputs();
                m_LastBoundDrivingInputs = useDrivingHand;
            }

            // Gather inputs via polling.
            MovementInputs inputs = new MovementInputs();
            inputs.accelerate = AccelerateInput != null ? AccelerateInput.action.ReadValue<float>() : 0f;
            inputs.brake = BrakeInput != null ? BrakeInput.action.ReadValue<float>() : 0f;

            // Invoke the inputs gathered event.
            InputsGathered?.Invoke(inputs);

            return inputs;
        }

        // Protected method(s).
        protected void BindActions()
        {
            // Enable inputs to allow for polling.
            // Right hand.
            if (m_RightHandAccelerateInput != null)
                m_RightHandAccelerateInput.action.Enable();
            if (m_RightHandBrakeInput != null)
                m_RightHandBrakeInput.action.Enable();
            // Left hand.
            if (m_LeftHandAccelerateInput != null)
                m_LeftHandAccelerateInput.action.Enable();
            if (m_LeftHandBrakeInput != null)
                m_LeftHandBrakeInput.action.Enable();

            // Assign default driving inputs.
            AssignDrivingInputs();
            m_LastBoundDrivingInputs = useDrivingHand;
        }

        protected void UnbindActions()
        {
            // Disable inputs.
            // Right hand.
            if (m_RightHandAccelerateInput != null)
                m_RightHandAccelerateInput.action.Disable();
            if (m_RightHandBrakeInput != null)
                m_RightHandBrakeInput.action.Disable();
            // Left hand.
            if (m_LeftHandAccelerateInput != null)
                m_LeftHandAccelerateInput.action.Enable();
            if (m_LeftHandBrakeInput != null)
                m_LeftHandBrakeInput.action.Enable();
        }
        
        /// <summary>A function that selects the based driving inputs based on the 'useDrivingHand' field.</summary>
        protected void AssignDrivingInputs()
        {
            // Bind driving inputs to the appropriate hand.
            if (useDrivingHand == ControllerSide.Left)
            {
                AccelerateInput = m_LeftHandAccelerateInput;
                BrakeInput = m_LeftHandBrakeInput;
            }
            else
            {
                AccelerateInput = m_RightHandAccelerateInput;
                BrakeInput = m_RightHandBrakeInput;
            }
        }
    }
}
