using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using GrabSystem;
using VRDriving.VehicleSystem;
using VRDriving.Collisions;
using VRDriving.Transformation;
using VRDriving.PhysicsSystem;
using VRDriving.Grabbing;
using VRDriving.Steering;

namespace VRDriving.Editor
{
    /// <summary>
    /// A tool designed to allow you to easily create the 'blank' object hierarchy for drivable vehicles.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class DrivableCarSetupWizardWindow : EditorWindow
    {
        // SteeringType.
        public enum SteeringType
        {
            None,
            SteeringWheel,
            Handlebars
        }

        // WheelInfo.
        [Serializable]
        public class WheelInfo
        {
            [Tooltip("A reference to a Transform the wheel collider will be placed onto.")]
            public Transform transform;
            [Tooltip("Is this wheel a motor wheel?")]
            public bool isMotor;
            [Tooltip("Is this wheel a wheel that steers?")]
            public bool isSteering;
            [Tooltip("(Optional) An array of extra visual Transform(s) for the wheel, most wheels have just one dualies like on semitrucks may drive multiple visual Transforms with a single wheel collider.")]
            public Transform[] extraVisualTransforms;
            [Tooltip("(Optional) The 'side' the wheel is on.")]
            public VehicleWheel.Side wheelSide;
        }

        // WheelLookupEntry.
        [Serializable]
        public class WheelLookupEntry
        {
            [Tooltip("The equivalent wheel transform in the new duplicated car physics body.")]
            public Transform equivalentTransform;
            [Tooltip("The WheelInfo associated with this entry.")]
            public WheelInfo wheelInfo;
        }

        // DrivableCarSetupWizardWindow.
        [Tooltip("A reference to the 'car mesh' Transform that will be copied into the drivable vehicle being configured.")]
        public Transform carMeshTransform;
        [Tooltip("An array of wheel information for the drivable vehicle being configured.")]
        public WheelInfo[] wheelInfos;
        [Tooltip("(Optional) A reference to the Transform that represents the steering mechanism (wheels, handlebars, etc) for this vehicle. This Transform will be automatically moved into the vehicle's interior.")]
        public Transform steeringTransform;
        [Tooltip("(Optional) If a 'steeringTransform' is specified this may automatically be used to attempt to configure the specified steering type on the drivable vehicle automatically.")]
        public SteeringType steeringType;

        // Delegate C# event(s).
        /// <summary>A C# delegate event that is invoked when the DrivableCarSetupWizardWindow is intialized.</summary>
        public static event Action<DrivableCarSetupWizardWindow> Initialized;

        // Public constant(s).
        public const string DRIVEABLE_CAR_PREFIX = "DrivableCar_";

        bool m_GeneralSettingsGroupEnabled = true;

        SerializedObject m_SerializedObject;

        // General properties.
        SerializedProperty m_SerializedCarMeshTransformProperty;
        SerializedProperty m_SerializedWheelInfosProperty;
        SerializedProperty m_SerializedSteeringTransformProperty;
        SerializedProperty m_SerializedSteeringTypeProperty;

        /// <summary>The scroll position for the editor window.</summary>
        Vector2 m_ScrollPosition = Vector2.zero;

        // Unity callback(s).
        [MenuItem("Tools/VRDriving/Drivable Car Setup Wizard")]
        static void Init()
        {
            // Get existing open window or if none, make a new one.
            DrivableCarSetupWizardWindow window = (DrivableCarSetupWizardWindow)EditorWindow.GetWindow(typeof(DrivableCarSetupWizardWindow), false, "Drivable Car Setup Wizard");
            window.Show();
        }

        void OnEnable()
        {
            // Create serialized object for this editor window.
            m_SerializedObject = new SerializedObject(this);

            // Find serialized properties for the created serialized object.
            m_SerializedCarMeshTransformProperty = m_SerializedObject.FindProperty(nameof(carMeshTransform));
            if (m_SerializedCarMeshTransformProperty == null)
                Debug.LogError("No 'carMeshTransform' property found when looking for serialized property in DrivableCarSetupWizardWindow!");
            m_SerializedWheelInfosProperty = m_SerializedObject.FindProperty(nameof(wheelInfos));
            if (m_SerializedWheelInfosProperty == null)
                Debug.LogError("No 'wheelInfos' property found when looking for serialized property in DrivableCarSetupWizardWindow!");
            m_SerializedSteeringTransformProperty = m_SerializedObject.FindProperty(nameof(steeringTransform));
            if (m_SerializedSteeringTransformProperty == null)
                Debug.LogError("No 'steeringTransform' property found when looking for serialized property in DrivableCarSetupWizardWindow!");
            m_SerializedSteeringTypeProperty = m_SerializedObject.FindProperty(nameof(steeringType));
            if (m_SerializedSteeringTypeProperty == null)
                Debug.LogError("No 'steeringType' property found when looking for serialized property in DrivableCarSetupWizardWindow!");

            // Invoke the 'Initialized' C# delegate event.
            Initialized?.Invoke(this);
        }

        void OnGUI()
        {
            // SECTION: GUI.
            m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);

            // Title label.
            GUILayout.Label("Drivable Car Setup", EditorStyles.boldLabel);

            // Usage description
            EditorGUILayout.HelpBox("USAGE: The setup wizard will automatically duplicate the specified 'car mesh' Transform into a parent object making one valid base object hierarchy for a drivable vehicle.", MessageType.Info);

            // General settings toggle group.
            m_GeneralSettingsGroupEnabled = EditorGUILayout.Foldout(m_GeneralSettingsGroupEnabled, "General Settings");
            if (m_GeneralSettingsGroupEnabled)
            {
                // General settings property field(s).
                EditorGUILayout.PropertyField(m_SerializedCarMeshTransformProperty);
                EditorGUILayout.PropertyField(m_SerializedWheelInfosProperty);

                // Car mesh related property(s).
                if (carMeshTransform != null)
                {
                    EditorGUILayout.PropertyField(m_SerializedSteeringTransformProperty);
                    if (steeringTransform != null) // Only show steering type if a valid steering transform is specified.
                    {
                        // Ensure steering transform specified is a child of the car mesh Transform.
                        if (steeringTransform.IsChildOf(carMeshTransform))
                        {
                            EditorGUILayout.PropertyField(m_SerializedSteeringTypeProperty);
                        }
                        else { EditorGUILayout.HelpBox("The 'steering transform' you specified is not part of the specified 'Car Mesh Transform' and therefore will be ignored.", MessageType.Warning); }
                    }
                }
            }

            // Apply modified properties.
            m_SerializedObject.ApplyModifiedProperties();

            // Detect if in prefab mode.
            bool inPrefabMode = carMeshTransform != null ? EditorSceneManager.IsPreviewScene(carMeshTransform.gameObject.scene) : false;

            // Prefab mode warning.
            if (inPrefabMode)
                EditorGUILayout.HelpBox("You cannot set up drivable cars while in prefab-editing mode! Please exit the prefab editor and setup the vehicle within a scene (even a blank scene).", MessageType.Warning);

            // Setup button.
            if (GUILayout.Button("Setup"))
                SetupDrivableCar();

            // END OF GUI SECTION.
            EditorGUILayout.EndScrollView();
        }

