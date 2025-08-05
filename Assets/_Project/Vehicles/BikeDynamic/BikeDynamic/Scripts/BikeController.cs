using System;
using UnityEngine;

#pragma warning disable 649
namespace UnityStandardAssets.Bike
{
    internal enum BikeDriveType
    {
        FrontWheelDrive,
        RearWheelDrive,
        FourWheelDrive
    }

    internal enum BikeSpeedType
    {
        MPH,
        KPH
    }

    public class BikeController : MonoBehaviour
    {
        [SerializeField] private BikeDriveType m_BikeDriveType = BikeDriveType.FourWheelDrive;
        [SerializeField] public WheelCollider[] m_WheelColliders = new WheelCollider[4];
        [SerializeField] private GameObject[] m_WheelMeshes = new GameObject[4];

        [SerializeField] private Vector3 m_CentreOfMassOffset;
        [SerializeField] public float m_MaximumSteerAngle;
        [Range(0, 1)][SerializeField] private float m_SteerHelper; // 0 is raw physics , 1 the bike will grip in the direction it is facing
        [Range(0, 1)][SerializeField] private float m_TractionControl; // 0 is no traction control, 1 is full interference
        [SerializeField] private float m_FullTorqueOverAllWheels;
        [SerializeField] private float m_ReverseTorque;
        [SerializeField] private float m_MaxHandbrakeTorque;
        [SerializeField] private float m_Downforce = 100f;
        [SerializeField] private BikeSpeedType m_SpeedType;
        [SerializeField] private float m_Topspeed = 200;
        [SerializeField] private float m_SlipLimit;
        [SerializeField] public float m_BrakeTorque;

        private Quaternion[] m_WheelMeshLocalRotations;
        private Vector3 m_Prevpos, m_Pos;
        private float m_SteerAngle;
        private float m_OldRotation;
        private float m_CurrentTorque;
        private Rigidbody m_Rigidbody;
        private const float k_ReversingThreshold = 0.01f;

        public bool Skidding { get; private set; }
        public float BrakeInput { get; private set; }
        public float CurrentSteerAngle { get { return m_SteerAngle; } }
        public float CurrentSpeed { get { return m_Rigidbody.linearVelocity.magnitude * 2.23693629f; } }
        public float MaxSpeed { get { return m_Topspeed; } }
        public float AccelInput { get; private set; }

        private void Start()
        {
            m_WheelMeshLocalRotations = new Quaternion[4];
            for (int i = 0; i < 4; i++)
            {
                m_WheelMeshLocalRotations[i] = m_WheelMeshes[i].transform.localRotation;
            }
            m_WheelColliders[0].attachedRigidbody.centerOfMass = m_CentreOfMassOffset;

            m_MaxHandbrakeTorque = float.MaxValue;
            m_Rigidbody = GetComponent<Rigidbody>();
            m_CurrentTorque = m_FullTorqueOverAllWheels - (m_TractionControl * m_FullTorqueOverAllWheels);
        }

        // ────────────────────────────────────────────────────────────────
        //  BikeController.Move - updated to offset wheel meshes in X
        // ────────────────────────────────────────────────────────────────
        public void Move(float steering, float accel, float footbrake, float handbrake)
        {
            const float meshOffsetX = +0.17f;               // ← change sign/size as needed

            // 1) Update visual wheel meshes from the WheelColliders
            for (int i = 0; i < 4; i++)
            {
                Quaternion quat;
                Vector3 position;
                m_WheelColliders[i].GetWorldPose(out position, out quat);

                // push the mesh sideways in the bike’s local X direction
                position += transform.right * meshOffsetX;

                m_WheelMeshes[i].transform.position = position;
                m_WheelMeshes[i].transform.rotation = quat;
            }

            // 2) Clamp inputs
            steering = Mathf.Clamp(steering, -1f, 1f);
            AccelInput = accel = Mathf.Clamp(accel, 0f, 1f);
            BrakeInput = footbrake = -1f * Mathf.Clamp(footbrake, -1f, 0f);
            handbrake = Mathf.Clamp(handbrake, 0f, 1f);

            // 3) Steering (front wheels)
            m_SteerAngle = steering * m_MaximumSteerAngle;
            m_WheelColliders[0].steerAngle = m_SteerAngle;
            m_WheelColliders[1].steerAngle = m_SteerAngle;

            // 4) Core vehicle helpers
            SteerHelper();
            ApplyDrive(accel, footbrake);
            CapSpeed();

            // 5) Handbrake logic (rear wheels)
            if (handbrake > 0f)
            {
                float hbTorque = handbrake * m_MaxHandbrakeTorque;
                m_WheelColliders[2].brakeTorque = hbTorque;
                m_WheelColliders[3].brakeTorque = hbTorque;
            }
            else if (accel > 0f)
            {
                // release residual brake torque so the bike can move
                m_WheelColliders[2].brakeTorque = 0f;
                m_WheelColliders[3].brakeTorque = 0f;
            }

            // 6) Misc vehicle effects
            AddDownForce();
            TractionControl();
        }



