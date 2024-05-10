using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections.Generic;

namespace VRDriving.VehicleSystem
{
    /// <summary>
    /// The base class for all vehicles.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    [RequireComponent(typeof(Rigidbody))]
    public class Vehicle : MonoBehaviour
    {
        // CollisionUnityEvent
        [Serializable]
        public class CollisionUnityEvent : UnityEvent<Collision> { }

        // WheelInfo.
        [Serializable]
        public class WheelInfo
        {
            [Tooltip("A reference to the 'VehicleWheel' component associated with this wheel.")]
            public VehicleWheel component;

            /// <summary>The acceleration being asked for on this wheel.</summary>
            internal float m_Acceleration;
        }   

        // Gear.
        [Serializable]
        public enum Gear
        {
            Forward,
            Reverse
        }

        // Vehicle.
        [Header("Interior")]
        [Tooltip("(Optional) A reference to the interior transform of the vehicle. (Generally one that is in the same vehicle prefab but follows the vehicle via a script as opposed to using physics.)")]
        public Transform interiorTransform;

        [Header("Wheels")]
        public List<WheelInfo> wheels;

        [Header("Engine")]
        [Tooltip("Is this vehicles engine already running at Awake()? (NOTE: 'EngineStarted' will not be invoked for cars whose engines start on already.)")]
        public bool awakeEngineOn = true;
        [Tooltip("The delay in seconds when starting the engine.")]
        public FloatMinMax engineStartDelay = new FloatMinMax() { minimum = 1.6f, maximum = 2.3f };

        [Header("Motor & Brakes")]
        [Tooltip("Is the parking brake enabled?")]
        public bool enableParkingBrake;
        [Range(0f, 1f)]
        [Tooltip("The strength to apply the brakes with while the parking brake is enabled. (0 = none / 1 = max)")]
        public float parkingBrakeStrength = 1f;
        [Tooltip("The maximum amount of motor torque to be applied to the vehicle's wheel colliders when the vehicle is fully accelerating.")]
        public float maxMotorTorque;
        [Tooltip("The maximum angle the vehicle can steer in either direction.")]
        public float maxSteeringAngle;
        [Tooltip("The torque to be applied to the brakes when the vehicle is fully braking.")]
        public float brakeTorque;
        [Tooltip("The deceleration force to apply to the vehicle every frame. (Similar to engine braking done by a vehicle in real life.)")]
        public float decelerationForce;
        [Min(0)]
        [Tooltip("A multiplier that determines what percent of normal acceleration the vehicle gets while reversing. (Min: 0 => no reversing | 1 => full acceleration input for reverse)")]
        public float reverseAccelerationMultiplier = 1f;

        [Header("Lights")]
        [Tooltip("A reference to the Transform(s) that is/are the parent of the vehicle's headlight(s).")]
        public Transform[] headLightContainers;
        [Tooltip("A reference to the Transform(s) that is/are the parent of the vehicle's brake light(s).")]
        public Transform[] brakeLightContainers;

        [Header("Drifting")]
        [Min(0)]
        [Tooltip("The minimum average forward velocity deviance required to start a drift.")]
        public float driftMinimumForwardVelocityDeviance = 0.55f;
        [Min(0)]
        [Tooltip("The number of seconds of grace-time before a drift is considered 'stopped'.")]
        public float driftStopDelay = 0.42f;

        [Header("Events - Drifting")]
        [Tooltip("An event that is invoked when the vehicle starts a drift.")]
        public UnityEvent DriftStarted;
        [Tooltip("An event that is invoked when the vehicle stops drifting.")]
        public UnityEvent DriftStopped;

        [Header("Events - Collision")]
        [Tooltip("A unity event that is invoked when the vehicle enters a collision with another body.")]
        public CollisionUnityEvent CollisionEntered;
        [Tooltip("A unity event that is invoked when the vehicle exits a collision with another body.")]
        public CollisionUnityEvent CollisionExited;

