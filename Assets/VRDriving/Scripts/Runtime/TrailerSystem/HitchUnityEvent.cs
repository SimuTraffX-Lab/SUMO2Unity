using System;
using UnityEngine.Events;

namespace VRDriving.TrailerSystem
{
    // HitchUnityEvent.
    /// <summary>
    /// Arg0: HitchTrigger - The HitchTrigger involved in the event.
    /// Arg1: HitchReceiverTrigger - The HitchReceiverTrigger involved in the event.
    /// </summary>
    public class HitchUnityEvent : UnityEvent<HitchTrigger, HitchReceiverTrigger> { }
}
