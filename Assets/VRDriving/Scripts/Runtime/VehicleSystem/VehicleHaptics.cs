using UnityEngine;

namespace VRDriving.VehicleSystem
{
    /// <summary>
    /// A component that allows crash haptics for a vehicle to be specified.
    /// </summary>
    /// Author: Mathew Aloisio
    public class VehicleHaptics : MonoBehaviour
    {
        [Header("Haptics - Crash")]
        [Tooltip("The time the haptics will play for on a max threshold velocity crash.")]
        public float crashHapticsMaxTime = 0.5f;
        [Tooltip("The maximum amplitude haptics will play for on a max threshold velocity crash.")]
        public float crashHapticsMaxAmplitude = 1f;
        [Tooltip("The minimum velocity in which haptics will play upon crashing and the (maximum) velocity at which haptics will be longest & strongest.")]
        public FloatMinMax crashHapticsVelocityRange = new FloatMinMax() { minimum = 1.5f, maximum = 10f };

        /// <summary>The next Time.time that haptics will be allowed to play.</summary>
        public float NextPossibleHapticsTime { get; protected set; }
		
        // Private callback(s).
        void OnVehicleCollisionEntered(Collision pCollision)
        {
            // Ensure the collision does not involve a child.
            if (!pCollision.transform.IsChildOf(transform))
            {
                // Play haptics on hands holding steering wheel on crash if we're above the relative velocity threshold.
                float relativeVelocityMagnitude = pCollision.relativeVelocity.magnitude;
                if (relativeVelocityMagnitude >= crashHapticsVelocityRange.minimum)
                {
                    // Ensure the haptics cooldown for this component has passed.
                    if (Time.time >= NextPossibleHapticsTime)
                    {
                        // Determine haptics multiplier.
                        float hapticsMultiplier = Mathf.Clamp(relativeVelocityMagnitude / crashHapticsVelocityRange.maximum, 0f, 1f);

                        // Play the crash haptics.
                        float hapticsTime = crashHapticsMaxTime * hapticsMultiplier;
                        /*//TODO Play haptics. //if (m_LeftController != null)
                            m_LeftController.PlayHapticVibration(hapticsTime, crashHapticsMaxAmplitude * hapticsMultiplier);
                        if (m_RightController != null)
                            m_RightController.PlayHapticVibration(hapticsTime, crashHapticsMaxAmplitude * hapticsMultiplier);
                        */
                        // Set next possible haptics time.
                        NextPossibleHapticsTime = Time.time + hapticsTime;
                    }
                }
            }
        }
    }
}
