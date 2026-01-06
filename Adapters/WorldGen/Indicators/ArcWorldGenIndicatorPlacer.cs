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
            // - NxN solid block base at (x,y) .. (x+N-1,y+N-1)
            // - glass "frame" perimeter around the base, padded by P tiles
            // - torch above the base (centered)
            var x = placement.TileX;
            var y = placement.TileY;

            if (!TryClampMarkerPositionToHub(hubRegion, ref x, ref y))
            {
                return false;
            }

            var baseTileType = GetMarkerBaseTileType(placement.Stage);

            var placedFrame = TryPlaceFramePerimeter(x, y);
            var placedBase = TryPlaceSolidBase(x, y, baseTileType);
            var placedTorch = TryPlaceTorch(x + (ArcWorldGenIndicatorConstants.BaseSizeTiles / 2), y - 1);

            if (placedFrame || placedBase || placedTorch)
            {
                FrameArea(x, y);
                return true;
            }

            return false;
        }

        private static bool TryClampMarkerPositionToHub(IntRect hubRegion, ref int x, ref int y)
        {
            // Ensure marker footprint remains within hub bounds (frame) and in-world.
            if (!hubRegion.IsValid)
            {
                return false;
            }

            var framePad = ArcWorldGenIndicatorConstants.FramePaddingTiles;
            var baseSize = ArcWorldGenIndicatorConstants.BaseSizeTiles;

            var minBaseX = hubRegion.X + framePad;
            var maxBaseX = hubRegion.Right - baseSize - framePad;
            var minBaseY = hubRegion.Y + framePad;
            var maxBaseY = hubRegion.Bottom - baseSize - framePad;

            x = Clamp(x, minBaseX, Math.Max(minBaseX, maxBaseX));
            y = Clamp(y, minBaseY, Math.Max(minBaseY, maxBaseY));

            var frameLeft = x - framePad;
            var frameTop = y - framePad;
            var frameRight = x + (baseSize - 1) + framePad;
            var frameBottom = y + (baseSize - 1) + framePad;

            // The torch is inside the frame area (y-1), so checking the frame corners is sufficient.
            return TWorldGen.InWorld(frameLeft, frameTop, 10) && TWorldGen.InWorld(frameRight, frameBottom, 10);
        }

        private static bool TryPlaceSolidBase(int x, int y, ushort tileType)
        {
            var anyPlaced = false;
            var size = ArcWorldGenIndicatorConstants.BaseSizeTiles;
            // Bounded: always NxN tiles.
            for (var dx = 0; dx < size; dx++)
            {
                for (var dy = 0; dy < size; dy++)
                {
                    anyPlaced |= TryPlaceTileIfEmpty(x + dx, y + dy, tileType);
                }
            }

            return anyPlaced;
        }

        private static bool TryPlaceFramePerimeter(int baseX, int baseY)
        {
            var pad = ArcWorldGenIndicatorConstants.FramePaddingTiles;
            var size = ArcWorldGenIndicatorConstants.BaseSizeTiles;
            // Bounded: perimeter tiles for a fixed-size square.
            var anyPlaced = false;

            // Use a vanilla tile type so worlds remain loadable if the mod is disabled/removed.
            const ushort frameTileType = TileID.Glass;

            var left = -pad;
            var right = (size - 1) + pad;
            var top = -pad;
            var bottom = (size - 1) + pad;

            for (var dx = left; dx <= right; dx++)
            {
                for (var dy = top; dy <= bottom; dy++)
                {
                    var isPerimeter = dx == left || dx == right || dy == top || dy == bottom;
                    if (!isPerimeter)
                    {
                        continue;
                    }

                    anyPlaced |= TryPlaceTileIfEmpty(baseX + dx, baseY + dy, frameTileType);
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
            var pad = ArcWorldGenIndicatorConstants.FramePaddingTiles;
            var size = ArcWorldGenIndicatorConstants.BaseSizeTiles;
            // Bounded: constant framing area around the marker footprint.
            var left = -pad - 1;
            var right = (size - 1) + pad + 1;
            var top = -pad - 1;
            var bottom = (size - 1) + pad + 1;

            for (var dx = left; dx <= right; dx++)
            {
                for (var dy = top; dy <= bottom; dy++)
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
