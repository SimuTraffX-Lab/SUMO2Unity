using UnityEngine;
using UnityEngine.Events;

namespace VRDriving.Invokers
{
    /// <summary>
    /// A component that invokes an event, Triggered, when the component's Unity callback OnEnable() is invoked.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class InvokeEventOnEnable : MonoBehaviour
    {
        [Header("Events")]
        [Tooltip("An event that is invoked after this component's OnEnable() Unity callback is invoked.")]
        public UnityEvent Triggered;

        // Unity callback(s).
        void OnEnable()
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
