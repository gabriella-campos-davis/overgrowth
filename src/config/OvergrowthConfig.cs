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

        public OvergrowthConfig()
        {}

        public static OvergrowthConfig Current { get; set; }

        public static OvergrowthConfig GetDefault()
        {
            OvergrowthConfig defaultConfig = new();

            defaultConfig.useKnifeForClipping = true;
            defaultConfig.useShearsForClipping = true;

            return defaultConfig;
        }
    }
}