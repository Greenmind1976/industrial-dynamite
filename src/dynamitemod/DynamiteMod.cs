using Vintagestory.API.Common;

namespace DynamiteMod
{
    public class DynamiteMod : ModSystem
    {
        public override void Start(ICoreAPI api)
        {
            api.RegisterItemClass("ItemDynamite", typeof(ItemDynamite));
            api.RegisterEntity("EntityDynamite", typeof(EntityDynamite));
        }
    }
}