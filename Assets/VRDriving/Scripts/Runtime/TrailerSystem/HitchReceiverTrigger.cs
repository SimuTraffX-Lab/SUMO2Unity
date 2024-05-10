using UnityEngine;

namespace VRDriving.TrailerSystem
{
    /// <summary>
    /// A component that receives hitch trigger components.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class HitchReceiverTrigger : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("A reference to the ConfigurableJoint that will be used as the hitch joint.")]
        public ConfigurableJoint hitchJoint;

        [Header("Events")]
        [Tooltip("An event that is invoked when this HitchReceiverTrigger component starts towing a HitchTrigger.\n\nArg0: HitchTrigger - that component that started being towed.\nArg1: HitchReceiverTrigger - This component.")]
        public HitchUnityEvent Hitched;
        [Tooltip("An event that is invoked when this HitchReceiverTrigger component stops towing a HitchTrigger.\n\nArg0: HitchTrigger - the component that is no longer being towed.\nArg1: HitchReceiverTrigger - This component.")]
        public HitchUnityEvent Unhitched;

        /// <summary>Returns true if the hitch joint is connected to a Rigidbody, otherwise false.</summary>
        public bool IsTowing { get { return hitchJoint.connectedBody != null; } }
        /// <summary>Returns the HitchTrigger being towed by this component, otherwise null.</summary>
        public HitchTrigger Towing { get; private set; }

        // Unity callback(s).
        void OnTriggerEnter(Collider pOther)
        {
            // Only attempt to connect trailers while not already towing.
            if (!IsTowing && pOther.attachedRigidbody != null)
            {
                // Check that a HitchTrigger triggered this.
                HitchTrigger hitchTrigger = pOther.GetComponent<HitchTrigger>();
                if (hitchTrigger != null && !hitchTrigger.IsHitched)
                {
                    // Hitch the hitch trigger.
                    Hitch(hitchTrigger);
                }
            }
        }

        // Public method(s).
        /// <summary>Unhitches any HitchTrigger currently being towed.</summary>
        public void Unhitch()
        {
            // Only unhitch if towing.
            if (IsTowing)
            {
                // Store 'was towing' reference.
                HitchTrigger wasTowing = Towing;

                // Nullify 'Towing' refernece.
                Towing = null;

                // Clear connected body.
                hitchJoint.connectedBody = null;

                // Invoke the Unhitched Unity event.
                Unhitched?.Invoke(wasTowing, this);
            }
        }

        // Private method(s).
        /// <summary>Hitches the specified HitchTrigger to this component.</summary>
        /// <param name="pHitchTrigger"></param>
        void Hitch(HitchTrigger pHitchTrigger)
        {
            // Invoke the HitchTrigger's internal hitch event.
            if (pHitchTrigger.Internal_TryHitch(this))
            {
                //TODO: Position the trailer properly over time before actually connecting it.
                // Can be hitched, finish hitching by connecting the body.
                hitchJoint.connectedBody = pHitchTrigger.hitchBody;

                // Set 'Towing' reference.
                Towing = pHitchTrigger;

                // Invoke Hitched Unity event.
                Hitched?.Invoke(pHitchTrigger, this);
            }
        }
    }
}