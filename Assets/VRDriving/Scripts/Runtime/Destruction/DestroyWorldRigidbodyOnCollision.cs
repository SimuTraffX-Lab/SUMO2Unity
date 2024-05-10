using UnityEngine;
using UnityEngine.Events;

namespace VRDriving.Destruction
{
    /// <summary>
    /// A component that converts a GameObject into a destructible world object by exposing public callbacks for destruction that may be activated by a KillableObject component's events for example.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    [RequireComponent(typeof(Rigidbody))]
    public class DestroyWorldRigidbodyOnCollision : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("The sqrMagnitude of relative velocity at or above which the destructible will destruct during a collision.")]
        public float destructVelocityThreshold = 0.5f;
        [Tooltip("The number of seconds before respawning when destructed.")]
        public float respawnTime = 30f;
        [Tooltip("Should the destructible respawn while it or it's spawn position is visible?")]
        public bool respawnWhileVisible = false;
        [Tooltip("(Optional) If not empty the destructible object will be moved into the layer with the given name when it is destructed.")]
        public string destructedLayer = "";
        [Tooltip("Should collisions between the destructor Rigidbody and this destroyable world rigidbody be ignored while destructed?")]
        public bool ignoreCollisionsWithDestructor = true;

        [Header("Events")]
        [Tooltip("An event that is invoked when the 'Respawn()' method was ran on this component.")]
        public UnityEvent Respawned;
        [Tooltip("An event that is invoked when the DesctructibleWorldRigidbody is destructed.")]
        public UnityEvent Destructed;

        /// <summary>Returns true if the DestroyWorldRigidbodyOnCollision has been destructed, otherwise false.</summary>
        public bool IsDestructed { get; private set; }
        /// <summary>Returns a reference to the Rigidbody that destructed this component, or null.</summary>
        public Rigidbody DestructedBy { get; private set; }

        Collider[] m_Colliders;
        bool[] m_IsColliderComplexMesh;

        Rigidbody m_Rigidbody;
        Vector3 m_InitialPosition;
        Quaternion m_InitialRotation;
        int m_DefaultLayerIndex;
        float m_NextRespawnAttempt;
        /// <summary>Tracks whether or not collisions were ignored by this component.</summary>
        bool m_CollisionsIgnored;

        // Unity callback(s).
        void Awake()
        {
            // Find Rigidbody reference.
            m_Rigidbody = GetComponent<Rigidbody>();

            // Find all collider reference(s) and store whether or not they are compelx.
            m_Colliders = GetComponentsInChildren<Collider>();
            m_IsColliderComplexMesh = new bool[m_Colliders.Length];
            for (int i = 0; i < m_Colliders.Length; ++i)
            {
                m_IsColliderComplexMesh[i] = m_Colliders[i] is MeshCollider && !((MeshCollider)m_Colliders[i]).convex;
            }

            // Disable the destructible's rigidbody by making it kinematic.
            m_Rigidbody.isKinematic = true;
            
            // Capture the initial position, rotation, ando ther values of the destructible.
            m_InitialPosition = transform.position;
            m_InitialRotation = transform.rotation;
            m_DefaultLayerIndex = gameObject.layer;
        }

        void Update()
        {
            // Only run with an active camera.
            // Handle respawning.
            if (IsDestructed && respawnTime > 0f && m_NextRespawnAttempt != 0f && Time.time >= m_NextRespawnAttempt)
            {
                // Check if the object should be respawned.
                // It must be both not visible to the camera, nor should its spawn point be visible to the camera. (NOTE: don't check z because camera is top-down, the camera is not front-facing so 'behind the camera may be visible.)
                Vector3 spawnViewportPoint = Camera.current != null ? Camera.current.WorldToViewportPoint(m_InitialPosition) : Vector3.zero;
                if (respawnWhileVisible || (!Utility.IsGameObjectVisible(gameObject) && spawnViewportPoint.x >= 0f && spawnViewportPoint.y <= 1f && spawnViewportPoint.y >= 0f && spawnViewportPoint.y <= 1f))
                {
                    // Respawn the destructible.
                    Respawn();
                    m_NextRespawnAttempt = 0f;
                }
                else
                {
                    // Set next respawn attempt to be respawnTime seconds away.
                    m_NextRespawnAttempt = Time.time + respawnTime;
                }
            }
        }

        void OnCollisionEnter(Collision pCollision)
        {
            // Destruct when collision with required velocity magnitude occurs.
            if (!IsDestructed && pCollision.relativeVelocity.sqrMagnitude >= destructVelocityThreshold)
                Destruct(pCollision);
        }

        // Public method(s).
        /// <summary>
        /// Respawns a DestructibleWorldRigidbody restoring it's initial position, orientation, and health.
        /// </summary>
        public void Respawn()
        {
            // Invoke the respawn event.
            Respawned?.Invoke();

            // Restore this destructible to the proper non-destroyed layer.
            gameObject.layer = m_DefaultLayerIndex;

            // If any collider is a complex mesh collider reset it's state.
            for (int i = 0; i < m_Colliders.Length; ++i)
            {
                if (m_IsColliderComplexMesh[i])
                    ((MeshCollider)m_Colliders[i]).convex = false;
            }

            // Disable the destructible's rigidbody by making it kinematic.
            m_Rigidbody.isKinematic = true;

            // Reset transform to initial state.
            transform.SetPositionAndRotation(m_InitialPosition, m_InitialRotation);

            // Ignore collisions with 'destructed by'.
            if (DestructedBy != null && m_CollisionsIgnored)
            {
                Collider[] destructedByColliders = DestructedBy.GetComponentsInChildren<Collider>();
                Collider[] colliders = m_Rigidbody.GetComponentsInChildren<Collider>();
                foreach (Collider colliderA in destructedByColliders)
                {
                    foreach (Collider colliderB in colliders)
                    {
                        Physics.IgnoreCollision(colliderA, colliderB, false);
                    }
                }

                // Mark collisions as not being ignored by component.
                m_CollisionsIgnored = false;
            }

            // Set the 'is destructed' boolean flag to false.
            IsDestructed = false;
            DestructedBy = null;
            
            // Invoke the 'Respawned' unity event.
            Respawned?.Invoke();
        }

        /// <summary>Destructs the Rigidbody this component is on.</summary>
        public void Destruct() { Destruct(null); }

        /// <summary>Destructs the Rigidbody this component is on.</summary>
        /// <param name="pCollision">(Optional) Collision data information.</param>
        public void Destruct(Collision pCollision)
        {
            // If any collider is a complex mesh collider temporarily make it convex.
            for (int i = 0; i < m_Colliders.Length; ++i)
            {
                if (m_IsColliderComplexMesh[i])
                    ((MeshCollider)m_Colliders[i]).convex = true;
            }

            // Change the destroyed object's layer if setting is enabled.
            if (destructedLayer != null && destructedLayer != string.Empty)
            {
                int layer = LayerMask.NameToLayer(destructedLayer);
                if (layer != -1)
                    gameObject.layer = layer;
            }

            // Activate the destructible's rigidbody.
            m_Rigidbody.isKinematic = false;

            // Apply velocity transfer.
            if (pCollision != null && pCollision.rigidbody != null)
                m_Rigidbody.velocity = pCollision.rigidbody.velocity;

            // Setup next respawn attempt.
            if (respawnTime > 0f)
            {
                m_NextRespawnAttempt = Time.time + respawnTime;
            }
            else { m_NextRespawnAttempt = 0f; }

            // Set the 'is destructed' boolean field to true.
            IsDestructed = true;
            DestructedBy = pCollision.rigidbody;

            // Ignore collisions with 'destructed by'.
            if (ignoreCollisionsWithDestructor)
            {
                Collider[] destructedByColliders = pCollision.rigidbody.GetComponentsInChildren<Collider>();
                Collider[] colliders = m_Rigidbody.GetComponentsInChildren<Collider>();
                foreach (Collider colliderA in destructedByColliders)
                {
                    foreach (Collider colliderB in colliders)
                    {
                        Physics.IgnoreCollision(colliderA, colliderB, true);
                    }
                }

                // Mark collisions as ignored by component.
                m_CollisionsIgnored = true;
            }

            // Invoke the 'Destructed' unity event.
            Destructed?.Invoke();
        }
    }
}