        private void CapSpeed()
        {
            float speed = m_Rigidbody.linearVelocity.magnitude;
            switch (m_SpeedType)
            {
                case BikeSpeedType.MPH:
                    speed *= 2.23693629f;
                    if (speed > m_Topspeed)
                        m_Rigidbody.linearVelocity = (m_Topspeed / 2.23693629f) * m_Rigidbody.linearVelocity.normalized;
                    break;
                case BikeSpeedType.KPH:
                    speed *= 3.6f;
                    if (speed > m_Topspeed)
                        m_Rigidbody.linearVelocity = (m_Topspeed / 3.6f) * m_Rigidbody.linearVelocity.normalized;
                    break;
            }
        }

        private void ApplyDrive(float accel, float footbrake)
        {
            float thrustTorque;
            switch (m_BikeDriveType)
            {
                case BikeDriveType.FourWheelDrive:
                    thrustTorque = accel * (m_CurrentTorque / 4f);
                    for (int i = 0; i < 4; i++)
                    {
                        m_WheelColliders[i].motorTorque = thrustTorque;
                    }
                    break;
                case BikeDriveType.FrontWheelDrive:
                    thrustTorque = accel * (m_CurrentTorque / 2f);
                    m_WheelColliders[0].motorTorque = m_WheelColliders[1].motorTorque = thrustTorque;
                    break;
                case BikeDriveType.RearWheelDrive:
                    thrustTorque = accel * (m_CurrentTorque / 2f);
                    m_WheelColliders[2].motorTorque = m_WheelColliders[3].motorTorque = thrustTorque;
                    break;
            }

            for (int i = 0; i < 4; i++)
            {
                if (CurrentSpeed > 5 && Vector3.Angle(transform.forward, m_Rigidbody.linearVelocity) < 50f)
                {
                    m_WheelColliders[i].brakeTorque = m_BrakeTorque * footbrake;
                }
                else if (footbrake > 0)
                {
                    m_WheelColliders[i].brakeTorque = 0f;
                    m_WheelColliders[i].motorTorque = -m_ReverseTorque * footbrake;
                }
            }
        }

        private void SteerHelper()
        {
            for (int i = 0; i < 4; i++)
            {
                WheelHit wheelhit;
                m_WheelColliders[i].GetGroundHit(out wheelhit);
                if (wheelhit.normal == Vector3.zero)
                    return;
            }

            if (Mathf.Abs(m_OldRotation - transform.eulerAngles.y) < 10f)
            {
                var turnadjust = (transform.eulerAngles.y - m_OldRotation) * m_SteerHelper;
                Quaternion velRotation = Quaternion.AngleAxis(turnadjust, Vector3.up);
                m_Rigidbody.linearVelocity = velRotation * m_Rigidbody.linearVelocity;
            }
            m_OldRotation = transform.eulerAngles.y;
        }

        private void AddDownForce()
        {
            m_WheelColliders[0].attachedRigidbody.AddForce(-transform.up * m_Downforce *
                                                           m_WheelColliders[0].attachedRigidbody.linearVelocity.magnitude);
        }

        private void TractionControl()
        {
            WheelHit wheelHit;
            switch (m_BikeDriveType)
            {
                case BikeDriveType.FourWheelDrive:
                    for (int i = 0; i < 4; i++)
                    {
                        m_WheelColliders[i].GetGroundHit(out wheelHit);
                        AdjustTorque(wheelHit.forwardSlip);
                    }
                    break;
                case BikeDriveType.RearWheelDrive:
                    m_WheelColliders[2].GetGroundHit(out wheelHit);
                    AdjustTorque(wheelHit.forwardSlip);

                    m_WheelColliders[3].GetGroundHit(out wheelHit);
                    AdjustTorque(wheelHit.forwardSlip);
                    break;
                case BikeDriveType.FrontWheelDrive:
                    m_WheelColliders[0].GetGroundHit(out wheelHit);
                    AdjustTorque(wheelHit.forwardSlip);

                    m_WheelColliders[1].GetGroundHit(out wheelHit);
                    AdjustTorque(wheelHit.forwardSlip);
                    break;
            }
        }

        private void AdjustTorque(float forwardSlip)
        {
            if (forwardSlip >= m_SlipLimit && m_CurrentTorque >= 0)
            {
                m_CurrentTorque -= 10 * m_TractionControl;
            }
            else
            {
                m_CurrentTorque += 10 * m_TractionControl;
                if (m_CurrentTorque > m_FullTorqueOverAllWheels)
                {
                    m_CurrentTorque = m_FullTorqueOverAllWheels;
                }
            }
        }
    }
}