        [Header("Events - Engine")]
        [Tooltip("An event that is invoked when the engine starter is initiated.")]
        public UnityEvent StarterActivated;
        [Tooltip("An event that is invoked when the engine starter is stopped for any reason, including a succesful start.")]
        public UnityEvent StarterDeactivated;
        [Tooltip("An event that is invoked when the Vehicle's engine is started.")]
        public UnityEvent EngineStarted;
        [Tooltip("An event that is invoked when the Vehicle's engine is stopped.")]
        public UnityEvent EngineStopped;

        [Header("Control - Instance")]
        [Tooltip("The gear the vehicle is in.")]
        public Gear gear;
        [Tooltip("The current steering angle for the vehicle.")]
        public float steeringAngle;

        // General.
        /// <summary>A reference to the Rigidbody associated with this vehicle.</summary>
        public Rigidbody Rigidbody { get; private set; }

        // Engine and Ignition.
        /// <summary>Tracks whether or not this vehicles engine is currently running. Use StartEngine()/StopEngine() to modify the state directly.</summary>
        public bool IsEngineOn { get; protected set; }
        /// <summary>Tracks whether or not this vehicle is currently turning over/starting the engine. Use ActivateStarter()/DeactivateStarter() to modify the state directly.</summary>
        public bool IsStarterOn { get; protected set; }
        /// <summary>The Time.time the starter for the vehicle was last activated.</summary>
        public float StarterActivateTime { get; protected set; }
        /// <summary>The computed time the current start sequence (if there is on) will complete.</summary>
        public float StarterCompleteTime { get; protected set; }

        // Velocity and Speed.
        /// <summary>Returns the velocity of the vehicle.</summary>
        public Vector3 Velocity
        {
            get { return Rigidbody.velocity; }
        }
        /// <summary>Returns the vehicle's velocity in local space.</summary>
        public Vector3 LocalVelocity
        {
            get { return transform.InverseTransformDirection(Rigidbody.velocity); }
        }
        /// <summary>Returns the vehicles current speed in meters per second.</summary>
        public float CurrentSpeed
        {
            get { return Velocity.magnitude; }
        }
        /// <summary>Returns the vehicles current speed in kilometers per hour.</summary>
        public float CurrentSpeedInKmh
        {
            get { return CurrentSpeed * 3.6f; }
        }
        /// <summary>Returns the vehicles current speed in miles per hour.</summary>
        public float CurrentSpeedInMph
        {
            get { return CurrentSpeed * 2.2369f; }
        }
        /// <summary>Returns the average forward deviance from the VehicleSFX's vehicle, this is the average of (vehicle.LocalVelocity.magnitude - Mathf.Abs(vehicle.LocalVelocity.z)) over a number of frames.</summary>
        public float AverageForwardVelocityDeviance { get; protected set; }

        // Drifting.
        /// <summary>The last Time.time that the vehicle was detected as drifting.</summary>
        public float LastDriftTime { get; protected set; }
        /// <summary>The Time.time that a drift start was detected at.</summary>
        public float DriftStartTime { get; protected set; }
        /// <summary>Returns true if the vehicle is drifting, otherwise false.</summary>
        public bool IsDrifting { get; protected set; }
        /// <summary>Returns true if the vehicle is braking, otherwise false.</summary>
        public bool IsBraking { get; protected set; }
        /// <summary>Returns the last brake input argument (0f->1f) that was received by Vehicle.BrakeByInput(...)</summary>
        public float LastBrakeInput { get; protected set; }

        private float[] m_ForwardDevianceArray = new float[4]; // An array that holds average forward deviance over a given number of frames.

        // Unity callback(s).
        void Awake()
        {
            // Find the vehicle's rigidbody.
            Rigidbody = GetComponent<Rigidbody>();

            // If the engine starts on then ensure it is.
            if (awakeEngineOn)
                IsEngineOn = true;
        }

