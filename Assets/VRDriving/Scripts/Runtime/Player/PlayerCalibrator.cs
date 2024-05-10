using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace VRDriving.Player
{
    /// <summary>
    /// A component that is attached to a player to reposition the player based on their headset position relative to a target head 'neutral' transform.
    /// </summary>
	/// Author: Intuitive Gaming Solutions
    public class PlayerCalibrator : MonoBehaviour
    {
        // CalibratedUnityEvent.
        [Serializable]
        public class CalibratedUnityEvent : UnityEvent<Vector3> { };

        // PlayerCalibrator.
        [Header("Settings")]
        [Tooltip("A reference to the transform to align the player's head with when calibrating.")]
        public Transform headTargetTransform;
        [Tooltip("A reference to the transform for the player's headset.")]
        public Transform headsetTransform;
        [Tooltip("An optional array of extra Transforms to apply the calibration offset to.")]
        public Transform[] offsetTransforms;

        [Header("Inputs")]
        [Tooltip("The input that will recalibrate the player's view.")]
        [SerializeField] protected InputActionProperty m_CalibrateInput;

        [Header("Events")]
        [Tooltip("An event that is invoked when a PlayerCalibrator calibration is done. [Type: void(float pDeltaPosition] [pDeltaPosition is in world space.]")]
        public CalibratedUnityEvent Calibrated;

        // Unity callback(s).
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

        // Public method(s).
        /// <summary>
        /// (Re)calibrates the player's view.
        /// </summary>
        public void Calibrate()
        {
            if (headTargetTransform != null)
            {
                if (headsetTransform != null)
                {
                    // Move the entire player so that the head is in the same position as the head target transform.
                    Vector3 offset = headTargetTransform.position - headsetTransform.position;
                    transform.position += offset;

                    // Apply offset to additionals.
                    foreach (Transform additional in offsetTransforms)
                    {
                        additional.position += offset;
                    }

                    // Invoke the 'Calibrated' Unity event.
                    Calibrated?.Invoke(offset);
                }
                else { Debug.LogWarning("No 'headsetTransform' set in PlayerViewCalibrator component on gameObject '" + gameObject.name + "'. View was not calibrated!", gameObject); }
            }
            else { Debug.LogWarning("No 'headTargetTransform' set in PlayerViewCalibrator component on gameObject '" + gameObject.name + "'. View was not calibrated!", gameObject); }
        }

        // Protected method(s).
        protected void BindActions()
        {
            // Enable inputs and subscribe to event(s).
            if (m_CalibrateInput != null)
            {
                m_CalibrateInput.action.Enable();
                m_CalibrateInput.action.started += OnCalibratePressed;
            }
        }

        protected void UnbindActions()
        {
            // Disable inputs and unsubscribe from event(s).
            if (m_CalibrateInput != null)
            {
                m_CalibrateInput.action.Disable();
                m_CalibrateInput.action.started -= OnCalibratePressed;
            }
        }

        // Protected callback(s).
        protected void OnCalibratePressed(InputAction.CallbackContext pContext)
        {
            // Recalibrate.
            Calibrate();
        }
    }
}
