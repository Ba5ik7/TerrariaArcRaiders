using Terraria.WorldBuilding;
using TerrariaArcRaiders.Core.WorldGen;

namespace TerrariaArcRaiders.Adapters.WorldGen
{
    internal interface IArcWorldGenPass
    {
        ArcWorldGenStage Stage { get; }
        GenPass AsGenPass();
    }
}