        void Update()
        {
            // Update average forward deviance.
            // 1. Shift array.
            AverageForwardVelocityDeviance = 0f;
            for (int i = m_ForwardDevianceArray.Length - 1; i >= 1; --i)
            {
                m_ForwardDevianceArray[i - 1] = m_ForwardDevianceArray[i];
                AverageForwardVelocityDeviance += m_ForwardDevianceArray[i - 1];
            }
            // 2. Insert the current deviance in the array.
            float forwardVelocityDeviance = LocalVelocity.magnitude - Mathf.Abs(LocalVelocity.z);
            m_ForwardDevianceArray[m_ForwardDevianceArray.Length - 1] = forwardVelocityDeviance;
            AverageForwardVelocityDeviance += forwardVelocityDeviance;
            // 3. Divide by the total number of values affecting the average forward deviance.
            AverageForwardVelocityDeviance /= m_ForwardDevianceArray.Length;

            // Check for a drift.
            if (AverageForwardVelocityDeviance >= driftMinimumForwardVelocityDeviance)
            {
                // Update the vehicle's drifitng status.
                if (!IsDrifting)
                {
                    IsDrifting = true;
                    DriftStartTime = Time.time;

                    // Invoke the drift started unity event.
                    DriftStarted?.Invoke();
                }

                // Update last drift time.
                LastDriftTime = Time.time;
            }
            else if (IsDrifting && Time.time - LastDriftTime >= driftStopDelay)
            {
                IsDrifting = false;

                // Invoke the drift stopped unity event.
                DriftStopped?.Invoke();
            }

            // Apply full braking if the parking brake is enabled.
            if (enableParkingBrake && parkingBrakeStrength > 0)
                BrakeByInput(parkingBrakeStrength);

            // Iterate over every wheel on the vehicle.
            foreach (WheelInfo wheel in wheels)
            {
                // Apply steering to steerable wheels.
                if (wheel.component.steering)
                    Steer(wheel, steeringAngle);

                // Apply deceleration to non-accelerated, non-braking wheels while the engine is running.
                if (wheel.component.motor && wheel.m_Acceleration == 0f && !IsBraking && IsEngineOn)
                {
                    Decelerate(wheel);
                }
                else if (!IsBraking) { wheel.component.wheelCollider.brakeTorque = 0; } // Stop decelerating if not braking.

                // Apply visuals to wheel.
                ApplyTransformToVisuals(wheel);
            }

            // Update starting sequence.
            if (IsStarterOn)
            {
                // If the engine isn't on check if its time to start.
                if (!IsEngineOn && Time.time >= StarterCompleteTime)
                {
                    // Start the vehicle's engine.
                    StartEngine();
                }
            }
        }

        void OnCollisionEnter(Collision pCollision)
        {
            // Invoke the 'CollisionEntered' unity event.
            CollisionEntered?.Invoke(pCollision);
        }

        void OnCollisionExit(Collision pCollision)
        {
            // Invoke the 'CollisionExited' unity event.
            CollisionExited?.Invoke(pCollision);
        }

        // Public method(s).
        /// <summary>
        /// For all wheels on all axles applies the colldier's position and rotation to the visual wheel.
        /// </summary>
        /// <param name="pInfo">The wheel information containing references to the collider and visual wheel.</param>
        public void ApplyTransformToVisuals(WheelInfo pInfo)
        {
            if (pInfo.component.wheelTransforms != null && pInfo.component.wheelCollider != null)
            {
                // 1. Gather collider position information.
                pInfo.component.wheelCollider.GetWorldPose(out Vector3 position, out Quaternion rotation);

                // 2. Apply collider position information to visual wheel.
                foreach (Transform wheelTransform in pInfo.component.wheelTransforms)
                {
                    Vector3 oldPosition = wheelTransform.position;
                    wheelTransform.transform.position = new Vector3(
                        oldPosition.x,
                        pInfo.component.disableYPosition ? oldPosition.y : position.y,
                        oldPosition.z
                    );

                    // Apply any rotation limits.
                    Vector3 oldEulerAngles = wheelTransform.eulerAngles;
                    Vector3 targetEulerAngles = rotation.eulerAngles;
                    wheelTransform.eulerAngles = new Vector3(
                        pInfo.component.disableRotations.x ? oldEulerAngles.x : targetEulerAngles.x,
                        pInfo.component.disableRotations.y ? oldEulerAngles.y : targetEulerAngles.y,
                        pInfo.component.disableRotations.z ? oldEulerAngles.z : targetEulerAngles.z
                    );
                }
            }
        }

