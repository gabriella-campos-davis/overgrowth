using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Vintagestory.API.Client;
using Vintagestory.API.MathTools;
using overgrowth.config;

[assembly: ModInfo( "Overgrowth",
	Description = "A library of useful plant code.",
	Website     = "",
	Authors     = new []{ "gabb", "CATASTEROID" } )]

namespace overgrowth
{
    public class overgrowth : ModSystem
    {
        public override bool ShouldLoad(EnumAppSide forSide)
        {
            return true;
        }

        public override void Start(ICoreAPI api)
        {
            base.Start(api);
            api.RegisterBlockClass("OvergrowthBerryBush", typeof(OvergrowthBerryBush));
            
            api.RegisterBlockEntityClass("BEOvergrowthBerryBush", typeof(BEOvergrowthBerryBush));
            api.RegisterBlockEntityClass("BETallBerryBush", typeof(BETallBerryBush));
            api.RegisterBlockEntityClass("BEClipping", typeof(BEClipping));

            api.RegisterItemClass("ItemClipping", typeof(ItemClipping));

            try
            {
                var Config = api.LoadModConfig<OvergrowthConfig>("overgrowthconfig.json");
                if (Config != null)
                {
                    api.Logger.Notification("Mod Config successfully loaded.");
                    OvergrowthConfig.Current = Config;
                }
                else
                {
                    api.Logger.Notification("No Mod Config specified. Falling back to default settings");
                    OvergrowthConfig.Current = OvergrowthConfig.GetDefault();
                }
            }
            catch
            {
                OvergrowthConfig.Current = OvergrowthConfig.GetDefault();
                api.Logger.Error("Failed to load custom mod configuration. Falling back to default settings!");
            }
            finally
            {
                if (OvergrowthConfig.Current.useKnifeForClipping == null)
                    OvergrowthConfig.Current.useKnifeForClipping = OvergrowthConfig.GetDefault().useKnifeForClipping;

                if (OvergrowthConfig.Current.useShearsForClipping == null)
                    OvergrowthConfig.Current.useShearsForClipping = OvergrowthConfig.GetDefault().useShearsForClipping;

                api.StoreModConfig(OvergrowthConfig.Current, "overgrowthconfig.json");
            }
        }

        public override void StartClientSide(ICoreClientAPI capi)
        {
            base.StartClientSide(capi);  
        }
    }
}
