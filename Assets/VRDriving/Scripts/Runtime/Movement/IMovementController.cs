using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace VRDriving.Movement
{
    /// <summary>
    /// An abstract base class for designs that intend to use a Mover stack to control how movement inputs are handled.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public abstract class IMovementController : MonoBehaviour
    {
        // MovementEvent.
        [Serializable]
        public class MovementEvent : UnityEvent<MovementInputs> { }

        // IMovementController.
        [Header("Enable/Disable")]
        [Tooltip("When enabled inputs are still simulated (or not) normally but only empty inputs are simulated.")]
        public bool ignoreInputs = false;

        [Header("Events")]
        [Tooltip("An event that is invoked when movement inputs have been gathered for a frame.")]
        public MovementEvent InputsGathered;

        /// <summary>
        /// The active mover being used to handle movement inputs.
        /// </summary>
        public IMover ActiveMover
        {
            get { return m_ActiveMover; }
            set
            {
                // Disable the old active mover behaviour.
                if (m_ActiveMover != null)
                    m_ActiveMover.enabled = false;

                m_ActiveMover = value;

                // Enable the new active mover behaviour if it is disabled.
                if (m_ActiveMover != null && !m_ActiveMover.enabled)
                    m_ActiveMover.enabled = true;
            }
        }
        /// <summary>The stack containing all movers affecting this MovementController.</summary>
        public Stack<IMover> MoverStack { get; private set; } = new Stack<IMover>();

        // Hidden backing field(s).
        /// <summary>The hidden backing field for the 'ActiveMover' property.</summary>
        IMover m_ActiveMover;

        // Unity callback(s).
		protected virtual void Update()
		{
			// Move the active mover when inputs are gathered.
            IMover mover = PeekMover();
            if (mover != null && mover.simulateInputs)
            {
                MovementInputs movementInputs = ignoreInputs ? MovementInputs.EmptyInputs() : mover.GatherMovementInputs();
                mover.Move(movementInputs);
            }
		}
		
        // Public method(s).
        /// <summary>
        /// Push a mover onto the stack.
        /// </summary>
        /// <returns></returns>
        public void PushMover(IMover pMover)
        {
            ActiveMover = pMover;
            MoverStack.Push(pMover);
        }

        /// <summary>
        /// Creates a mover of a given type and pushes it to the motor stack.
        /// </summary>
        /// <typeparam name="T">The type of mover being pushed to the stack.</typeparam>
        /// <returns>The IMover that was added to the stack.</returns>
        public void AddMover<T>() where T : IMover
        {
            IMover mover = gameObject.AddComponent<T>();
            PushMover(mover);
        }

        /// <summary>
        /// Returns the top element in the mover stack.
        /// </summary>
        /// <returns>The top element in the mover stack, or null if the stack is empty.</returns>
        public IMover PeekMover()
        {
            return MoverStack.Count > 0 ? MoverStack.Peek() : null;
        }

        /// <summary>
        /// Pop the current ActiveMover off of the stack and set the ActiveMover to the next one.
        /// </summary>
        /// <returns>The new ActiveMover.</returns>
        public IMover PopMover()
        {
            // Don't allow pop if there's only 1 mover left in the mover stacker.
            if (MoverStack.Count <= 1)
                return null;

            // Destroy the active mover.
            Destroy(ActiveMover);

            // Otherwise set the active mover to the top mover in the stack.
            MoverStack.Pop();
            ActiveMover = PeekMover();        

            return ActiveMover;
        }

        #region Public Setting Method(s)
        /// <summary>Sets the 'ignoreInputs' field of this component. Useful for use with Unity editor events.</summary>
        /// <param name="pIgnore"></param>
        public void SetIgnoreInputs(bool pIgnore) { ignoreInputs = pIgnore; }
        #endregion
    }
}