        /// <summary>
        /// A function used to steer individual wheels on the vehicle.
        /// </summary>
        /// <param name="pWheel"></param>
        /// <param name="pSteering"></param>
        public void Steer(WheelInfo pWheel, float pSteering)
        {
            pWheel.component.wheelCollider.steerAngle = pSteering;
        }

        /// <summary>
        /// A function used to apply acceleration to the vehicle.
        /// Applies 0 acceleration if the engine is not running.
        /// </summary>
        /// <param name="pWheel"></param>
        /// <param name="pMotor"></param>
        public void Accelerate(WheelInfo pWheel, float pMotor)
        {
            // Only accelerate the vehicle if the engine is running.
            if (IsEngineOn)
            {
                pWheel.component.wheelCollider.motorTorque = pMotor;
                pWheel.m_Acceleration = pMotor;
            }
            // Otherwise acceleration should be 0.
            else
            {
                pWheel.component.wheelCollider.motorTorque = 0;
                pWheel.m_Acceleration = 0;
            }
        }

        /// <summary>
        /// A function used to apply acceleration to all motored wheels on the vehicle.
        /// </summary>
        /// <param name="pMotor"></param>
        public void Accelerate(float pMotor)
        {
            foreach (WheelInfo info in wheels)
            {
                if (info.component.motor)
                    Accelerate(info, pMotor);
            }
        }

        /// <summary>
        /// Accelerates the vehicle based on an acceleration input value between -1f and 1f.
        /// </summary>
        /// <param name="pInput">-1f to 1f</param>
        public void AccelerateByInput(float pInput)
        {
            Accelerate(maxMotorTorque * pInput);
        }

        /// <summary>
        /// A function used to apply deceleration to the vehicle (through brakeTorque) while not accelerating or braking.
        /// If the engine si not running this forces 'brakeTorque' for teh wheel to 0.
        /// </summary>
        /// <param name="pWheel"></param>
        public void Decelerate(WheelInfo pWheel)
        {
            if (IsEngineOn)
            {
                pWheel.component.wheelCollider.brakeTorque = decelerationForce;
            }
        }

        /// <summary>
        /// A function used to apply deceleration to all motored wheels on the vehicle.
        /// </summary>
        public void Decelerate()
        {
            foreach (WheelInfo info in wheels)
            {
                if (info.component.motor)
                    Decelerate(info);
            }
        }

        /// <summary>
        /// A function used to apply brakeTorque to the vehicle while braking.
        /// WARNING: This function does NOT update the 'IsBraking' or 'LastBrakeInput' field, use 'CheckBrakeStatus()' to update 'IsBraking'.
        /// </summary>
        /// <param name="pWheel"></param>
        /// <param name="pBrakeInput"></param>
        public void BrakeByInput(WheelInfo pWheel, float pBrakeInput)
        {
            pWheel.component.wheelCollider.brakeTorque = (brakeTorque * pWheel.component.brakeStrengthMultiplier) * pBrakeInput;
        }

        /// <summary>
        /// Intended for use after calling the BrakeByInput(WheelInfo pWheel, float pBrakeInput) variant of brake to update whether or not the vehicle is braking.
        /// NOTE: EVERY wheel has to be braking in order to be considered 'IsBraking' since someone may want to use individual wheel brakes for effects such as missing tire.
        /// </summary>
        public void CheckIsBraking()
        {
            foreach (WheelInfo info in wheels)
            {
                // Every wheel must be braking atleast 1 percent of it's set vehicle's brakeTorque * the wheels brakeStrengthMultiplier, otherwise set IsBreaking to false and return.
                if (info.component.wheelCollider.brakeTorque < (brakeTorque * info.component.brakeStrengthMultiplier) * 0.01f)
                {
                    IsBraking = false;
                    return;
                }
            }

            // All wheels fully braking, set IsBraking to true.
            IsBraking = true;
        }

        /// <summary>
        /// A function used to apply brakeTorque to all of the vehicle's wheels at once.
        /// </summary>
        /// <param name="pBrakeInput"></param>
        public void BrakeByInput(float pBrakeInput)
        {
            foreach (WheelInfo info in wheels)
            {
                if (pBrakeInput != 0 || LastBrakeInput != 0)
                    info.component.wheelCollider.brakeTorque = (brakeTorque * info.component.brakeStrengthMultiplier) * pBrakeInput;
            }

            // Update 'is braking' state and 'last brake input'.
            LastBrakeInput = pBrakeInput;
            IsBraking = pBrakeInput > 0f;
        }

