using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

namespace overgrowth
{
    public class BEClipping : BlockEntity
    {
        double totalHoursTillGrowth;
        long growListenerId;
        public float dieBelowTemp;
        public string bushCode;
        public string bushType;
        Block block;

        
        public override void Initialize(ICoreAPI api)
        {
            base.Initialize(api);
            
            block = api.World.BlockAccessor.GetBlock(Pos);

            if(block.Attributes is null){
                return;
            } else {
                bushCode = block.Attributes["bushCode"].ToString();
            }

            if (api is ICoreServerAPI)
            {
                growListenerId = RegisterGameTickListener(CheckGrow, 2000);
            }
        }

        NatFloat nextStageDaysRnd
        {
            get
            {
                NatFloat matureDays = NatFloat.create(EnumDistribution.UNIFORM, 7f, 2f);
                if (Block?.Attributes != null)
                {
                    return Block.Attributes["matureDays"].AsObject(matureDays);
                }
                return matureDays;
            }
        }

        float GrowthRateMod => Api.World.Config.GetString("saplingGrowthRate").ToFloat(1);

        public override void OnBlockPlaced(ItemStack byItemStack)
        {
            ICoreServerAPI sapi = Api as ICoreServerAPI;

            totalHoursTillGrowth = Api.World.Calendar.TotalHours + nextStageDaysRnd.nextFloat(1, Api.World.Rand) * 24 * GrowthRateMod;
            bushType = this.Block.Variant["type"].ToString();
        }


        private void CheckGrow(float dt)
        {
            if (Api.World.Calendar.TotalHours < totalHoursTillGrowth)
                return;

            ClimateCondition conds = Api.World.BlockAccessor.GetClimateAt(Pos, EnumGetClimateMode.NowValues);
            if (conds == null)
            {
                return;
            }

            if(conds.Temperature < dieBelowTemp)
            {
                DoGrow("dead");
                return;
            }

            if (conds.Temperature < 0)
            {
                totalHoursTillGrowth = Api.World.Calendar.TotalHours + (float)Api.World.Rand.NextDouble() * 72 * GrowthRateMod;
                return;
            }

            if (conds.Temperature < 5)
            {
                return;
            }

            DoGrow("alive");
            
        }
        private void DoGrow(string state){ //this contains the worst code ever written, please fix
            ICoreServerAPI sapi = Api as ICoreServerAPI;

            if(state == "alive")
            {
                Block newBushBlock = Api.World.GetBlock(AssetLocation.Create(bushCode));
                if (newBushBlock is null){
                    throw new ArgumentNullException(nameof(newBushBlock), "BEClipping newBushBlock is Null. Exiting.");
                }
                Api.World.BlockAccessor.SetBlock(newBushBlock.BlockId, Pos);
            }

            if(state == "dead")
            {
                Block deadClippingBlock;

                if(this.Block.Code.FirstCodePart() == "growth"){
                    bushType = this.Block.Variant["type"].ToString();
                    deadClippingBlock = Api.World.GetBlock(AssetLocation.Create("overgrowth:growth-" + bushType + "-dead"));
                    if (deadClippingBlock is null){
                        throw new ArgumentNullException(nameof(deadClippingBlock), "BEClipping dead growth block is Null. Exiting.");  
                    }
                    Api.World.BlockAccessor.SetBlock(deadClippingBlock.BlockId, Pos);
                } else{
                    bushType = this.Block.Variant["type"].ToString();
                    deadClippingBlock = Api.World.GetBlock(AssetLocation.Create("overgrowth:clipping-" + bushType + "-dead"));
                    if (deadClippingBlock is null){
                        throw new ArgumentNullException(nameof(deadClippingBlock), "BEClipping dead bush block is Null. Exiting.");  
                    }
                    Api.World.BlockAccessor.SetBlock(deadClippingBlock.BlockId, Pos);
                }
            }
        }

        public override void ToTreeAttributes(ITreeAttribute tree)
        {
            base.ToTreeAttributes(tree);

            tree.SetDouble("totalHoursTillGrowth", totalHoursTillGrowth);
            tree.SetFloat("dieBelowTemp", dieBelowTemp);
        }

        public override void FromTreeAttributes(ITreeAttribute tree, IWorldAccessor worldForResolving)
        {
            base.FromTreeAttributes(tree, worldForResolving);

            totalHoursTillGrowth = tree.GetDouble("totalHoursTillGrowth", 0);
            dieBelowTemp = tree.GetFloat("dieBelowTemp", -2);
        }

        public override void GetBlockInfo(IPlayer forPlayer, StringBuilder dsc)
        {
            base.GetBlockInfo(forPlayer, dsc);

            string isAlive = this.Block.Variant["state"].ToString();
            if(isAlive == "dead"){
                string type = this.Block.Variant["type"].ToString();
                dsc.AppendLine(Lang.Get("Dead {0} clipping", type));
            }
            else 
            {
                double hoursleft = totalHoursTillGrowth - Api.World.Calendar.TotalHours;
                double daysleft = hoursleft / Api.World.Calendar.HoursPerDay;

                if (daysleft <= 1)
                {
                    dsc.AppendLine(Lang.Get("Will grow in less than a day"));
                }
                else
                {
                    dsc.AppendLine(Lang.Get("Will grow in about {0} days", (int)daysleft));
                }
            } 
        }
    }
}
