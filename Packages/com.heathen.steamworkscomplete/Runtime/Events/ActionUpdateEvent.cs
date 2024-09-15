#if !DISABLESTEAMWORKS && HE_SYSCORE && STEAMWORKSNET
using UnityEngine.Events;

namespace HeathenEngineering.SteamworksIntegration
{
    /// <summary>
    /// Unity Event for <see cref="InputActionUpdate"/> data
    /// </summary>
    [System.Serializable]
    public class ActionUpdateEvent : UnityEvent<InputActionUpdate>
    { }
}
#endif