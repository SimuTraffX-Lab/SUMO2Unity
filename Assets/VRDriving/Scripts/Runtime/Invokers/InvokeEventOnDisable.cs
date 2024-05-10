using UnityEngine;
using UnityEngine.Events;

namespace VRDriving.Invokers
{
    /// <summary>
    /// A component that invokes an event, Triggered, when the component's Unity callback OnDisable() is invoked.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class InvokeEventOnDisable : MonoBehaviour
    {
        [Header("Events")]
        [Tooltip("An event that is invoked after this component's OnDisable() Unity callback is invoked.")]
        public UnityEvent Triggered;

        // Unity callback(s).
        void OnDisable()
        {
            // Trigger the component.
            Trigger();
        }

        // Public method(s).
        /// <summary>
        /// Triggers the event associated with this component manually.
        /// </summary>
        public void Trigger()
        {
            // Invoke the 'Triggered' event.
            Triggered?.Invoke();
        }
    }
}
