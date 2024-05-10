using UnityEngine;

namespace VRDriving.TrailerSystem
{
    /// <summary>
    /// A component that triggers hitch receiver trigger components.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class HitchTrigger : MonoBehaviour
    {
        // HitchTrigger.
        [Header("Settings")]
        [Tooltip("A reference to the Rigidbody that should be hitched by this component.")]
        public Rigidbody hitchBody;
        [Tooltip("The minimum number of seconds after being unhitched before this component can be towed again.")]
        public float rehitchDelay = 1.5f;

        [Header("Events")]
        [Tooltip("An event that is invoked when this HitchTrigger component is hitched to a HitchReceiverTrigger.\n\nArg0: HitchTrigger - this component.\nArg1: HitchReceiverTrigger - The component this HitchTrigger was hitched to.")]
        public HitchUnityEvent Hitched;
        [Tooltip("An event that is invoked when this HitchTrigger component is unhitched from a HitchReceiverTrigger.\n\nArg0: HitchTrigger - this component.\nArg1: HitchReceiverTrigger - The component this HitchTrigger was unhitched from.")]
        public HitchUnityEvent Unhitched;

        /// <summary>Returns true if this hitch trigger is currently hitched to something, otherwise false.</summary>
        public bool IsHitched { get { return HitchedTo != null; } }
        /// <summary>A reference to the HitchReceiverTrigger that this component is hitched to, otherwise null.</summary>
        public HitchReceiverTrigger HitchedTo { get; private set; }
        /// <summary>Returns the last Time.time this component was unhitched.</summary>
        public float LastUnhitchTime { get; private set; }

        // Public method(s).
        /// <summary>Returns true if the given HitchReceiverTrigger, pTrigger, may tow this HitchTrigger, otherwise false.</summary>
        /// <param name="pTrigger"></param>
        /// <returns>true if the given HitchReceiverTrigger, pTrigger, may tow this HitchTrigger, otherwise false.</returns>
        public bool CanHitch(HitchReceiverTrigger pTrigger)
        {
            return !IsHitched && Time.time - LastUnhitchTime > rehitchDelay;
        }

        // Internal method(s).
        /// <summary>
        /// Attempts to hitch the HitchTrigger to the given HitchReceiverTrigger, pTrigger. Returns the result.
        /// </summary>
        /// <param name="pTrigger"></param>
        /// <returns>true when the HitchTrigger is successfully hitched to the HitchReceiverTrigger, pTrigger, otherwise false.</returns>
        internal bool Internal_TryHitch(HitchReceiverTrigger pTrigger)
        {
            // Ensure this component can be towed by pTrigger.
            if (CanHitch(pTrigger))
            {
                // Update hitched to.
                HitchedTo = pTrigger;

                // Invoke the Hitched unity event.
                Hitched?.Invoke(this, pTrigger);

                // Successfully hitched.
                return true;
            }

            // Unable to hitch.
            return false;
        }

        /// <summary>Unhitches the HitchTrigger component.</summary>
        internal void Internal_Unhitch()
        {
            // Store 'last hitched to'.
            HitchReceiverTrigger wasHitchedTo = HitchedTo;

            // Nullify 'hitched to' reference.
            HitchedTo = null;

            // Handle unhitch event(s).
            if (wasHitchedTo != null)
            {
                // Update last unhitched time.
                LastUnhitchTime = Time.time;

                // Invoke the Unhitched unity event.
                Unhitched?.Invoke(this, wasHitchedTo);
            }           
        }
    }
}
