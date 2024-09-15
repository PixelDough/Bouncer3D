#if !DISABLESTEAMWORKS && HE_SYSCORE && STEAMWORKSNET

namespace HeathenEngineering.SteamworksIntegration
{
    /// <summary>
    /// The type applied to a Steam Inventory Item
    /// </summary>
    public enum InventoryItemType
    {
        item,
        bundle,
        generator,
        playtimegenerator,
        tag_generator,
    }
}
#endif