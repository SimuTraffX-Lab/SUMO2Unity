using UnityEngine;

namespace VRDriving.Collisions
{
    /// <summary>
    /// A simple component that ignores collisions between all colliders in the gameObject the component is attached to and the referenceObject GameObject on Start().
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class IgnoreCollisionsBetweenGameObjects : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("A reference to the GameObject that collisions will be ignored in.")]
        public GameObject referenceObject;

        // Unity callback(s).
        void Start()
        {
            // Ignore collisions between all colliders in gameObject and referenceObject and their children.
            Collider[] collidersInObject = GetComponentsInChildren<Collider>(true);
            Collider[] collidersInReference = referenceObject.GetComponentsInChildren<Collider>(true);
            foreach (Collider colliderA in collidersInObject)
            {
                foreach (Collider colliderB in collidersInReference)
                {
                    Physics.IgnoreCollision(colliderA, colliderB, true);
                }
            }
        }

        void OnDrawGizmos()
        {
            // Ensure 'referenceObject' is not equal to, or a child of this component's gameObject.
            if (referenceObject != null && (referenceObject == gameObject || referenceObject.transform.IsChildOf(transform)))
            {
                referenceObject = null;
                Debug.LogWarning("The 'referenceObject' cannot be the same object, or a child object of this component's transform.", gameObject);
            }
        }
    }
}
