using UnityEngine;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof(CarController))]
    public class CarUserControl : MonoBehaviour
    {
        private CarController m_Car; // The car controller we want to use
        public GameObject m_Wheel; // The steering wheel GameObject

        [Header("Steering Settings")]

        [Tooltip("Speed at which the wheel rotates towards the target angle.")]
        public float rotationSpeed = 5f; // Speed at which the wheel rotates towards the target angle

        [Tooltip("Speed at which the wheel returns to center.")]
        public float returnSpeed = 5f; // Speed at which the wheel returns to center

        private float currentAngle = 0f; // Current angle of the wheel

        private void Awake()
        {
            // Get the CarController component
            m_Car = GetComponent<CarController>();
        }

        private void FixedUpdate()
        {
            // Get input
            float h = Input.GetAxis("Horizontal"); // Horizontal input for steering
            float v = Input.GetAxis("Vertical");   // Vertical input for acceleration/braking

            // Determine the target steering angle based on input
            float targetAngle = h * m_Car.m_MaximumSteerAngle;

            if (Mathf.Abs(h) > 0.01f)
            {
                // Smoothly rotate the wheel towards the target angle
                currentAngle = Mathf.LerpAngle(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);
            }
            else
            {
                // Smoothly return the wheel to the center
                currentAngle = Mathf.LerpAngle(currentAngle, 0f, returnSpeed * Time.deltaTime);
            }

            // Apply the rotation to the wheel around the Z-axis
            m_Wheel.transform.localRotation = Quaternion.Euler(0f, 0f, -currentAngle);

            // Get the handbrake input
            float handbrake = Input.GetAxis("Jump"); // Typically mapped to the spacebar

            // Pass the input to the car controller
            m_Car.Move(h, v, v, handbrake);
        }
    }
}
