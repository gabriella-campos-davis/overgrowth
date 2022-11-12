using System.Collections.Generic;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Util;
using Vintagestory.GameContent;
using System.Linq;
using System;
using Vintagestory.API.Common.Entities;
using overgrowth.config;
using overgrowth;

namespace overgrowth
{
    public class OvergrowthBerryBush : BlockBerryBush
    {
        ItemStack clipping = new();
        public AssetLocation harvestingSound;

        public override void OnLoaded(ICoreAPI api)
        {
            base.OnLoaded(api);

            clipping = new ItemStack(api.World.GetItem(AssetLocation.Create("overgrowth:clipping-" + this.Variant["type"] + "-green")), 1);

            string code = "game:sounds/block/leafy-picking";
            if (code != null) {
                harvestingSound = AssetLocation.Create(code, this.Code.Domain);
            }
        }

        public override bool OnBlockInteractStart(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel)
        {

            if ((byPlayer?.InventoryManager?.ActiveHotbarSlot?.Itemstack?.Collectible?.Tool == EnumTool.Knife && OvergrowthConfig.Current.useKnifeForClipping) ||
                (byPlayer?.InventoryManager?.ActiveHotbarSlot?.Itemstack?.Collectible?.Tool == EnumTool.Shears && OvergrowthConfig.Current.useShearsForClipping))
            {

                if (clipping is null){
                    throw new ArgumentNullException(nameof(clipping), "OvergrowthBerryBush clipping is Null. Exiting.");  
                }

                if(world.BlockAccessor.GetBlockEntity(blockSel.Position) is BEOvergrowthBerryBush beogbush && !beogbush.Pruned)
                {
                    beogbush.Prune();

                    if(world.BlockAccessor.GetBlock(blockSel.Position).Variant["state"] == "flowering")
                    {
                        clipping = new ItemStack(api.World.GetItem(AssetLocation.Create("overgrowth:clipping-" + this.Variant["type"] + "-green")), 1);

                        if (byPlayer?.InventoryManager.TryGiveItemstack(clipping) == false)
                        {
                            world.SpawnItemEntity(clipping, byPlayer.Entity.SidedPos.XYZ);
                        }
                    }

                    world.PlaySoundAt(harvestingSound, blockSel.Position.X, blockSel.Position.Y, blockSel.Position.Z, byPlayer);

                    return true;
                } 
            }

            return base.OnBlockInteractStart(world, byPlayer, blockSel); 
        }

        public override bool CanPlantStay(IBlockAccessor blockAccessor, BlockPos pos)
        {
            Block belowBlock = blockAccessor.GetBlock(pos.DownCopy());
            if (belowBlock.Fertility > 0) return true;
            if (!(belowBlock is OvergrowthBerryBush)) return false;

            Block belowbelowBlock = blockAccessor.GetBlock(pos.DownCopy(2));
            return belowbelowBlock.Fertility > 0 && this.Attributes?.IsTrue("stackable") == true && belowBlock.Attributes?.IsTrue("stackable") == true;
        }


        public override ItemStack[] GetDrops(IWorldAccessor world, BlockPos pos, IPlayer byPlayer, float dropQuantityMultiplier = 1)
        {
            if (this.Variant["state"] == "ripe")
            {
                ItemStack[] drops = base.GetDrops(world, pos, byPlayer, dropQuantityMultiplier);
			    foreach (ItemStack drop in drops)
			    {
				    if (!(drop.Collectible is OvergrowthBerryBush))
				    {
					    float dropRate = 1f;
					    JsonObject attributes = this.Attributes;
					    if (attributes != null && attributes.IsTrue("forageStatAffected"))
					    {
						    dropRate *= byPlayer.Entity.Stats.GetBlended("forageDropRate");
					    }
					    drop.StackSize = GameMath.RoundRandom(this.api.World.Rand, (float)drop.StackSize * dropRate);
				    }
			    }
			    return drops;
            }
            else
            {
                return null;
            }
        }
    }
}
