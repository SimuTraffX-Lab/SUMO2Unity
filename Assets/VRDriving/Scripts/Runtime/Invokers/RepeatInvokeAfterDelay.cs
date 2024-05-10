using UnityEngine;
using UnityEngine.Events;

namespace VRDriving.Invokers
{
    /// <summary>
    /// A component that automatically invokes the unity event 'Triggered' every RepeatInvokeAfterDelay.delay seconds.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class RepeatInvokeAfterDelay : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("The number of seconds between event invocations of this component's 'Triggered' unity event.")]
        public float delay;

        [Header("Events")]
        [Tooltip("An event that is invoked every 'delay' seconds while this component is active.")]
        public UnityEvent Triggered;

        /// <summary>The last Time.time this component invoked it's event.</summary>
        float m_LastInvokeTime;

        // Unity callback(s).
        void Update()
        {
            // Invoke if invoke time has been reached.
            if (Time.time - m_LastInvokeTime >= delay)
                Invoke();
        }

        // Public method(s).
        /// <summary>
        /// A method that forces the 'Triggered' evvent of this component to be invoked.
        /// </summary>
        public void Invoke()
        {
            // Invoke the 'Triggered' unity event.
            Triggered?.Invoke();

            // Update last invoke time.
            m_LastInvokeTime = Time.time;
        }
    }
}
