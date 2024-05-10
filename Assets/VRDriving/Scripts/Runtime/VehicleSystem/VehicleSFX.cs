using System;
using System.Collections.Generic;
using UnityEngine;

namespace VRDriving.VehicleSystem
{
    /// <summary>
    /// A component that adds sound effects to vehicles.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class VehicleSFX : MonoBehaviour
    {
        // EnginePitchModulation.
        /// <summary>Holds information about automatic engine pitch modulation for an engine sound.</summary>
        [Serializable]
        public struct EnginePitchModulation
        {
            [Tooltip("The speed at which maxPitchIncrease will be added to engine sound pitch, the value will linearly scale up to this speed. A value of 0 means no pitch scaling.")]
            public float scalePitchUpToSpeed;
            [Tooltip("The maximum pitch increase when scalePitchUpToSpeed forward speed is reached. Note that the total pitch will never go over 3. A value of 0 will result in no pitch scaling.")]
            [Range(0f, 3f)]
            public float maxPitchIncrease;
        }

        // EngineSoundEntry
        [Serializable]
        public class EngineSoundEntry
        {
            [Tooltip("The minimum forward speed the vehicle needs to activate this engine sound. (Make this -Infinity to ignore it.)")]
            public float minimumForwardSpeed = float.NegativeInfinity;
            [Tooltip("The minimum backward speed the vehicle needs to activate this engine sound. (Make this Infinity to ignore it.)")]
            public float minimumBackwardSpeed = float.PositiveInfinity;
            [Tooltip("The minimum torque factor the vehicle needs to be accelerating with to activate this engine sound. (NOTE: Mathf.Abs(currentTorque)/maxWheelTorque = torqueFactor)")]
            public float minimumTorqueFactor;
            [Range(0f, 1f)]
            [Tooltip("The volume for the related audio clip.")]
            public float volume;
            [Range(-3f, 3f)]
            [Tooltip("The pitch to play the related audio clip in.")]
            public float pitch;
            [Tooltip("Holds information about automatic pitch modulation.")]
            public EnginePitchModulation pitchModulation;
            [Tooltip("An audio clip containing a sound.")]
            public AudioClip clip;
        }

        // VehicleSFX.
        [Header("SFX Settings - Engine")]
        [Range(0f, 1f)]
        [Tooltip("The volume multiplier for the engine idle sound. (0f to 1f)")]
        public float engineIdleSoundVolume = 1f;
        [Range(-3f, 3f)]
        [Tooltip("The pitch to play the engine idle sound at. (-3f to 3f)")]
        public float engineIdleSoundPitch = 1f;
        [Tooltip("The audio clip to play on the engine audio source while the vehicle's engine is idling.")]
        public AudioClip engineIdleSound;
        [Tooltip("An array of engine sounds and conditions to trigger them. (NOTE: Conditions are checked from last-to-first so put the highest-speed conditions at the end of the array.)")]
        public EngineSoundEntry[] engineSounds;

        [Header("SFX Settings - Brakes")]
        [Min(0)]
        [Tooltip("The velocity magnitude at which the brake volume will be played at maximum.")]
        public float brakeVolumeMaximumVelocityMagnitude = 7f;
        [Min(0)]
        [Tooltip("The velocity magnitude at which the braking sound will stop playing.")]
        public float brakeVolumeMinimumVelocityMagnitude = 0.5f;
        [Range(0f, 1f)]
        [Tooltip("The volume multiplier for braking sounds. (0f->1f) [NOTE: The true volume is calculated in-game based on the velocity of the vehicle while braking.]")]
        public float brakingSoundMaximumVolume = 1f;
        [Tooltip("The audio clip to play on the wheel audio source while the vehicle is braking.")]
        public AudioClip brakingSound;

        [Header("SFX Settings - Drift")]
        [Tooltip("The number of seconds a vehicle has to be drifting for before the drift sound is started.")]
        public float driftSoundStartDelay = 0.09f;
        [Min(0)]
        [Tooltip("The difference between forward speed and velocity magnitude where the drift volume is maximized.")]
        public float driftVolumeMaximumForwardVelocityDeviance = 1.5f;
        [Range(0f, 1f)]
        [Tooltip("The volume multiplier for drifting sounds. (0f->1f) [NOTE: The true volume is calculated in-game based on the velocity of the vehicle while drifitng.")]
        public float driftSoundMaximumVolume = 1f;
        [Tooltip("The drift sound to play on the wheel audio source while the vehicle is drifting.")]
        public AudioClip driftSound;

        [Header("References")]
        [Tooltip("A reference to the vehicle to play sfx for. (If null the component will attempt to find a Vehicle component on this component's gameObject.)")]
        public Vehicle vehicle;
        [Tooltip("A reference to the torque-applying wheel(s) associated with this component. Only one is neccesary unless you have a system like traciton control enabled that may alter power to the wheels.")]
        public WheelCollider[] torqueRefWheels;
        [Tooltip("A reference to the audio source for the vehicle's engine sounds. (If null the component will attempt to find an AudioSource on this component's gameObject.)")]
        public AudioSource engineAudioSource;
        [Tooltip("A reference to the audio source for skid and braking sounds of the vehicle. (If null the component will attempt to find an AudioSource on this component's gameOBject.)")]
        public AudioSource wheelAudioSource;

        /// <summary>Returns true if the component is playing the vehicle engine sound, otherwise false.</summary>
        public bool IsPlayingEngineSound { get; private set; }
        /// <summary>Returns true if the component is playing the vehicle brake sound, otherwise false.</summary>
        public bool IsPlayingBrakeSound { get; private set; }
        /// <summary>Returns true if the component is playing the vehicle drift sound, otherwise false.</summary>
        public bool IsPlayingDriftSound { get; private set; }

        // Unity callback(s).
        void Start()
        {
            // Look for references.
            if (vehicle == null)
            {
                vehicle = GetComponent<Vehicle>();
                if (vehicle == null)
                    Debug.LogWarning("No 'vehicle' found for VehicleSFX component on gameObject '" + gameObject.name + "'!");
            }
            if (engineAudioSource == null)
            {
                engineAudioSource = GetComponent<AudioSource>();
                if (engineAudioSource == null)
                    Debug.LogWarning("No 'engine audio source' found for VehicleSFX component on gameObject '" + gameObject.name + "'!");
            }
            if (wheelAudioSource == null)
            {
                wheelAudioSource = GetComponent<AudioSource>();
                if (wheelAudioSource == null)
                    Debug.LogWarning("No 'wheel audio source' found for VehicleSFX component on gameObject '" + gameObject.name + "'!");
            }

            // Ensure audio sources aren't the same.
            if (engineAudioSource != null && wheelAudioSource == engineAudioSource)
                Debug.LogWarning("The same audio source is set for 'engine audio source' and 'wheel audio source' in the VehicleSFX component on gameObject '" + gameObject.name + "'! This will cause the wheel and engine related SFX to overwrite eachother.");
        }

        void Reset()
        {
            // Find 'vehicle' reference.
            if (vehicle == null)
                vehicle = GetComponent<Vehicle>();

            // If there is a valid 'vehicle' reference look for default 'torqueRefWheels' reference.
            if (vehicle != null)
            {
                if (torqueRefWheels == null || torqueRefWheels.Length == 0)
                {
                    List<WheelCollider> torqueApplyingWheels = new List<WheelCollider>();
                    foreach (Vehicle.WheelInfo wheelInfo in vehicle.wheels)
                    {
                        if (wheelInfo != null && wheelInfo.component != null && wheelInfo.component.motor)
                            torqueApplyingWheels.Add(wheelInfo.component.wheelCollider);
                    }
                    torqueRefWheels = torqueApplyingWheels.ToArray();
                }
            }
        }

        void Update()
        {
            // Handle engine sound.
            UpdateEngineSFX();

            // Handle wheels sounds.
            UpdateWheelsSFX();
        }

        // Public method(s).
        /// <summary>Returns the highest torque force reported by any of the 'torqueRefWheels', or 0.</summary>
        /// <returns>The highest torque force reported by any of the 'torqueRefWheels', or 0.</returns>
        public float GetTorqueForceReference()
        {
            if (torqueRefWheels != null)
            {
                float highestTorque = 0;
                foreach (WheelCollider wheelCollider in torqueRefWheels)
                {
                    if (wheelCollider != null)
                    {
                        float torqueForce = Mathf.Abs(wheelCollider.motorTorque) / vehicle.maxMotorTorque;
                        if (torqueForce > highestTorque)
                            highestTorque = torqueForce;
                    }
                }

                // Return the highest torque force.
                return highestTorque;
            }

            return 0;
        }

        // Protected method(s).
        /// <summary>Update vehicle engine sounds.</summary>
        protected void UpdateEngineSFX()
        {
            // Only play if an engine audio source was found and the engine is on.
            if (engineAudioSource != null)
            {
                if (vehicle.IsEngineOn)
                {
                    // Playing engine sound.
                    IsPlayingEngineSound = true;

                    // Loop over every engine sound from last-to-first and see if the conditions are matched for a sound.
                    float torqueFactor = GetTorqueForceReference();
					for (int i = engineSounds.Length - 1; i >= 0; --i)
                    {
                        if (vehicle.LocalVelocity.z >= engineSounds[i].minimumForwardSpeed && vehicle.LocalVelocity.z <= engineSounds[i].minimumBackwardSpeed && torqueFactor >= engineSounds[i].minimumTorqueFactor) // Don't check absolute forward velocity since reverse should only play the first engine sound.
                        {
                            // Sound conditions met for this engine sound entry.
                            // Determine engine pitch.
                            engineAudioSource.pitch = engineSounds[i].pitch;
                            float absForwardVelocity = Mathf.Abs(vehicle.LocalVelocity.z);

                            // Modulate pitch if all conditions are met. (scalePitchUpSpeed > 0, maxPitchIncrease > 0, and absoluteForwardVelocity >= minimumForwardSpeed)
                            if (engineSounds[i].pitchModulation.scalePitchUpToSpeed > 0 && engineSounds[i].pitchModulation.maxPitchIncrease > 0 && absForwardVelocity >= engineSounds[i].minimumForwardSpeed)
                            {
                                float pitchIncreaseFactor = Mathf.Abs(vehicle.LocalVelocity.z) / engineSounds[i].pitchModulation.scalePitchUpToSpeed;
                                engineAudioSource.pitch = Mathf.Clamp(engineAudioSource.pitch + (engineSounds[i].pitchModulation.maxPitchIncrease * pitchIncreaseFactor), -3f, 3f);
                            }

                            // Set the engine sound volume if it doesn't match.
                            if (engineAudioSource.volume != engineSounds[i].volume)
                                engineAudioSource.volume = engineSounds[i].volume;

                            // Set the clip and play the engine sound if it is not already playing.
                            if (engineAudioSource.clip != engineSounds[i].clip)
                            {
                                // Setup engine volume and clip before playing.
                                engineAudioSource.clip = engineSounds[i].clip;
                                engineAudioSource.Play();
                            }

                            return;
                        }
                    }

                    // Only if there is a valid engine idle sound...
                    if (engineIdleSound != null)
                    {
                        // Update pitch and volume if they mismatch.
                        if (engineAudioSource.pitch != engineIdleSoundPitch)
                            engineAudioSource.pitch = engineIdleSoundPitch;
                        if (engineAudioSource.volume != engineIdleSoundVolume)
                            engineAudioSource.volume = engineIdleSoundVolume;

                        // Play the engine idle sound if it is not already playing.
                        if (engineAudioSource.clip != engineIdleSound)
                        {
                            engineAudioSource.clip = engineIdleSound;
                            engineAudioSource.Play();
                        }
                    }
                }
                // Otherwise stop playing the engine sound if its being played by this component.
                else if (IsPlayingEngineSound)
                {
                    engineAudioSource.Stop();

                    // Track that the engine sound is not being played.
                    IsPlayingEngineSound = false;
                }
            }
        }

        /// <summary>
        /// Update vehicle wheel sounds.
        /// </summary>
        protected void UpdateWheelsSFX()
        {
            // Only play if a wheel audio source was found.
            if (wheelAudioSource != null)
            {
                // Check if the vehicle is braking.
                if (vehicle.IsBraking && brakingSound != null)
                {
                    float vehicleVelocityMagnitude = vehicle.Velocity.magnitude;
                    if (vehicleVelocityMagnitude >= brakeVolumeMinimumVelocityMagnitude)
                    {
                        // Determine brake sound volume.
                        wheelAudioSource.volume = brakingSoundMaximumVolume * Mathf.Clamp(vehicleVelocityMagnitude / brakeVolumeMaximumVelocityMagnitude, 0f, 1f);

                        // Play brake sound if it is not already being played.
                        if (!IsPlayingBrakeSound || !wheelAudioSource.isPlaying)
                        {
                            IsPlayingBrakeSound = true;
                            wheelAudioSource.clip = brakingSound;
                            wheelAudioSource.Play();
                        }
                    }
                    else if (IsPlayingBrakeSound) { StopBrakeSound(); }
                }
                else
                {
                    // Stop braking sound if neccesary.
                    if (IsPlayingBrakeSound)
                        StopBrakeSound();

                    // Drifting sounds.
                    if (driftSound != null)
                    {
                        // Check if we're in a drift or not, if not stop the sound.
                        if (vehicle.IsDrifting)
                        {
                            // Ensure we've been drifting for the appropriate number of seconds.
                            if (Time.time - vehicle.DriftStartTime >= driftSoundStartDelay)
                            {
                                // Determine drift sound volume.
                                wheelAudioSource.volume = driftSoundMaximumVolume * Mathf.Clamp(vehicle.AverageForwardVelocityDeviance / driftVolumeMaximumForwardVelocityDeviance, 0f, 1f);

                                // Play drift sound if it is not already being played.
                                if (!IsPlayingDriftSound || !wheelAudioSource.isPlaying)
                                {
                                    IsPlayingDriftSound = true;
                                    wheelAudioSource.clip = driftSound;
                                    wheelAudioSource.Play();
                                }
                            }
                        }
                        else if (IsPlayingDriftSound) { StopDriftSound(); } // Stop playing the drift sound since the vehicle has stopped drifting.
                    }
                }
            }
        }

        // Private method(s).
        void StopBrakeSound()
        {
            IsPlayingBrakeSound = false;
            wheelAudioSource.Stop();
        }

        void StopDriftSound()
        {
            IsPlayingDriftSound = false;
            wheelAudioSource.Stop();
        }
    }
}
