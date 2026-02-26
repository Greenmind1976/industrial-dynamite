using Vintagestory.API.Common;

namespace DynamiteMod
{
    public class DynamiteMod : ModSystem
    {
        public override void Start(ICoreAPI api)
        {
            api.Logger.Notification("[dynamitemod] Mod loaded");
        }
    }
}