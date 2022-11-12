using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;
using Vintagestory.GameContent;
using Vintagestory.API.Common.Entities;
using overgrowth.config;

namespace overgrowth
{
    public class PricklyBerryBush : OvergrowthBerryBush
    {
        public bool canDamage = OvergrowthConfig.Current.berryBushCanDamage;
        public string[] willDamage = OvergrowthConfig.Current.berryBushWillDamage;
        public float dmg = OvergrowthConfig.Current.berryBushDamage;
        public float dmgTick = OvergrowthConfig.Current.berryBushDamageTick;
        
        public override void OnEntityInside(IWorldAccessor world, Entity entity, BlockPos pos)
        {
            if(!canDamage)
            {
                return;
            }
            if(entity == null)
            {
                return;
            }

            if(willDamage == null)
            {
                return;
            }

            foreach(string creature in willDamage) 
            {
                if(entity.Code.ToString().Contains(creature))
                {
                    goto damagecreature;
                }
            }
            return;
            damagecreature:

            if (world.Side == EnumAppSide.Server && entity is EntityAgent && !(entity as EntityAgent).ServerControls.Sneak) //if the creature ins't sneaking, deal damage.
            {
                if (world.Rand.NextDouble() > dmgTick) //while standing in the bush, how often will it hurt you
                {
                    entity.ReceiveDamage(new DamageSource() 
                    { 
                        Source = EnumDamageSource.Block, 
                        SourceBlock = this, 
                        Type = EnumDamageType.PiercingAttack, 
                        SourcePos = pos.ToVec3d() 
                    }
                    , dmg); //Deal damage
                }
            }
            base.OnEntityInside(world, entity, pos);
        }
    }
}