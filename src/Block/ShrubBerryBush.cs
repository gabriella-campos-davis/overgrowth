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
    public class ShrubBerryBush : OvergrowthBerryBush
    {
        MeshData[] prunedmeshes;

        new public MeshData GetPrunedMesh(BlockPos pos)
        {
            if (api == null) return null;
            if (prunedmeshes == null) genPrunedMeshes();

            int rnd = RandomizeAxes == EnumRandomizeAxes.XYZ ? GameMath.MurmurHash3Mod(pos.X, pos.Y, pos.Z, prunedmeshes.Length) : GameMath.MurmurHash3Mod(pos.X, 0, pos.Z, prunedmeshes.Length);

            return prunedmeshes[rnd];
        }

        private void genPrunedMeshes()
        {
            var capi = api as ICoreClientAPI;

            prunedmeshes = new MeshData[Shape.BakedAlternates.Length];

            var selems = new string[] { "Stem1/Leaves", "Stem1/Leaves2", "Stem3/Leaves5", "Stem3/Leaves6", "Stem1/Berries", "Stem1/Berries2", "Stem3/Berries5", "Stem3/Berries6"};
            if (State == "empty")
            {
                selems = selems.Remove("Stem1/Berries");
                selems = selems.Remove("Stem1/Berries2");
                selems = selems.Remove("Stem3/Berries5");
                selems = selems.Remove("Stem3/Berries6");
            } 

            for (int i = 0; i < Shape.BakedAlternates.Length; i++)
            {
                var cshape = Shape.BakedAlternates[i];
                var shape = capi.TesselatorManager.GetCachedShape(cshape.Base);
                capi.Tesselator.TesselateShape(this, shape, out prunedmeshes[i], this.Shape.RotateXYZCopy, null, selems);
            }
        }
    }
}