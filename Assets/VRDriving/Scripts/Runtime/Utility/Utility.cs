using UnityEngine;

namespace VRDriving
{
    /// <summary>
    /// A static class that provdies utility functions relevant to the provided VR driving setup.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public static class Utility
    {
        /// <summary>
        /// Returns true if the GameObject has a visible renderer, otherwise false.
        /// </summary>
        /// <param name="pObject"></param>
        /// <returns>true if the GameObject has a visible renderer, otherwise false.</returns>
        public static bool IsGameObjectVisible(GameObject pObject)
        {
            Renderer renderer = pObject.GetComponent<Renderer>();
            if (renderer != null && renderer.isVisible)
                return true; // Visible.

            // Handle nested renderers.
            foreach (Renderer r in pObject.GetComponentsInChildren<Renderer>())
            {
                if (r.isVisible)
                    return true;
            }

            // Return false, no visible renderers in GameObject.
            return false;
        }
    }
}
