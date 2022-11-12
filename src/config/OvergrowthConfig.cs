using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Common;

namespace overgrowth.config
{
    class OvergrowthConfig 
    {
        public bool useKnifeForClipping = true;
        public bool useShearsForClipping = true;
        public bool berryBushCanDamage = true;
        public float berryBushDamage = 0.5f;
        public float berryBushDamageTick = 0.7f;
        public string[] berryBushWillDamage = new string[]{"game:wolf", "game:bear", "game:drifter", "game:player"};

        public OvergrowthConfig()
        {}

        public static OvergrowthConfig Current { get; set; }

        public static OvergrowthConfig GetDefault()
        {
            OvergrowthConfig defaultConfig = new();

            defaultConfig.useKnifeForClipping = true;
            defaultConfig.useShearsForClipping = true;
            defaultConfig.berryBushCanDamage = true;
            defaultConfig.berryBushDamage = 0.5f;
            defaultConfig.berryBushDamageTick = 0.7f;
            defaultConfig.berryBushWillDamage = new string[]{"game:wolf", "game:bear", "game:drifter", "game:player"};

            return defaultConfig;
        }
    }
}