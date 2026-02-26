using Vintagestory.API.Common;

namespace dynamitemod
{
    public class DynamiteMod : ModSystem
    {
        public override void StartPre(ICoreAPI api)
        {
            api.Logger.Notification("[dynamitemod] Registering classes");

            api.RegisterItemClass("ItemDynamite", typeof(ItemDynamite));
            api.RegisterEntity("dynamite", typeof(EntityDynamite));

            // Debug: prove these types exist in the loaded DLL
            api.Logger.Notification("[dynamitemod] Item type = " + typeof(ItemDynamite).FullName);
            api.Logger.Notification("[dynamitemod] Entity type = " + typeof(EntityDynamite).FullName);
        }
    }
}