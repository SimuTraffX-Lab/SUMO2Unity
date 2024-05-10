using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;
using VRDriving.Steering;

namespace VRDriving.Haptics
{
    /// <summary>
    /// A component that allows haptics to be played on XR devices.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class HapticsManager : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("The channel to play haptics on.")]
        public uint hapticChannel = 0;
        [Tooltip("Should hands be automatically registered as haptic devices whenever they are connected?")]
        public bool autoRegisterHands = true;

        /// <summary>A dictionary of XRNodes are their respective InputDevices.</summary>
        Dictionary<XRNode, List<InputDevice>> m_Devices = new Dictionary<XRNode, List<InputDevice>>();

        // Unity callback(s).
        void Start()
        {
            InputDevices.deviceConnected += OnDeviceConnected;
            InputDevices.deviceDisconnected += OnDeviceDisconnected;
        }

        void OnDestroy()
        {
            InputDevices.deviceConnected -= OnDeviceConnected;
            InputDevices.deviceDisconnected -= OnDeviceDisconnected;
        }

        // Public method(s).
        /// <summary>Registers all XR devices in the XRNode with the haptics manager.</summary>
        /// <param name="pNode"></param>
        public void RegisterXRNode(XRNode pNode)
        {
            List<InputDevice> devices = new List<InputDevice>();
            InputDevices.GetDevicesAtXRNode(pNode, devices);
            if (devices != null && devices.Count > 0)
            {
                m_Devices[pNode] = devices;
            }
        }

        /// <summary>Deregisters all XR devices in the XRNode with the haptics manager.</summary>
        /// <param name="pNode"></param>
        public void DeregisterXRNode(XRNode pNode)
        {
            m_Devices.Remove(pNode);
        }

        /// <summary>
        /// Attempts to play a haptic pulse with the specified parameters on all devices registered with this HapticsManager.
        /// Note that haptics will only play while the HapticsManager component is enabled.
        /// </summary>
        /// <param name="pDuration"></param>
        /// <param name="pAmplitude"></param>
        /// <param name="pFrequency"></param>
        public void HapticImpulse(float pDuration, float pAmplitude, float pFrequency)
        {
            if (enabled)
            {
                foreach (var pair in m_Devices)
                {
                    foreach (InputDevice device in pair.Value)
                    {
                        if (device.TryGetHapticCapabilities(out var capabilities) && capabilities.supportsImpulse)
                            device.SendHapticImpulse(hapticChannel, pAmplitude, pDuration);
                    }
                }
            }
        }

        /// <summary>Stops haptics on all registered haptic devices.</summary>
        public void StopHaptics()
        {
            foreach (var pair in m_Devices)
            {
                foreach (InputDevice device in pair.Value)
                {
                    if (device.TryGetHapticCapabilities(out var capabilities) && capabilities.supportsImpulse)
                        device.StopHaptics();
                }
            }
        }

        /// <summary>A public method that allows the 'hapticChannel' field to be set. Useful for use with Unity editor events.</summary>
        /// <param name="pChannel"></param>
        public void SetHapticChannel(uint pChannel) { hapticChannel = pChannel; }

        #region Controller Compatability
        /// <summary>Registers XR haptic devices for the left and right hand.</summary>
        public void RegisterBothHands()
        {
            RegisterXRNode(XRNode.LeftHand);
            RegisterXRNode(XRNode.RightHand);
        }

        /// <summary>Deregisters XR haptic devices for the left and right hand.</summary>
        public void DeregisterBothHands()
        {
            DeregisterXRNode(XRNode.LeftHand);
            DeregisterXRNode(XRNode.RightHand);
        }

        /// <summary>A callback for registering left or right hand controller(s) as haptics devices using the ControllerSide enumerate.</summary>
        /// <param name="pSide"></param>
        public void RegisterControllerHaptics(ControllerSide pSide)
        {
            if (pSide == ControllerSide.Left)
            {
                RegisterXRNode(XRNode.LeftHand);
            }
            else { RegisterXRNode(XRNode.RightHand); }
        }

        /// <summary>A callback for deregistering left or right hand controller(s) as haptics devices using the ControllerSide enumerate.</summary>
        /// <param name="pSide"></param>
        public void DeregisterControllerHaptics(ControllerSide pSide)
        {
            if (pSide == ControllerSide.Left)
            {
                DeregisterXRNode(XRNode.LeftHand);
            }
            else { DeregisterXRNode(XRNode.RightHand); }
        }
        #endregion

        #region Event Compatability
        /// <summary>A callback for registering left or right hand controller(s) as haptics devices. The arguments are formatted in a way that controllers can be registered as haptics devices from GrabbableObject Grab event(s).</summary>
        /// <param name="pSteeringBase"></param>
        /// <param name="pInfo"></param>
        public void RegisterControllerHaptics(VehicleGrabbableSteeringBase pSteeringBase, ControllerInfo pInfo)
        {
            RegisterControllerHaptics(pInfo.side);
        }

        /// <summary>A callback for deregistering left or right hand controller(s) as haptics devices. The arguments are formatted in a way that controllers can be deregistered as haptics devices from GrabbableObject Release event(s).</summary>
        /// <param name="pSteeringBase"></param>
        /// <param name="pInfo"></param>
        public void DeregisterControllerHaptics(VehicleGrabbableSteeringBase pSteeringBase, ControllerInfo pInfo)
        {
            DeregisterControllerHaptics(pInfo.side);
        }
        #endregion

        #region GrabbableObject Event Compatability
        /// <summary>A callback for registering left or right hand controller(s) as haptics devices. The arguments are formatted in a way that controllers can be registered as haptics devices from Grab event(s) of components such as VehicleGrabbableSteeringBase derived components.</summary>
        /// <param name="pInfo"></param>
        public void RegisterControllerHaptics(ControllerInfo pInfo)
        {
            RegisterControllerHaptics(pInfo.side);
        }

        /// <summary>A callback for deregistering left or right hand controller(s) as haptics devices. The arguments are formatted in a way that controllers can be deregistered as haptics devices from Release event(s) of components such as VehicleGrabbableSteeringBase derived components.</summary>
        /// <param name="pInfo"></param>
        public void DeregisterControllerHaptics(ControllerInfo pInfo)
        {
            DeregisterControllerHaptics(pInfo.side);
        }
        #endregion

        // Private callback(s).
        void OnDeviceConnected(InputDevice pDevice)
        {
            // (Re)register hands if enabled.
            if (autoRegisterHands)
            {
                RegisterXRNode(XRNode.LeftHand);
                RegisterXRNode(XRNode.RightHand);
            }
        }

        void OnDeviceDisconnected(InputDevice pDevice)
        {
            // Check if this device exists anywhere in the devices dictionary, if it does remove it.
            bool removed = false;
            bool removeNode = false;
            XRNode nodeToRemove = XRNode.Head;
            foreach (var pair in m_Devices)
            {
                for (int i = 0; i < pair.Value.Count; ++i)
                {
                    if (pair.Value[i] == pDevice)
                    {
                        pair.Value.RemoveAt(i);
                        nodeToRemove = pair.Key;
                        removed = true;
                        break;
                    }
                }

                // If a device was removed break out of the loop after checking if the node should be removed from the dictionary.
                if (removed)
                {
                    if (m_Devices[nodeToRemove].Count == 0)
                        removeNode = true;
                    break;
                }
            }

            // If 'removeNode' is true remove the node from the dictionary.
            if (removeNode)
                m_Devices.Remove(nodeToRemove);
        }
    }
}
