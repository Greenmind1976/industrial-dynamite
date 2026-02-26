using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

namespace DynamiteMod
{
    public class ItemDynamite : Item
    {
        public override void OnHeldInteractStart(
            ItemSlot slot,
            EntityAgent byEntity,
            BlockSelection blockSel,
            EntitySelection entitySel,
            bool firstEvent,
            ref EnumHandHandling handling)
        {
            if (!firstEvent) return;

            byEntity.World.Logger.Notification("DYNAMITE CLICKED");

            handling = EnumHandHandling.Handled;

            if (byEntity.World.Side != EnumAppSide.Server) return;

            EntityProperties type = byEntity.World.GetEntityType(
                new AssetLocation("dynamitemod", "dynamite")
            );

            if (type == null) return;

            Entity entity = byEntity.World.ClassRegistry.CreateEntity(type);
            if (entity == null) return;

            entity.ServerPos.SetPos(
                byEntity.Pos.X,
                byEntity.Pos.Y + 1.4,
                byEntity.Pos.Z
            );

            Vec3f viewf = byEntity.SidedPos.GetViewVector();
            entity.ServerPos.Motion.Set(viewf.X * 0.8, viewf.Y * 0.6, viewf.Z * 0.8);
            entity.Pos.SetFrom(entity.ServerPos);

            byEntity.World.SpawnEntity(entity);

            slot.TakeOut(1);
            slot.MarkDirty();
        }
    }
}