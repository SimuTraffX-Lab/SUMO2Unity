using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace VRDriving.Invokers
{
    /// <summary>
    /// A component that invokes a UnityEvent after an input action is completed for the first time.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class InvokeAfterInputActionPerformed : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("The InputAction to listen for.")]
        public InputAction listenForAction;
        [Tooltip("How many times must the action be performed before invoking the event.")]
        public int requireActionCount = 2;

        [Header("Events")]
        [Tooltip("An event that is invoked after the target input action was ran for the first time.")]
        public UnityEvent InputActionPerformed;

        private int m_ActionPerformedCount = 0;

        public bool Performed { get; protected set; } = false;

        // Unity callbacks.
        void OnEnable()
        {
            // Bind input action if one is set.
            if (listenForAction.bindings.Count > 0)
            {
                listenForAction.Enable();
                listenForAction.performed += OnInputActionPerformed;
            }
        }

        void OnDisable()
        {
            // Unbind input action if set and the action has not already been performed.
            UnbindActions();
        }

        // Private method(s).
        private void UnbindActions()
        {
            // Unbind input action if set and the action has not already been performed.
            if (listenForAction.bindings.Count > 0 && !Performed)
            {
                listenForAction.performed -= OnInputActionPerformed;
                listenForAction.Disable();
            }
        }

        // Protected callback(s).
        /// <summary>
        /// A callback that is invoked when any of 'listenForAction' input acitons are performed for the first time.
        /// </summary>
        protected void OnInputActionPerformed(InputAction.CallbackContext pContext)
        {
            // Increment action performed count.
            ++m_ActionPerformedCount;

            if (m_ActionPerformedCount >= requireActionCount)
            {
                // Invoke the input action performed event.
                InputActionPerformed?.Invoke();

                // Unbind actions before updating the 'Performed' boolean.
                UnbindActions();

                // Update the 'Performed' boolean.
                Performed = true;
            }
        }
    }
}
