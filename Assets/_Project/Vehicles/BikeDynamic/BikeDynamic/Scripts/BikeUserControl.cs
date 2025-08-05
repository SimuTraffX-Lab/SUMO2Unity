using System;
using UnityEngine;

namespace UnityStandardAssets.Bike
{
    [RequireComponent(typeof(BikeController))]
    public class BikeUserControl : MonoBehaviour
    {
        private BikeController m_Bike; // the car controller we want to use
        public GameObject m_Wheel;

        private float currentAngle = 0f; // Starting angle

        private void Awake()
        {
            // get the car controller
            m_Bike = GetComponent<BikeController>();
        }


        private void FixedUpdate()
        {
            // pass the input to the car!
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            // Determine the target steering angle based on input
            float targetAngle = h * m_Bike.m_MaximumSteerAngle;


            // Apply the rotation to the wheel around the Z-axis
            m_Wheel.transform.localRotation = Quaternion.Euler(0f, targetAngle, 0f);

            // Get the handbrake input
            float handbrake = Input.GetAxis("Jump"); // Typically mapped to the spacebar

            // Pass the input to the car controller
            m_Bike.Move(h, v, v, handbrake);
        }
    }
}