        // Private method(s).
        /// <summary>Sets up a drivable car object based on the specified settings in the wizard.</summary>
        void SetupDrivableCar()
        {
            // Ensure a valid 'carTransform' is specified.
            if (carMeshTransform != null)
            {
                // Detect if in prefab mode.
                bool inPrefabMode = EditorSceneManager.IsPreviewScene(carMeshTransform.gameObject.scene);

                // Don't allow use of editor in prefab mode.
                if (!inPrefabMode)
                {
                    // Create new undo group.
                    Undo.IncrementCurrentGroup();

                    // Register undo for 'carMeshTransform'.
                    Undo.RegisterFullObjectHierarchyUndo(carMeshTransform.gameObject, "Modify carMeshTransform State");

                    // 1. Create container object.
                    string containerObjectName = DRIVEABLE_CAR_PREFIX + carMeshTransform.gameObject.name;
                    GameObject carContainer = new GameObject(containerObjectName);
                    Undo.RegisterCreatedObjectUndo(carContainer, "Create Car Container");
                    carContainer.transform.SetPositionAndRotation(carMeshTransform.transform.position, carMeshTransform.transform.rotation);

                    // 2. Duplicate 'car mesh transform' and put it in the container, make sure it has a local position and euler angles with all axes equal to 0.
                    GameObject carPhysicsBody = Instantiate(carMeshTransform.gameObject);
                    Undo.RegisterCreatedObjectUndo(carPhysicsBody, "Create Car Physics Body");
                    carPhysicsBody.name = DRIVEABLE_CAR_PREFIX + "PhysicsBody";
                    Undo.SetTransformParent(carPhysicsBody.transform, carContainer.transform, "Set Car Physics Body Parent");
                    carPhysicsBody.transform.localPosition = Vector3.zero;
                    carPhysicsBody.transform.localEulerAngles = Vector3.zero;
                    carPhysicsBody.SetActive(true); // Ensure the physics body is active for the car.

                    // 3. Add 'Rigidbody' and 'Vehicle' component to carPhysicsBody.
                    Rigidbody carPhysicsRigidbody = carPhysicsBody.GetComponent<Rigidbody>();
                    if (carPhysicsRigidbody == null)
                    {
                        carPhysicsRigidbody = Undo.AddComponent<Rigidbody>(carPhysicsBody);

                        // Set default 'carPhysicsRigidbody' values.
                        carPhysicsRigidbody.mass = 2000f;
                    }
                    Vehicle vehicleComponent = carPhysicsBody.GetComponent<Vehicle>();
                    if (vehicleComponent == null)
                    {
                        vehicleComponent = Undo.AddComponent<Vehicle>(carPhysicsBody);

                        // Set default values for 'vehicleComponent'.
                        vehicleComponent.maxMotorTorque = 850f;
                        vehicleComponent.maxSteeringAngle = 30;
                        vehicleComponent.brakeTorque = 800f;
                        vehicleComponent.decelerationForce = 400f;
                        vehicleComponent.reverseAccelerationMultiplier = 1f;
                        vehicleComponent.driftMinimumForwardVelocityDeviance = 0.55f;
                        vehicleComponent.driftStopDelay = 0.42f;
                    }

                    // 4. Create the vehicle's interior and set interior reference for 'vehicleComponent'.
                    GameObject carInterior = new GameObject(DRIVEABLE_CAR_PREFIX + "Interior");
                    Undo.RegisterCreatedObjectUndo(carInterior, "Create Car Interior");
                    Undo.SetTransformParent(carInterior.transform, carContainer.transform, "Set Car Interior Parent");
                    carInterior.transform.localPosition = Vector3.zero;
                    carInterior.transform.localEulerAngles = Vector3.zero;

                    vehicleComponent.interiorTransform = carInterior.transform;

                    // 5. Create 'center of mass' reference for car physics body & add 'RigidbodyCenterOfMass' component.
                    GameObject carCenterOfMass = new GameObject(DRIVEABLE_CAR_PREFIX + "CenterOfMass");
                    Undo.RegisterCreatedObjectUndo(carCenterOfMass, "Create Car Center Of Mass Reference");
                    Undo.SetTransformParent(carCenterOfMass.transform, carPhysicsBody.transform, "Set Car Center Of Mass Parent");
                    carCenterOfMass.transform.localPosition = Vector3.zero;
                    carCenterOfMass.transform.localEulerAngles = Vector3.zero;

                    RigidbodyCenterOfMass centerOfMassComponent = Undo.AddComponent<RigidbodyCenterOfMass>(carPhysicsBody);
                    centerOfMassComponent.comTransform = carCenterOfMass.transform;

                    // 6. Attach 'follow transform' component to carInterior, reference carPhysicsBody.
                    FollowTransform followTransformComponent = Undo.AddComponent<FollowTransform>(carInterior);
                    followTransformComponent.followTransform = carPhysicsBody.transform;

                    // 7. Attach 'ignore collisions between' component to carInterior, reference carPhysicsBody.
                    IgnoreCollisionsBetweenGameObjects ignoreCollisionsComponent = Undo.AddComponent<IgnoreCollisionsBetweenGameObjects>(carInterior);
                    ignoreCollisionsComponent.referenceObject = carPhysicsBody;

                    // Generate wheel transform lookup for new copied instance.
                    // Find all equivalent wheel transforms.
                    // KEY: Transform - The original wheel Transform | VALUE: Transform - The new, equivalent wheel collider Transform.
                    Dictionary<Transform, WheelLookupEntry> wheelLookup = new Dictionary<Transform, WheelLookupEntry>();
                    for (int i = 0; i < wheelInfos.Length; ++i)
                    {
                        WheelInfo wheelInfo = wheelInfos[i];
                        if (wheelInfo.transform != null)
                        {
                            string pathFromParent = GetTransformPathFromParent(wheelInfo.transform, carMeshTransform);
                            Transform equivalenTransform = carPhysicsBody.transform.Find(pathFromParent);
                            if (equivalenTransform != null)
                            {
                                wheelLookup[wheelInfo.transform] = new WheelLookupEntry() { equivalentTransform = equivalenTransform, wheelInfo = wheelInfo };
                            }
                            else { Debug.LogWarning("[Drivable Car Setup Wizard] No 'equivalent wheel collider' found for wheel info in index " + i + "! Ignoring wheel in setup... (Report this to a developer, this should not happen.)"); }
                        }
                        else { Debug.LogWarning("[Drivable Car Setup Wizard] No 'collider transform' specified for wheel info in index " + i + "! Ignoring wheel in setup..."); }
                    }

                    // 8. Make a 'car wheel container' and use the 'Wheel Lookup' table to make the wheels.
                    GameObject carWheelContainer = new GameObject(DRIVEABLE_CAR_PREFIX + "WheelContainer");
                    Undo.RegisterCreatedObjectUndo(carWheelContainer, "Create Car Wheel Container");
                    Undo.SetTransformParent(carWheelContainer.transform, carPhysicsBody.transform, "Set Car Wheel Container Parent");
                    carWheelContainer.transform.localPosition = Vector3.zero;
                    carWheelContainer.transform.localEulerAngles = Vector3.zero;

                    List<Vehicle.WheelInfo> vehicleWheelInfos = new List<Vehicle.WheelInfo>(); // A list to track Vehicle.WheelInfos.
                    foreach (var pair in wheelLookup)
                    {
                        // Make wheel pivot.
                        GameObject wheelPivot = new GameObject("Pivot_" + pair.Key.gameObject.name);
                        Undo.RegisterCreatedObjectUndo(wheelPivot, "Create Wheel Pivot");
                        Undo.SetTransformParent(wheelPivot.transform, carWheelContainer.transform, "Set Wheel Pivot Parent");
                        wheelPivot.transform.position = pair.Key.position;
                        wheelPivot.transform.localEulerAngles = Vector3.zero;

                        // Move equivalent wheel transform into the pivot and rename it giving it a geometry prefix.
                        Undo.SetTransformParent(pair.Value.equivalentTransform, wheelPivot.transform, true, "Set Wheel Transform Parent To Pivot");
                        pair.Value.equivalentTransform.gameObject.name = "Geometry_" + pair.Value.equivalentTransform.gameObject.name;

                        // Make wheel collider & add component.
                        GameObject wheelCollider = new GameObject("Collider_" + pair.Key.gameObject.name);
                        Undo.RegisterCreatedObjectUndo(wheelCollider, "Create Wheel Collider");
                        Undo.SetTransformParent(wheelCollider.transform, wheelPivot.transform, "Set Wheel Collider Parent");
                        wheelCollider.transform.localPosition = Vector3.zero;
                        wheelCollider.transform.localEulerAngles = Vector3.zero;

                        WheelCollider wheelColliderComponent = Undo.AddComponent<WheelCollider>(wheelCollider);

                        // Add VehicleWheel component to pivot.
                        VehicleWheel vehicleWheel = Undo.AddComponent<VehicleWheel>(wheelPivot);
                        vehicleWheel.motor = pair.Value.wheelInfo.isMotor;
                        vehicleWheel.steering = pair.Value.wheelInfo.isSteering;
                        vehicleWheel.wheelSide = pair.Value.wheelInfo.wheelSide;
                        vehicleWheel.wheelCollider = wheelColliderComponent;
                        vehicleWheel.brakeStrengthMultiplier = 1f;

                        // Setup visual wheel transforms.
                        List<Transform> visualWheelTransforms = new List<Transform>();
                        visualWheelTransforms.Add(pair.Value.equivalentTransform);
                        foreach (Transform t in pair.Value.wheelInfo.extraVisualTransforms)
                        {
                            if (!visualWheelTransforms.Contains(t))
                                visualWheelTransforms.Add(t);
                        }
                        vehicleWheel.wheelTransforms = visualWheelTransforms.ToArray();

                        // Add VehicleWheel component to 'vehicleWheelInfos' list.
                        vehicleWheelInfos.Add(new Vehicle.WheelInfo() { component = vehicleWheel });
                    }

                    // Set wheel array in vehicleComponent using vehicleWheelInfos list.
                    vehicleComponent.wheels = vehicleWheelInfos;

                    // 9. Setup steering (if required references are set).
                    if (steeringTransform != null)
                    {
                        string steeringTransformPathFromParent = GetTransformPathFromParent(steeringTransform, carMeshTransform);
                        Transform equivalentSteeringTransform = carPhysicsBody.transform.Find(steeringTransformPathFromParent);
                        if (equivalentSteeringTransform != null)
                        {
                            // Create steering container object.
                            GameObject steeringContainer = new GameObject("SteeringMechanism");
                            Undo.RegisterCreatedObjectUndo(steeringContainer, "Create Steering Mechanism Container");
                            Undo.SetTransformParent(steeringContainer.transform, carInterior.transform, "Set Steering Container Parent");
                            steeringContainer.transform.SetPositionAndRotation(equivalentSteeringTransform.position, equivalentSteeringTransform.rotation);

                            // Add relevant 'grab' related components to the steering container.
                            GrabbableObject grabbableObject = Undo.AddComponent<GrabbableObject>(steeringContainer);
                            if (grabbableObject != null)
                            {
                                grabbableObject.grabMode = GrabbableObject.GrabMode.MaintainOffset;
                            }    

                            // Create steering pivot object.
                            GameObject steeringPivot = new GameObject("Pivot_" + equivalentSteeringTransform.gameObject.name);
                            Undo.RegisterCreatedObjectUndo(steeringPivot, "Create Steering Pivot");
                            Undo.SetTransformParent(steeringPivot.transform, steeringContainer.transform, "Set Steering Pivot Parent");
                            steeringPivot.transform.localPosition = Vector3.zero;
                            steeringPivot.transform.localEulerAngles = Vector3.zero;

                            // Move equivalentSteeringTransform into steeringContainer.
                            Undo.SetTransformParent(equivalentSteeringTransform.transform, steeringPivot.transform, true, "Set Equivalent Steering Transform Parent");

                            // Add relevant steering component(s) if specified.
                            VehicleSteeringBase steeringComponent;
                            VehicleKinematicSteering kinematicSteeringComponent;
                            switch (steeringType)
                            {
                                case SteeringType.None:
                                    break;
                                case SteeringType.SteeringWheel:
                                    // Add VehicleHandRelativeSteering component.
                                    steeringComponent = Undo.AddComponent<VehicleHandRelativeSteering>(steeringContainer);
                                    steeringComponent.vehicleTransform = carPhysicsBody.transform;
                                    steeringComponent.vehicleRigidbody = carPhysicsRigidbody;
                                    (steeringComponent as VehicleHandRelativeSteering).steeringWheelTransform = steeringPivot.transform;

                                    // Add VehicleKinematicSteering component to carPhysicsBody and reference steering component from above.
                                    kinematicSteeringComponent = Undo.AddComponent<VehicleKinematicSteering>(carPhysicsBody);
                                    kinematicSteeringComponent.steering = steeringComponent;

                                    break;
                                case SteeringType.Handlebars:
                                    // Add VehicleHandRelativeHandlebarSteering component.
                                    steeringComponent = Undo.AddComponent<VehicleHandRelativeHandlebarSteering>(steeringContainer);
                                    steeringComponent.vehicleTransform = carPhysicsBody.transform;
                                    steeringComponent.vehicleRigidbody = carPhysicsRigidbody;
                                    (steeringComponent as VehicleHandRelativeHandlebarSteering).handlebarTransform = steeringPivot.transform;

                                    // Add VehicleKinematicSteering component to carPhysicsBody and reference steering component from above.
                                    kinematicSteeringComponent = Undo.AddComponent<VehicleKinematicSteering>(carPhysicsBody);
                                    kinematicSteeringComponent.steering = steeringComponent;
                                    break;
                                default:
                                    Debug.LogWarning("[Drivable Car Setup Wizard] Unhandled 'steeringType' found '" + steeringType.ToString() + "'! Report this problem to a developer...");
                                    break;
                            }
                        }
                    }

                    // Select the newly created object and disable the original 'carMeshTransform'.
                    Selection.activeGameObject = carContainer;
                    carMeshTransform.gameObject.SetActive(false);

                    // Name undo group.
                    Undo.SetCurrentGroupName("Drivable Car Setup");

                    // Mark the container game object dirty.
                    EditorUtility.SetDirty(carContainer);

                    // Log setup complete.
                    Debug.Log("[Drivable Car Setup Wizard] Drivable car setup wizard complete!", carContainer);
                }
                else { Debug.LogWarning("[Drivable Car Setup Wizard] Unable to 'setup' drivable car while in prefab mode!"); }
            }
            else { Debug.LogWarning("[Drivable Car Setup Wizard] Unable to 'setup' drivable car! No 'Car Mesh Transform' was specified."); }

            // Private static method(s).
            /// <summary>Generates the 'Transform Path' of the specified Transform, pTransform, from the specified parent pParent. Useable in methods like GameObject.Find().</summary>
            /// <returns>a string representing the 'Transform Path' of pTransform from the specified parent, pParent, useable in methods like GameObject.Find().</returns>
            static string GetTransformPathFromParent(Transform pTransform, Transform pParent)
            {
                if (pTransform.parent == null || pTransform.parent == pParent)
                    return pTransform.name;
                return GetTransformPathFromParent(pTransform.parent, pParent) + "/" + pTransform.name;
            }
        }
    }
}
