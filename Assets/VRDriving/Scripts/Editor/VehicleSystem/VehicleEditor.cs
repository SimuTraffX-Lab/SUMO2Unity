using UnityEngine;
using UnityEditor;
using VRDriving.VehicleSystem;

namespace VRDriving.Editor.VehicleSystem
{
    /// <summary>
    /// A custom inspector for Vehicles.
    /// </summary>
    /// Author: Mathew Aloisio
    [CustomEditor(typeof(Vehicle)), CanEditMultipleObjects]
    public class VehicleEditor : UnityEditor.Editor
    {
        // Unity callback(s).
        public override void OnInspectorGUI()
        {
            // Draw the default inspector.
            DrawDefaultInspector();

            // Draw custom inspector elements for Vehicle.
            Vehicle vehicle = (Vehicle)target;

            // Section: Play mode only UI.
            if (Application.isPlaying)
            {
				// Section: Play mode only UI ONLY ACTIVE WHEN NON-MULTI-EDIT.
				if (Selection.objects.Length == 1)
				{
					if (vehicle.IsEngineOn)
					{
						// Button: Stop Engine - Stop the Vehicle's engine.
						if (GUILayout.Button("Stop Engine"))
							vehicle.StopEngine();
					}
					else if (vehicle.IsStarterOn)
					{
						// Button: Deactivate Starter - Deactivate the Vehicle's starter.
						if (GUILayout.Button("Deactivate Starter"))
							vehicle.DeactivateStarter();
					}
					else
					{
						// // Button: Instant Start - Instantly starts the vehicle's engine.
						if (GUILayout.Button("Instant Start"))
							vehicle.StartEngine();
						// Button: Activate Starter - Activate the Vehicle's starter.
						if (GUILayout.Button("Activate Starter"))
							vehicle.ActivateStarter();
					}
				}
            }
        }
    }
}
