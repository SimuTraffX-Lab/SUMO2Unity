using UnityEngine;

namespace VRDriving.Transformation
{
    /// <summary>
    /// A component that makes a gameObject maintain it's localPosition and localRotation from Start().
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class MaintainStartLocalTransform : MonoBehaviour
    {
        private Vector3 m_StartingLocalPosition;
        private Quaternion m_StartingLocalRotation;

        // Unity callback(s).
        void Start()
        {
            m_StartingLocalPosition = transform.localPosition;
            m_StartingLocalRotation = transform.localRotation;
        }

        void Update()
        {
            // Maintain default starting local position/rotation.
            transform.localPosition = m_StartingLocalPosition;
            transform.localRotation = m_StartingLocalRotation;
        }
    }
}
