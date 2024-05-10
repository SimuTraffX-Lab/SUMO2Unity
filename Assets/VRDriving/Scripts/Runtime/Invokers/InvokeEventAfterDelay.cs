using UnityEngine;
using UnityEngine.Events;

namespace VRDriving.Invokers
{
    /// <summary>
    /// A component that provides a public DelayedInvoke(float pDelay) method that allows an event to be invoked after a given number of seconds.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class InvokeEventAfterDelay : MonoBehaviour
    {
        [Header("Events")]
        [Tooltip("An event that is invoked after the 'delay' expires after DelayedInvoke(...) is called.")]
        public UnityEvent Triggered;

        private float m_InvokeTime = float.NegativeInfinity;

        // Unity callback(s).
        void Update()
        {
            // Invoke if invoke time has been reached.
            if (m_InvokeTime != float.NegativeInfinity && Time.time > m_InvokeTime)
            {
                // Set 'invoke time' back to negative infinity to prevent re-invokation.
                m_InvokeTime = float.NegativeInfinity;

                // Invoke the 'Triggered' unity event.
                Triggered?.Invoke();
            }
        }

        // Public method(s).
        /// <summary>
        /// Invokes the 'Triggered' event of this component after pDelaySeconds pass from the Time.time DelayedInvoke(...) is called.
        /// </summary>
        /// <param name="pDelaySeconds"></param>
        public void DelayedInvoke(float pDelaySeconds)
        {
            m_InvokeTime = Time.time + pDelaySeconds;
        }

        /// <summary>
        /// Cancels any pending delayed invokations from this component.
        /// </summary>
        public void CancelDelayedInvoke()
        {
            // Set invoke time to float.NegativeInfinity to ensure any pending invokations are cancelled.
            m_InvokeTime = float.NegativeInfinity;
        }
    }
}
