using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UnityStandardAssets.Scooter
{
    [RequireComponent(typeof(ScooterController))]
    public class ScooterAudio : MonoBehaviour
    {
        /* ───────────── NEW FIELD ───────────── */
        [Header("Master Volume (per-scooter)")]
        [Range(0f, 1f)] public float masterVolume = 0.05f;   // 0 = mute
        /* ───────────────────────────────────── */

        public enum EngineAudioOptions { Simple, FourChannel }

        [Header("Clips")]
        public EngineAudioOptions engineSoundStyle = EngineAudioOptions.FourChannel;
        public AudioClip lowAccelClip;
        public AudioClip lowDecelClip;
        public AudioClip highAccelClip;
        public AudioClip highDecelClip;

        [Header("Pitch")]
        public float pitchMultiplier = 1f;
        public float lowPitchMin = 1f;
        public float lowPitchMax = 6f;
        public float highPitchMultiplier = 0.25f;

        [Header("3-D & FX")]
        public float maxRolloffDistance = 500f;
        public float dopplerLevel = 1f;
        public bool useDoppler = true;

        private AudioSource m_LowAccel, m_LowDecel, m_HighAccel, m_HighDecel;
        private bool m_StartedSound;
        private ScooterController m_ScooterController;

        /* ─────────────────────────────────── */

        private void StartSound()
        {
            m_ScooterController = GetComponent<ScooterController>();

            m_HighAccel = SetUpEngineAudioSource(highAccelClip);

            if (engineSoundStyle == EngineAudioOptions.FourChannel)
            {
                m_LowAccel = SetUpEngineAudioSource(lowAccelClip);
                m_LowDecel = SetUpEngineAudioSource(lowDecelClip);
                m_HighDecel = SetUpEngineAudioSource(highDecelClip);
            }

            m_StartedSound = true;
        }

        private void StopSound()
        {
            foreach (var src in GetComponents<AudioSource>()) Destroy(src);
            m_StartedSound = false;
        }

        private void Update()
        {
            if (Camera.main == null) return;

            float camDistSqr = (Camera.main.transform.position - transform.position).sqrMagnitude;
            float maxDistSqr = maxRolloffDistance * maxRolloffDistance;

            if (m_StartedSound && camDistSqr > maxDistSqr) StopSound();
            if (!m_StartedSound && camDistSqr < maxDistSqr) StartSound();

            if (!m_StartedSound) return;

            /* ─────────── pitch calc ─────────── */
            float pitch = ULerp(lowPitchMin, lowPitchMax, m_ScooterController.Revs);
            pitch = Mathf.Min(lowPitchMax, pitch);      // clamp high revs

            if (engineSoundStyle == EngineAudioOptions.Simple)
            {
                m_HighAccel.pitch = pitch * pitchMultiplier * highPitchMultiplier;
                m_HighAccel.dopplerLevel = useDoppler ? dopplerLevel : 0;
                m_HighAccel.volume = 1f * masterVolume;            // <<< scaled
            }
            else        // four-channel blending
            {
                m_LowAccel.pitch = pitch * pitchMultiplier;
                m_LowDecel.pitch = pitch * pitchMultiplier;
                m_HighAccel.pitch = pitch * highPitchMultiplier * pitchMultiplier;
                m_HighDecel.pitch = pitch * highPitchMultiplier * pitchMultiplier;

                float accFade = Mathf.Abs(m_ScooterController.AccelInput);
                float decFade = 1f - accFade;
                float highFade = Mathf.InverseLerp(0.2f, 0.8f, m_ScooterController.Revs);
                float lowFade = 1f - highFade;

                // smoother fades
                highFade = 1f - (1f - highFade) * (1f - highFade);
                lowFade = 1f - (1f - lowFade) * (1f - lowFade);
                accFade = 1f - (1f - accFade) * (1f - accFade);
                decFade = 1f - (1f - decFade) * (1f - decFade);

                // volumes, each multiplied by masterVolume
                m_LowAccel.volume = lowFade * accFade * masterVolume;
                m_LowDecel.volume = lowFade * decFade * masterVolume;
                m_HighAccel.volume = highFade * accFade * masterVolume;
                m_HighDecel.volume = highFade * decFade * masterVolume;

                float dop = useDoppler ? dopplerLevel : 0f;
                m_HighAccel.dopplerLevel = dop;
                m_LowAccel.dopplerLevel = dop;
                m_HighDecel.dopplerLevel = dop;
                m_LowDecel.dopplerLevel = dop;
            }
        }

        /* ───────── helper fns (unchanged) ───────── */

        private AudioSource SetUpEngineAudioSource(AudioClip clip)
        {
            AudioSource src = gameObject.AddComponent<AudioSource>();
            src.clip = clip;
            src.volume = 0f;
            src.loop = true;
            src.time = Random.Range(0f, clip.length);
            src.Play();
            src.minDistance = 5f;
            src.maxDistance = maxRolloffDistance;
            src.dopplerLevel = 0f;
            return src;
        }

        private static float ULerp(float from, float to, float value) =>
            (1f - value) * from + value * to;
    }
}
