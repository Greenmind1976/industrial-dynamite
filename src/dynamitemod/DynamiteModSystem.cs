using Vintagestory.API.Common;

namespace dynamitemod
{
    public class DynamiteModSystem : ModSystem
    {
        public override void StartPre(ICoreAPI api)
        {
            api.Logger.Notification("[dynamitemod] Registering classes");

            api.RegisterItemClass("ItemDynamite", typeof(ItemDynamite));
            api.RegisterEntity("dynamite", typeof(EntityDynamite));
        }
    }
}