using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;

namespace dynamitemod
{
    public class EntityDynamite : Entity
    {
        float timer = 0f;
        float fuse = 3f;

        public override void OnGameTick(float dt)
        {
            base.OnGameTick(dt);

            if (World.Side != EnumAppSide.Server) return;

            timer += dt;

            if (timer >= fuse)
            {
                World.BlockAccessor.BreakBlock(Pos.AsBlockPos, null);
                Die();
            }
        }
    }
}