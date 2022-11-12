using System.Collections.Generic;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;
using Vintagestory.GameContent;
using System.Linq;
using Vintagestory.API.Common.Entities;
using overgrowth.config;
using overgrowth;

namespace overgrowth
{
    public class GroundBerryPlant : BlockPlant
    {
        public override bool CanPlantStay(IBlockAccessor blockAccessor, BlockPos pos)
        {
            Block belowBlock = blockAccessor.GetBlock(pos.DownCopy());
            if (belowBlock.Fertility > 0) return true;
            else return false;
        }

        public override ItemStack[] GetDrops(IWorldAccessor world, BlockPos pos, IPlayer byPlayer, float dropQuantityMultiplier = 1)
        {
            if (this.Variant["state"] == "ripe")
            {
                ItemStack[] drops = base.GetDrops(world, pos, byPlayer, dropQuantityMultiplier);
                return drops;
            }
            else
            {
                return null;
            }
        }
    }
}