using UnityEngine;

namespace VRDriving.PhysicsSystem
{
    /// <summary>
    /// A component that can be attached to a Rigidbody that allows a specified Transform to override the Rigidbody's center of mass.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    [RequireComponent(typeof(Rigidbody))]
    public class RigidbodyCenterOfMass : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("A reference to the Transform that represents the location of the Rigidbody's CoM (center of mass).")]
        public Transform comTransform;

        /// <summary>A reference to the Rigidbody associated with this component.</summary>
        public Rigidbody Rigidbody { get; private set; }

        // Start is called before the first frame update
        void Awake()
        {
            // Find Rigidbody reference.
            Rigidbody = GetComponent<Rigidbody>();

            // Override the CoM.
            OverrideCenterOfMass();
        }

        // Public method(s).
        /// <summary>Sets Rigidbody.centerOfMass equal to comTransform.position.</summary>
        public void OverrideCenterOfMass()
        {
            if (comTransform != null)
            {
                Rigidbody.centerOfMass = Rigidbody.transform.InverseTransformPoint(comTransform.position);
            }
        }

        /// <summary>resets the related Rigidbody's center of mass to the automatically calculated value.</summary>
        public void ResetCenterOfMass()
        {
            Rigidbody.ResetCenterOfMass();
        }
    }
}

