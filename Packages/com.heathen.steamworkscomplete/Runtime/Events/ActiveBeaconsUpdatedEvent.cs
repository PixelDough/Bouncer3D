#if !DISABLESTEAMWORKS && HE_SYSCORE && STEAMWORKSNET
using Steamworks;
using UnityEngine.Events;

namespace HeathenEngineering.SteamworksIntegration
{
    /// <summary>
    /// Unity Event for <see cref="ActiveBeaconsUpdated_t"/> data
    /// </summary>
    [System.Serializable]
    public class ActiveBeaconsUpdatedEvent : UnityEvent<ActiveBeaconsUpdated_t> { }
}
#endif