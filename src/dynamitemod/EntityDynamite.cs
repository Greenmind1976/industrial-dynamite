using System;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace DynamiteMod
{
    public class EntityDynamite : Entity
    {
        private float timer;
        private float fuseTime;
        private string cap;

        public override void Initialize(EntityProperties properties)
        {
            base.Initialize(properties);

            cap = Attributes.GetString("cap", "copper");
            fuseTime = cap == "titanium" ? 5f : 3f;
            timer = 0f;
        }

        public override void OnGameTick(float dt)
        {
            base.OnGameTick(dt);

            if (World.Side != EnumAppSide.Server) return;

            timer += dt;

            if (timer >= fuseTime)
            {
                World.BlockAccessor.BreakBlock(Pos.AsBlockPos, null);
                Die();
            }
        }
    }
}