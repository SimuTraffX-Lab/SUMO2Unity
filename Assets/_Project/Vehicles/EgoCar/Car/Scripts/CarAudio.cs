using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof(CarController))]
    public class CarAudio : MonoBehaviour
    {
        // ────────────────────────  NEW  ────────────────────────
        [Header("Master Volume (0 = mute, 1 = full)")]
        [Range(0f, 1f)] public float masterVolume = 0.05f;   // <-- add this
        // ───────────────────────────────────────────────────────

        public enum EngineAudioOptions { Simple, FourChannel }
        public EngineAudioOptions engineSoundStyle = EngineAudioOptions.FourChannel;

        public AudioClip lowAccelClip;
        public AudioClip lowDecelClip;
        public AudioClip highAccelClip;
        public AudioClip highDecelClip;

        public float pitchMultiplier = 1f;
        public float lowPitchMin = 1f;
        public float lowPitchMax = 6f;
        public float highPitchMultiplier = 0.25f;
        public float maxRolloffDistance = 500;
        public float dopplerLevel = 1;
        public bool useDoppler = true;

        private AudioSource m_LowAccel, m_LowDecel, m_HighAccel, m_HighDecel;
        private bool m_StartedSound;
        private CarController m_CarController;

        /* ───────────────────────── helper funcs (unchanged) ───────────────────────── */

        private void StartSound()
        {
            m_CarController = GetComponent<CarController>();
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

        /* ─────────────────────────── main update ─────────────────────────── */

        private void Update()
        {
            float camDistSqr = (Camera.main.transform.position - transform.position).sqrMagnitude;
            float maxDistSqr = maxRolloffDistance * maxRolloffDistance;

            if (m_StartedSound && camDistSqr > maxDistSqr) StopSound();
            if (!m_StartedSound && camDistSqr < maxDistSqr) StartSound();

            if (!m_StartedSound) return;

            float pitch = Mathf.Min(lowPitchMax, ULerp(lowPitchMin, lowPitchMax, m_CarController.Revs));

            if (engineSoundStyle == EngineAudioOptions.Simple)
            {
                m_HighAccel.pitch = pitch * pitchMultiplier * highPitchMultiplier;
                m_HighAccel.dopplerLevel = useDoppler ? dopplerLevel : 0;
                m_HighAccel.volume = 1f * masterVolume;                 // <-- scaled
            }
            else
            {
                // four-channel calculations (unchanged) …
                float accFade = Mathf.Abs(m_CarController.AccelInput);
                float decFade = 1 - accFade;
                float highFade = Mathf.InverseLerp(0.2f, 0.8f, m_CarController.Revs);
                float lowFade = 1 - highFade;

                highFade = 1 - (1 - highFade) * (1 - highFade);
                lowFade = 1 - (1 - lowFade) * (1 - lowFade);
                accFade = 1 - (1 - accFade) * (1 - accFade);
                decFade = 1 - (1 - decFade) * (1 - decFade);

                m_LowAccel.pitch = pitch * pitchMultiplier;
                m_LowDecel.pitch = pitch * pitchMultiplier;
                m_HighAccel.pitch = pitch * highPitchMultiplier * pitchMultiplier;
                m_HighDecel.pitch = pitch * highPitchMultiplier * pitchMultiplier;

                // Volumes multiplied by masterVolume  ─────────────
                m_LowAccel.volume = lowFade * accFade * masterVolume;
                m_LowDecel.volume = lowFade * decFade * masterVolume;
                m_HighAccel.volume = highFade * accFade * masterVolume;
                m_HighDecel.volume = highFade * decFade * masterVolume;

                float dop = useDoppler ? dopplerLevel : 0;
                m_LowAccel.dopplerLevel = dop;
                m_LowDecel.dopplerLevel = dop;
                m_HighAccel.dopplerLevel = dop;
                m_HighDecel.dopplerLevel = dop;
            }
        }

        /* ───────────────────── setup helper (unchanged) ───────────────────── */

        private AudioSource SetUpEngineAudioSource(AudioClip clip)
        {
            var src = gameObject.AddComponent<AudioSource>();
            src.clip = clip;
            src.volume = 0;             // will be set in Update()
            src.loop = true;
            src.time = Random.Range(0f, clip.length);
            src.Play();
            src.minDistance = 5;
            src.maxDistance = maxRolloffDistance;
            src.dopplerLevel = 0;
            return src;
        }

        private static float ULerp(float from, float to, float value) =>
            (1f - value) * from + value * to;
    }
}
