using Vintagestory.API.Common;

namespace DynamiteMod
{
    public class DynamiteMod : ModSystem
    {
        public override void StartPre(ICoreAPI api)
        {
            api.Logger.Notification("[dynamitemod] Registering classes");

            api.RegisterItemClass("ItemDynamite", typeof(ItemDynamite));
            api.RegisterEntity("dynamite", typeof(EntityDynamite));
        }
    }
}