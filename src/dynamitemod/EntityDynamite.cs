using System;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

namespace DynamiteMod
{
    public class EntityDynamite : Entity
    {
        private float timer;
        private float fuseTime;

        private bool placed;
        private string cap;

        private Vec3d forward = new Vec3d(0, 0, 1);

        public override void OnEntitySpawn()
        {
            base.OnEntitySpawn();

            cap = Attributes.GetString("cap", "copper");
            placed = Attributes.GetBool("placed", false);

            double fx = Attributes.GetDouble("fwdx", 0);
            double fy = Attributes.GetDouble("fwdy", 0);
            double fz = Attributes.GetDouble("fwdz", 1);

            forward = new Vec3d(fx, fy, fz).Normalize();

            fuseTime = (cap == "titanium") ? 5f : 3f;
            timer = 0f;
        }

        public override void OnGameTick(float dt)
        {
            base.OnGameTick(dt);

            timer += dt;

            if (placed)
            {
                ServerPos.Motion.Set(0, 0, 0);
                Pos.Motion.Set(0, 0, 0);
            }

            if (!placed && cap != "titanium")
            {
                Block here = World.BlockAccessor.GetBlock(Pos.AsBlockPos);
                if (here?.LiquidCode != null)
                {
                    if (World.Rand.NextDouble() < 0.02)
                    {
                        Die();
                        return;
                    }
                }
            }

            if (timer >= fuseTime)
            {
                Explode();
            }
        }

        private void Explode()
        {
            if (!World.Side.IsServer()) return;

            int capTier = 2;
            double horizontalRadius = 4.0;
            double verticalRadius = 3.0;
            double oreDestroyBase = 0.20;

            bool useDirectionalCone = false;
            double coneThresholdDot = 0.0;

            switch (cap)
            {
                case "bronze":
                    capTier = 3;
                    oreDestroyBase = 0.15;
                    break;

                case "iron":
                    capTier = 4;
                    oreDestroyBase = 0.10;
                    break;

                case "steel":
                    capTier = 4;
                    horizontalRadius = 10.0;
                    verticalRadius = 6.0;
                    oreDestroyBase = 0.05;
                    useDirectionalCone = true;
                    coneThresholdDot = 0.40;
                    break;

                case "titanium":
                    capTier = 5;
                    horizontalRadius = 8.0;
                    verticalRadius = 1.5;
                    oreDestroyBase = 0.01;
                    useDirectionalCone = true;
                    coneThresholdDot = 0.55;
                    break;
            }

            BlockPos center = Pos.AsBlockPos;
            int hx = (int)Math.Ceiling(horizontalRadius);
            int vy = (int)Math.Ceiling(verticalRadius);

            for (int x = -hx; x <= hx; x++)
            for (int y = -vy; y <= vy; y++)
            for (int z = -hx; z <= hx; z++)
            {
                double dx = x / horizontalRadius;
                double dy = y / verticalRadius;
                double dz = z / horizontalRadius;
                double d2 = dx * dx + dy * dy + dz * dz;
                if (d2 > 1.0) continue;

                BlockPos bp = center.AddCopy(x, y, z);
                Block block = World.BlockAccessor.GetBlock(bp);
                if (block == null) continue;
                if (block.BlockMaterial != EnumBlockMaterial.Stone) continue;
                if (block.RequiredMiningTier > capTier) continue;

                if (useDirectionalCone)
                {
                    Vec3d toBlock = bp.ToVec3d().Add(0.5, 0.5, 0.5).Sub(Pos.XYZ).Normalize();
                    double dot = forward.Dot(toBlock);
                    if (dot < coneThresholdDot) continue;
                }

                bool isOre = block.Attributes?["ore"] != null;
                double centerBias = 1.0 - d2;
                double oreDestroyChance = oreDestroyBase * (0.35 + 0.65 * centerBias);

                if (isOre && World.Rand.NextDouble() < oreDestroyChance)
                {
                    World.BlockAccessor.SetBlock(0, bp);
                }
                else
                {
                    World.BlockAccessor.BreakBlock(bp, null);
                }
            }

            Die();
        }
    }
}