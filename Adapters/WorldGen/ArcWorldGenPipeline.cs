using System.Collections.Generic;
using System.Linq;
using Terraria.WorldBuilding;
using TerrariaArcRaiders.Adapters.WorldGen.Passes;
using TerrariaArcRaiders.Core.WorldGen;

namespace TerrariaArcRaiders.Adapters.WorldGen
{
    internal sealed class ArcWorldGenPipeline
    {
        private readonly Dictionary<ArcWorldGenStage, List<IArcWorldGenPass>> _passesByStage = new();

        public static ArcWorldGenPipeline CreateWithDefaultPasses()
        {
            var pipeline = new ArcWorldGenPipeline();
            pipeline.Register(new ArcStageA_Setup());
            pipeline.Register(new ArcStageB_BaseTerrain());
            pipeline.Register(new ArcStageC_RegionPlanning());
            pipeline.Register(new ArcStageD_BiomePainting());
#if DEBUG
            pipeline.Register(new ArcStageZ_TestMarker());
#endif
            pipeline.Register(new ArcStageE_StructureReservation());
            pipeline.Register(new ArcStageF_StructurePlacement());
            pipeline.Register(new ArcStageG_RaidAnchors());
            pipeline.Register(new ArcStageH_FinalValidation());
            return pipeline;
        }

        public void Register(IArcWorldGenPass pass)
        {
            if (pass == null)
            {
                return;
            }

            if (!_passesByStage.TryGetValue(pass.Stage, out var list))
            {
                list = new List<IArcWorldGenPass>();
                _passesByStage[pass.Stage] = list;
            }

            list.Add(pass);
        }

        public void InsertBefore(ArcWorldGenStage stage, IArcWorldGenPass pass)
        {
            if (pass == null)
            {
                return;
            }

            if (!_passesByStage.TryGetValue(stage, out var list))
            {
                list = new List<IArcWorldGenPass>();
                _passesByStage[stage] = list;
            }

            list.Insert(0, pass);
        }

        public List<GenPass> BuildOrderedPasses()
        {
            var orderedStages = _passesByStage.Keys.OrderBy(s => s).ToList();
            var result = new List<GenPass>();

            foreach (var stage in orderedStages)
            {
                foreach (var pass in _passesByStage[stage])
                {
                    var genPass = pass.AsGenPass();
                    if (genPass != null)
                    {
                        result.Add(genPass);
                    }
                }
            }

            return result;
        }
    }
}
