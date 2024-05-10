using System;
using UnityEngine;

namespace VRDriving.Transformation
{
    /// <summary>
    /// A component that will set a Transform's position to a spawn point either closest to or furthest from a target depending on the positionChooseMode, the Random mode can be used to select a random spawn position.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class SetPositionByDistanceFrom : MonoBehaviour
    {
        // EnumeratE: PositionChooseMode
        [Serializable]
        public enum PositionChooseMode
        {
            Furthest,   // Choose the position furthest from the target.
            Closest,    // Choose the position closest to the target.
            Random      // Choose a random position.
        }

        // SetPositionByDistanceFrom.
        [Header("Settings")]
        [Tooltip("The target game object.")]
        public GameObject target;
        [Tooltip("The transform to move. (Uses gameObject.transform if null)")]
        public Transform moveTransform;
        [Tooltip("Should the rotation be ignored when moving to a position?")]
        public bool ignoreRotation = true;
        [Tooltip("A multiplier to multiply MoveTransform's euler angles by after rotating upon moving to a new position.")]
        public Vector3 eulerAngleMultiplier = new Vector3(0f, 1f, 0f);
        [Tooltip("The mode to use to select the position to move to.")]
        public PositionChooseMode positionChooseMode;
        [Tooltip("An array of valid positions.")]
        public Transform[] validPositions;

        /// <summary>
        /// Returns the 'moveTransform' field or 'transform' if 'moveTransform' was null.
        /// </summary>
        public Transform MoveTransform { get { return moveTransform != null ? moveTransform : transform; } }

        // Public method(s).
        /// <summary>
        /// Moves the 'MoveTransform' to the next valid position.
        /// </summary>
        public void MoveToPosition()
        {
            if (validPositions.Length > 0)
            {
                int index = 0;
                switch (positionChooseMode)
                {
                    case PositionChooseMode.Furthest:
                        float maxDistance = Vector3.Distance(validPositions[index].position, MoveTransform.position);
                        for (int i = 1; i < validPositions.Length; ++i)
                        {
                            // Calculate distance from potential position.
                            float distance = Vector3.Distance(validPositions[i].position, MoveTransform.position);

                            // If the distance is greater than the previous largest distance set the target position to this one.
                            if (distance > maxDistance)
                            {
                                index = i;
                                maxDistance = distance;
                            }
                        }

                        // Once the furthest spawn position has been determined move to it.
                        MoveToPosition(index);
                        break;
                    case PositionChooseMode.Closest:
                        float minDistance = Vector3.Distance(validPositions[index].position, MoveTransform.position);
                        for (int i = 1; i < validPositions.Length; ++i)
                        {
                            // Calculate distance from potential position.
                            float distance = Vector3.Distance(validPositions[i].position, MoveTransform.position);

                            // If the distance is less than the previous smallest distance set the target position to this one.
                            if (distance < minDistance)
                            {
                                index = i;
                                minDistance = distance;
                            }
                        }

                        // Once the closest spawn position has been determined move to it.
                        MoveToPosition(index);
                        break;
                    case PositionChooseMode.Random:
                        MoveToPosition(UnityEngine.Random.Range(0, validPositions.Length));
                       
                        break;
                    default:
                        Debug.LogWarning("Unhandled 'positionChooseMode'(" + positionChooseMode.ToString() + ") in SetPositionByDistanceFrom.MoveToPosition(). No position will be set!");
                        break;
                }
            }
            else { Debug.LogWarning("No 'validPositions' set in SetPositionByDistanceFrom component! Unable to 'MoveToPosition()'."); }
        }

        /// <summary>
        /// Moves directly to a valid position by array index.
        /// </summary>
        /// <param name="pIndex"></param>
        public void MoveToPosition(int pIndex)
        {
            if (pIndex >= 0 && pIndex < validPositions.Length)
            {
                // Disable character controller if one was found.
                CharacterController controller = MoveTransform.GetComponent<CharacterController>();
                if (controller != null)
                    controller.enabled = false;

                // Move transform.
                MoveTransform.position = validPositions[pIndex].position;
                if (!ignoreRotation)
                    SetMoveTransformRotation(validPositions[pIndex].rotation);

                // Reenable character controller if one was found.
                if (controller != null)
                    controller.enabled = true;
            }
            else { Debug.LogWarning("Failed to 'SetPositionByDistanceFrom.MoveToPosition(int pIndex)' as pIndex was out of range!"); }
        }

        // Private method(s).
        void SetMoveTransformRotation(Quaternion pRotation)
        {
            MoveTransform.rotation = pRotation;
            MoveTransform.eulerAngles = new Vector3(
                MoveTransform.eulerAngles.x * eulerAngleMultiplier.x,
                MoveTransform.eulerAngles.y * eulerAngleMultiplier.y,
                MoveTransform.eulerAngles.z * eulerAngleMultiplier.z
            );
        }
    }
}
