using UnityEngine;
using UnityEngine.Events;

namespace VRDriving.Invokers
{
    /// <summary>
    /// A component that invokes event(s) based on the distance of transformA from transformB being above or below a threshold.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class DistanceThresholdEvents : MonoBehaviour
    {
        [Header("Settings")]
        [Min(0f)]
        [Tooltip("The threshold setting.")]
        public float distanceThreshold = 0.075f;
        [Tooltip("The first transform in the distance test.")]
        public Transform transformA;
        [Tooltip("The second transform in the distance test.")]
        public Transform transformB;

        [Header("Events")]
        [Tooltip("An event that is invoked when the distance threshold has been equalled or surpassed in the positive direction. Only invoked if the last invocation was a threshold unreached invocation.")]
        public UnityEvent ThresholdReached = new UnityEvent();
        [Tooltip("An event that is invoked when the distance threshold has been passed in the negative direction. Only invoked if the last invocation was a threshold reached invocation.")]
        public UnityEvent ThresholdUnreached = new UnityEvent();

        /// <summary>Tracks whether the last invocation was above or below the threshold (defaults to true, below threshold). </summary>
        public bool IsBelowThreshold { get; protected set; } = true;

        // Unity callback(s).
        void Update()
        {
            // Compute distnace between transformA and transformB.
            float distance = Vector3.Distance(transformA.position, transformB.position);

            // If the last invocation was below the threshold check if the threshold has been passed or equal'd.
            if (IsBelowThreshold)
            {
                // Check if the threshold has been passed or equal'd.
                if (distance >= distanceThreshold)
                    OnDistanceThresholdPassedPositive();
            }
            // Otherwise the last invocation was above the threshold check if the threshold has been passed in the negative direction.
            else if (distance < distanceThreshold)
            {
                OnDistanceThresholdPassedNegative();
            }
        }

        // Protected callback(s).
        /// <summary>An event that is invoked when the distance threshold has been passed in the positive direction.</summary>
        protected void OnDistanceThresholdPassedPositive()
        {
            // Track that the component invocation above (or equal to) the threshold wasm ade.
            IsBelowThreshold = false;
           
            // Invoke the 'ThresholdReached' Unity event.
            ThresholdReached?.Invoke();
        }

        /// <summary>An event that is invoked when the distance threshold has been passed in the negative direction.</summary>
        protected void OnDistanceThresholdPassedNegative()
        {
            // Track that the component invocation below the threshold was made.
            IsBelowThreshold = true;

            // Invoke the 'ThresholdUnreached' Unity event.
            ThresholdUnreached?.Invoke();
        }
    }
}