        /// <summary>
        /// Given a direction returns the vehicle's velocity in the given direction.
        /// </summary>
        /// <param name="pDirection"></param>
        /// <returns>the vehicle's velocity in the given direction.</returns>
        public Vector3 GetVelocityInDirection(Vector3 pDirection)
        {
            return Vector3.Project(Rigidbody.velocity, pDirection);
        }

        // SECTION: Public lighting method(s).
        /// <summary>
        /// Sets the head lights state for the vehicle.
        /// </summary>
        /// <param name="pEnabled"></param>
        public void SetHeadLightsEnabled(bool pEnabled)
        {
            foreach (Transform t in headLightContainers)
            {
                t.gameObject.SetActive(pEnabled);
            }
        }

        /// <summary>
        /// Returns true if all headLightContainers are active in the scene hierachy.
        /// Returns false if there are no headLightContainers on the vehicle.
        /// </summary>
        /// <returns>true if the Vehicle's headlights are enabled, otherwise false./returns>
        public bool IsHeadLightsEnabled()
        {
            if (headLightContainers.Length > 0)
            {
                foreach (Transform t in headLightContainers)
                {
                    if (!t.gameObject.activeInHierarchy)
                        return false;
                }
                return true;
            }

            return false; // Vehicle has no headlights.
        }

        /// <summary>
        /// Sets the brake lights state for the vehicle.
        /// </summary>
        /// <param name="pEnabled"></param>
        public void SetBrakeLightsEnabled(bool pEnabled)
        {
            foreach (Transform t in brakeLightContainers)
            {
                t.gameObject.SetActive(pEnabled);
            }
        }

        /// <summary>
        /// Returns true if all brakeLightContainers are active in the scene hierachy.
        /// Returns false if there are no brakeLightContainers on the vehicle.
        /// </summary>
        /// <returns>true if the Vehicle's brakelights are enabled, otherwise false./returns>
        public bool IsBrakeLightsEnabled()
        {
            if (brakeLightContainers.Length > 0)
            {
                foreach (Transform t in brakeLightContainers)
                {
                    if (!t.gameObject.activeInHierarchy)
                        return false;
                }
                return true;
            }

            return false; // Vehicle has no brakelights.
        }

        #region Public Engine and Ignition Method(s)
        /// <summary>Activates the starter motor for the vehicle to initiate engine ignition.</summary>
        public void ActivateStarter()
        {
            IsStarterOn = true;

            // Track the starter activation time.
            StarterActivateTime = Time.time;

            // Invoke the 'StarterActiavted' unity event.
            StarterActivated?.Invoke();

            // Compute the start delay.
            float startDelay = UnityEngine.Random.Range(engineStartDelay.minimum, engineStartDelay.maximum);

            // If the start delay is greater than 0 begin a delayed start.
            if (startDelay > 0)
            {
                // Compute start complete time.
                StarterCompleteTime = Time.time + startDelay;
            }
            // Otherwise the engine should start immediately.
            else
            {
                StarterCompleteTime = Time.time;

                // Start the vehicles engine.
                StartEngine();
            }
        }

        /// <summary>Deactivates the starter motor for the vehicle.</summary>
        public void DeactivateStarter()
        {
            IsStarterOn = false;

            // Invoke the 'StarterDeactiavted' unity event.
            StarterDeactivated?.Invoke();
        }

        /// <summary>Immediately starts the vehicles engine.</summary>
        public void StartEngine()
        {
            IsEngineOn = true;

            // Invoke the 'EngineStarted' Unity event.
            EngineStarted?.Invoke();

            // If the engine has been started stop the starter.
            if (IsStarterOn)
                DeactivateStarter();
        }

        /// <summary>Immediately stops the vehicles engine.</summary>
        public void StopEngine()
        {
            IsEngineOn = false;

            // Invoke the 'EngineStopped' Unity event.
            EngineStopped?.Invoke();
        }
        #endregion
    }
}
