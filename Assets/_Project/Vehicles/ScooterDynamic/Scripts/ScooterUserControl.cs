using System;
using UnityEngine;

namespace UnityStandardAssets.Scooter
{
    [RequireComponent(typeof (ScooterController))]
    public class ScooterUserControl : MonoBehaviour
    {
        private ScooterController m_Scooter; // the car controller we want to use
        public GameObject m_Wheel;

        public float rotationSpeed = 5f; // Adjust for how quickly the wheel rotates to new angle
        private float currentAngle = 90f; // Starting angle
        public float maxSteeringAngle = 180f;
        private void Awake()
        {
            // get the car controller
            m_Scooter = GetComponent<ScooterController>();
        }


        private void FixedUpdate()
        {
            // pass the input to the car!
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            // Calculate target angle. For example, "90f + h * 180f" means:
            // - When h = 0, angle = 90°
            // - When h = 1, angle = 270°
            // - When h = -1, angle = -90° (which modulo 360 is 270°, but will interpolate smoothly)
            float targetAngle = 90f - h * maxSteeringAngle;

            // Smoothly interpolate the current angle towards the target angle
            currentAngle = Mathf.LerpAngle(currentAngle, targetAngle, Time.deltaTime * rotationSpeed);

            // Apply new rotation to the wheel
            // Rotation is around the local X-axis. If you need a different axis, adjust Euler accordingly.
            m_Wheel.transform.localRotation = Quaternion.Euler(currentAngle, -90f, 90f);
#if !MOBILE_INPUT
            float handbrake = Input.GetAxis("Jump");
            m_Scooter.Move(h, v, v, handbrake);
#else
            m_Car.Move(h, v, v, 0f);
#endif
        }
    }
}
