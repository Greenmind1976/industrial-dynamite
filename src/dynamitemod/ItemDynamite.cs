using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

namespace DynamiteMod
{
    public class ItemDynamite : Item
    {
        public override bool OnHeldInteractStart(
            ItemSlot slot,
            EntityAgent byEntity,
            BlockSelection blockSel,
            EntitySelection entitySel,
            bool firstEvent,
            ref EnumHandHandling handling)
        {
            // Debug log (will help confirm this method is actually being called)
            byEntity?.World?.Logger?.Notification("[dynamitemod] Dynamite right click triggered");

            if (!firstEvent) return false;
            if (byEntity == null || byEntity.World == null) return false;

            // We only spawn entities server-side
            if (byEntity.World.Side != EnumAppSide.Server) return false;

            handling = EnumHandHandling.Handled;

            if (slot?.Itemstack == null) return false;

            string cap = slot.Itemstack.Collectible.Variant?["cap"] ?? "copper";
            IWorldAccessor world = byEntity.World;

            // Titanium: place charge on a block face (requires a block selection)
            if (cap == "titanium")
            {
                if (blockSel == null) return false;
                PlaceCharge(world, slot, blockSel, cap);
                return true;
            }

            // Other caps: throw charge
            ThrowCharge(world, slot, byEntity, cap);
            return true;
        }

        private void ThrowCharge(IWorldAccessor world, ItemSlot slot, EntityAgent byEntity, string cap)
        {
            EntityProperties type = world.GetEntityType(new AssetLocation("dynamitemod", "dynamite"));
            if (type == null) return;

            Entity entity = world.ClassRegistry.CreateEntity(type);
            if (entity == null) return;

            entity.ServerPos.SetPos(
                byEntity.Pos.X,
                byEntity.Pos.Y + 1.4,
                byEntity.Pos.Z
            );

            Vec3f viewf = byEntity.SidedPos.GetViewVector();
            Vec3d view = new Vec3d(viewf.X, viewf.Y, viewf.Z).Normalize();

            entity.ServerPos.Motion.Set(
                view.X * 0.9,
                view.Y * 0.6,
                view.Z * 0.9
            );

            entity.Pos.SetFrom(entity.ServerPos);

            entity.Attributes.SetString("cap", cap);
            entity.Attributes.SetBool("placed", false);
            entity.Attributes.SetDouble("fwdx", view.X);
            entity.Attributes.SetDouble("fwdy", view.Y);
            entity.Attributes.SetDouble("fwdz", view.Z);

            world.SpawnEntity(entity);

            slot.TakeOut(1);
            slot.MarkDirty();
        }

        private void PlaceCharge(IWorldAccessor world, ItemSlot slot, BlockSelection blockSel, string cap)
        {
            EntityProperties type = world.GetEntityType(new AssetLocation("dynamitemod", "dynamite"));
            if (type == null) return;

            Entity entity = world.ClassRegistry.CreateEntity(type);
            if (entity == null) return;

            Vec3d placePos = blockSel.Position.ToVec3d().Add(
                blockSel.Face.Normali.X * 0.501,
                blockSel.Face.Normali.Y * 0.501,
                blockSel.Face.Normali.Z * 0.501
            );

            entity.ServerPos.SetPos(placePos);
            entity.ServerPos.Motion.Set(0, 0, 0);
            entity.Pos.SetFrom(entity.ServerPos);

            entity.Attributes.SetString("cap", cap);
            entity.Attributes.SetBool("placed", true);

            entity.Attributes.SetDouble("fwdx", blockSel.Face.Normali.X);
            entity.Attributes.SetDouble("fwdy", blockSel.Face.Normali.Y);
            entity.Attributes.SetDouble("fwdz", blockSel.Face.Normali.Z);

            world.SpawnEntity(entity);

            slot.TakeOut(1);
            slot.MarkDirty();
        }
    }
}