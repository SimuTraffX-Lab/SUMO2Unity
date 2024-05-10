using UnityEngine;

namespace VRDriving.Haptics
{
    /// <summary>A component that detects collisions of a modifiable severity and plays the configured haptics.</summary>
    /// Author: Intuitive Gaming Solutions
    public class PlayHapticsOnCollision : MonoBehaviour
    {
        [Header("Settings - Collision")]
        [Tooltip("The range of collision velocity thresholds. Haptics will be played on collisions with a relative velocity square magnitude above collisionVelocityThreshold.minimum, they will be at maximum strength for collisions at or above collisionVelocityThreshold.maximum.")]
        public FloatMinMax collisionVelocityThreshold;

        [Header("Settings - Haptics")]
        [Tooltip("A reference to the HapticsManager associated with this component.")]
        public HapticsManager hapticsManager;
        [Tooltip("The number of seconds to play haptics for on a collision.")]
        public float hapticsDuration = 0.3f;
        [Tooltip("The haptics amplitude range to play for a collision.")]
        public FloatMinMax hapticsAmplitude;
        [Tooltip("The haptics frequency range to play for a collision.")]
        public FloatMinMax hapticsFrequency;
        [Tooltip("The minimum number of seconds before haptics can be played again after being played.")]
        public float hapticsRepeatDelay = 0.3f;

        /// <summary>Returns the last Time.realTimeSinceStartup this component played haptics.</summary>
        public float LastHapticsPlayRealTime { get; private set; }

        // Unity callback(s).
        void Awake()
        {
            // Find the HapticsManager component.
            if (hapticsManager == null)
                hapticsManager = GetComponent<HapticsManager>();
            if (hapticsManager == null)
                Debug.LogWarning("No 'hapticsManager' found or specified for PlayHapticsOnCollision component!", gameObject);
        }

        void Reset()
        {
            // Look for HapticsManager.
            if (hapticsManager == null)
                hapticsManager = GetComponent<HapticsManager>();

            // Set up collision default settings.
            collisionVelocityThreshold = new FloatMinMax() { minimum = 0f, maximum = 10f };

            // Set up haptics default settings.
            hapticsAmplitude = new FloatMinMax() { minimum = 0.5f, maximum = 0.5f };
            hapticsFrequency = new FloatMinMax() { minimum = 10f, maximum = 10f };
        }

        void OnCollisionEnter(Collision pCollision)
        {
            // Ensure collision was over the minimum threshold.
            if (pCollision.relativeVelocity.sqrMagnitude >= collisionVelocityThreshold.minimum)
            {
                // Calculate normalized collision severity.
                float collisionSeverity = Mathf.Clamp(pCollision.relativeVelocity.sqrMagnitude / collisionVelocityThreshold.maximum, 0, 1);

                // Play haptics.
                PlayHaptics(collisionSeverity);
            }
        }

        // Public method(s).
        /// <summary>
        /// Plays haptics for a collision with a normalized severity (0 to 1) of pNormalizedSeverity.
        /// Higher severity increases haptic amplitude within the specified range.
        /// Higher severity increases haptic frequency within the specified range.
        /// </summary>
        /// <param name="pNormalizedSeverity"></param>
        public void PlayHaptics(float pNormalizedSeverity)
        {
            // Assert that pNormalizedSeverity is in fact normalized.
            Debug.Assert(pNormalizedSeverity >= 0 && pNormalizedSeverity <= 1);

            // Play the haptics if it is not too soon.
            if (Time.realtimeSinceStartup - LastHapticsPlayRealTime >= hapticsRepeatDelay)
            {
                if (hapticsManager != null)
                {
                    hapticsManager.HapticImpulse(
                        hapticsDuration,
                        Mathf.Lerp(hapticsAmplitude.minimum, hapticsAmplitude.maximum, pNormalizedSeverity),
                        Mathf.Lerp(hapticsFrequency.minimum, hapticsFrequency.maximum, pNormalizedSeverity)
                    );
                }
                
                // Update last haptics play time.
                LastHapticsPlayRealTime = Time.realtimeSinceStartup;
            }
        }
    }
}
