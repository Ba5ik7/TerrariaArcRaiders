using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using TerrariaArcRaiders.Core.WorldGen;
using TerrariaArcRaiders.Core.WorldGen.Indicators;
using TWorldGen = Terraria.WorldGen;

namespace TerrariaArcRaiders.Adapters.WorldGen.Indicators
{
    internal sealed class ArcWorldGenIndicatorPlacer
    {
        public bool TryPlaceIndicatorBoard(IntRect hubRegion, IReadOnlyList<ArcWorldGenIndicatorPlacement> placements)
        {
            if (!hubRegion.IsValid || placements == null || placements.Count == 0)
            {
                return false;
            }

            var succeededAny = false;
            // Safety/perf: bound the amount of work even if a caller passes an unexpectedly large list.
            // Each placement has a constant-size footprint (2x2 base + torch + small framing area).
            var maxPlacements = 64;
            var placementCount = Math.Min(placements.Count, maxPlacements);

            for (var i = 0; i < placementCount; i++)
            {
                succeededAny |= TryPlaceStageMarker(hubRegion, placements[i]);
            }

            return succeededAny;
        }

        private static bool TryPlaceStageMarker(IntRect hubRegion, ArcWorldGenIndicatorPlacement placement)
        {
            // Marker footprint:
            // - 2x2 solid block base at (x,y) .. (x+1,y+1)
            // - torch at (x, y-1)
            var x = placement.TileX;
            var y = placement.TileY;

            if (!TryClampMarkerPositionToHub(hubRegion, ref x, ref y))
            {
                return false;
            }

            var baseTileType = GetMarkerBaseTileType(placement.Stage);

            var placedBase = TryPlaceSolid2x2(x, y, baseTileType);
            var placedTorch = TryPlaceTorch(x, y - 1);

            if (placedBase || placedTorch)
            {
                FrameArea(x, y);
                return true;
            }

            return false;
        }

        private static bool TryClampMarkerPositionToHub(IntRect hubRegion, ref int x, ref int y)
        {
            // Ensure (x,y) .. (x+1,y+1) and (x,y-1) remain in-bounds and within hub bounds.
            if (!hubRegion.IsValid)
            {
                return false;
            }

            x = Clamp(x, hubRegion.X, Math.Max(hubRegion.X, hubRegion.Right - 2));
            y = Clamp(y, hubRegion.Y + 1, Math.Max(hubRegion.Y + 1, hubRegion.Bottom - 2));

            return TWorldGen.InWorld(x, y, 10) && TWorldGen.InWorld(x + 1, y + 1, 10) && TWorldGen.InWorld(x, y - 1, 10);
        }

        private static bool TryPlaceSolid2x2(int x, int y, ushort tileType)
        {
            var anyPlaced = false;
            // Bounded: always 4 tiles.
            for (var dx = 0; dx <= 1; dx++)
            {
                for (var dy = 0; dy <= 1; dy++)
                {
                    anyPlaced |= TryPlaceTileIfEmpty(x + dx, y + dy, tileType);
                }
            }

            return anyPlaced;
        }

        private static bool TryPlaceTorch(int x, int y)
        {
            return TryPlaceTileIfEmpty(x, y, TileID.Torches);
        }

        private static bool TryPlaceTileIfEmpty(int x, int y, ushort tileType)
        {
            if (!TWorldGen.InWorld(x, y, 10))
            {
                return false;
            }

            var tile = Framing.GetTileSafely(x, y);
            if (tile.HasTile)
            {
                return false;
            }

            try
            {
                return TWorldGen.PlaceTile(x, y, tileType, mute: true, forced: false, plr: -1, style: 0);
            }
            catch
            {
                return false;
            }
        }

        private static void FrameArea(int x, int y)
        {
            // Bounded: constant framing area around the marker (4 cols x 5 rows).
            for (var dx = -1; dx <= 2; dx++)
            {
                for (var dy = -2; dy <= 2; dy++)
                {
                    var fx = x + dx;
                    var fy = y + dy;
                    if (TWorldGen.InWorld(fx, fy, 10))
                    {
                        TWorldGen.SquareTileFrame(fx, fy);
                    }
                }
            }
        }

        private static ushort GetMarkerBaseTileType(ArcWorldGenStage stage)
        {
            // Vanilla tile types chosen to be common and stable, and distinct per stage.
            return stage switch
            {
                ArcWorldGenStage.StageA_Setup => TileID.WoodBlock,
                ArcWorldGenStage.StageB_BaseTerrain => TileID.Stone,
                ArcWorldGenStage.StageC_RegionPlanning => TileID.Sand,
                ArcWorldGenStage.StageD_BiomePainting => TileID.Mud,
                ArcWorldGenStage.StageE_StructureReservation => TileID.ClayBlock,
                ArcWorldGenStage.StageF_StructurePlacement => TileID.SnowBlock,
                ArcWorldGenStage.StageG_RaidAnchors => TileID.Ash,
                ArcWorldGenStage.StageH_FinalValidation => TileID.Obsidian,
                _ => TileID.Dirt,
            };
        }

        private static int Clamp(int value, int minInclusive, int maxInclusive)
        {
            if (value < minInclusive)
            {
                return minInclusive;
            }

            return value > maxInclusive ? maxInclusive : value;
        }
    }
}
