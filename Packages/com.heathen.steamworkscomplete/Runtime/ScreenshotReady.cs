#if !DISABLESTEAMWORKS && HE_SYSCORE && STEAMWORKSNET
using Steamworks;
using System;

namespace HeathenEngineering.SteamworksIntegration
{
    [Serializable]
    public struct ScreenshotReady
    {
        public ScreenshotReady_t data;

        public readonly ScreenshotHandle Handle => data.m_hLocal;
        public readonly EResult Result => data.m_eResult;

        public static implicit operator ScreenshotReady(ScreenshotReady_t native) => new ScreenshotReady { data = native };
        public static implicit operator ScreenshotReady_t(ScreenshotReady heathen) => heathen.data;
    }
}
#endif