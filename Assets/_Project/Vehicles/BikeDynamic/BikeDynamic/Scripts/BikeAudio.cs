using UnityEngine;
using Random = UnityEngine.Random;

namespace UnityStandardAssets.Bike
{
    [RequireComponent(typeof(BikeController))]
    public class BikeAudio : MonoBehaviour
    {
        /* ───────────── NEW FIELD ───────────── */
        [Header("Master Volume (per-bike)")]
        [Range(0f, 1f)] public float masterVolume = 0.02f;   // 0 = silent

        [Header("Engine Clip")]
        [Tooltip("Single audio clip for the engine sound.")]
        public AudioClip engineClip;

        [Header("Pitch & Audio Settings")]
        public float pitchMultiplier = 1f;
        public float lowPitchMin = 1f;
        public float lowPitchMax = 6f;
        public float highPitchMultiplier = 1f;

        [Header("Distance & Effects")]
        public float maxRolloffDistance = 500f;
        public float dopplerLevel = 1f;
        public bool useDoppler = true;

        [Header("Optional Enhancements")]
        public float randomPitchOffset = 0.05f;

        private BikeController m_BikeController;
        private bool m_StartedSound;
        private AudioSource m_EngineSource;

        private const float SpeedThreshold = 0.1f;
        private const float ThrottleThreshold = 0.05f;

        /* ───────────────────────────────────────────── */

        private void Update()
        {
            if (Camera.main == null) return;

            float camDistSqr = (Camera.main.transform.position - transform.position).sqrMagnitude;
            float maxDistSqr = maxRolloffDistance * maxRolloffDistance;

            if (m_StartedSound && camDistSqr > maxDistSqr) StopSound();
            else if (!m_StartedSound && camDistSqr < maxDistSqr) StartSound();

            if (m_StartedSound) UpdateEngineAudio();
        }

        private void StartSound()
        {
            m_BikeController = GetComponent<BikeController>();
            if (m_BikeController == null) return;

            m_EngineSource = CreateEngineAudioSource(engineClip);

            // slight randomisation for natural feel
            pitchMultiplier *= 1f + Random.Range(-randomPitchOffset, randomPitchOffset);

            m_EngineSource.volume = 0f;
            m_EngineSource.Play();
            m_StartedSound = true;
        }

        private void StopSound()
        {
            if (!m_StartedSound) return;
            if (m_EngineSource != null) Destroy(m_EngineSource);
            m_StartedSound = false;
        }

        private void UpdateEngineAudio()
        {
            bool isStopped = Mathf.Abs(m_BikeController.AccelInput) < ThrottleThreshold &&
                             m_BikeController.CurrentSpeed < SpeedThreshold;

            if (isStopped)
            {
                StopSound();
                return;
            }

            // pitch scales with speed
            float speedFactor = Mathf.Clamp01(m_BikeController.CurrentSpeed / m_BikeController.MaxSpeed);
            float pitch = Mathf.Lerp(lowPitchMin, lowPitchMax, speedFactor);
            pitch = Mathf.Min(lowPitchMax, pitch) * pitchMultiplier * highPitchMultiplier;

            m_EngineSource.pitch = pitch;
            m_EngineSource.dopplerLevel = useDoppler ? dopplerLevel : 0f;

            // base volume 0.3–1.0, then scaled by masterVolume
            float baseVol = Mathf.Lerp(0.3f, 1f, speedFactor);
            m_EngineSource.volume = baseVol * masterVolume;   // <<< scaled
        }

        private AudioSource CreateEngineAudioSource(AudioClip clip)
        {
            var source = gameObject.AddComponent<AudioSource>();
            source.clip = clip;
            source.loop = true;
            source.spatialBlend = 1f;       // 3-D sound
            source.minDistance = 5f;
            source.maxDistance = maxRolloffDistance;
            source.dopplerLevel = 0f;
            return source;
        }
    }
}
