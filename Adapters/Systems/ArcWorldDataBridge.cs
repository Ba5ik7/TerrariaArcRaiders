#if !BUILD_TESTS
using Terraria.ModLoader.IO;
#endif
using System;
using TerrariaArcRaiders.Core.Services;
using TerrariaArcRaiders.Core.WorldGen;

namespace TerrariaArcRaiders.Adapters.Systems
{
    internal static class ArcWorldDataBridge
    {
        private const string KeyIsArc = "isArc";
        private const string KeyDataVersion = "dataVersion";
        private const string KeySafeHub = "safeHub";
        private const string KeyRegions = "regions";
        private const string KeyReservedSites = "reservedSites";

        private const string KeyX = "x";
        private const string KeyY = "y";
        private const string KeyWidth = "width";
        private const string KeyHeight = "height";

        private const string KeySiteKind = "kind";
        private const string KeyRadius = "radius";

        internal static TagCompoundDto ToTagCompoundDto(ArcWorldData data)
        {
            if (data == null)
            {
                return NonArcDto();
            }

            if (!data.IsArcWorld || !data.SafeHubRegion.IsValid)
            {
                return NonArcDto();
            }

            var dto = new TagCompoundDto();
            dto.SetInt(KeyIsArc, 1);
            dto.SetInt(KeyDataVersion, data.DataVersion);
            dto.SetCompound(KeySafeHub, ToRectCompound(data.SafeHubRegion));

            if (data.Regions.Count > 0)
            {
                var regionsCompound = new TagCompoundDto();
                foreach (var pair in data.Regions)
                {
                    if (!pair.Value.IsValid)
                    {
                        continue;
                    }

                    regionsCompound.SetCompound(pair.Key.ToString(), ToRectCompound(pair.Value));
                }

                if (regionsCompound.Compounds.Count > 0)
                {
                    dto.SetCompound(KeyRegions, regionsCompound);
                }
            }

            if (data.ReservedSites.Count > 0)
            {
                var sitesCompound = new TagCompoundDto();
                for (var i = 0; i < data.ReservedSites.Count; i++)
                {
                    var site = data.ReservedSites[i];
                    sitesCompound.SetCompound(i.ToString(), ToSiteCompound(site));
                }

                if (sitesCompound.Compounds.Count > 0)
                {
                    dto.SetCompound(KeyReservedSites, sitesCompound);
                }
            }

            return dto;
        }

        internal static ArcWorldData FromTagCompoundDto(TagCompoundDto dto)
        {
            if (dto == null)
            {
                return ArcWorldData.NonArc();
            }

            var isArcFlag = dto.GetInt(KeyIsArc);
            if (isArcFlag != 1)
            {
                return ArcWorldData.NonArc();
            }

            if (!TryParseRect(dto.GetCompound(KeySafeHub), out var safeHub) || !safeHub.IsValid)
            {
                return ArcWorldData.NonArc();
            }

            var data = new ArcWorldData
            {
                IsArcWorld = true,
                DataVersion = dto.GetInt(KeyDataVersion, ArcWorldData.CurrentDataVersion),
                SafeHubRegion = safeHub,
            };

            var regionsCompound = dto.GetCompound(KeyRegions);
            if (regionsCompound != null)
            {
                foreach (var pair in regionsCompound.Compounds)
                {
                    if (!Enum.TryParse(pair.Key, ignoreCase: true, out ArcRegionId regionId))
                    {
                        continue;
                    }

                    if (TryParseRect(pair.Value, out var regionRect) && regionRect.IsValid)
                    {
                        data.Regions[regionId] = regionRect;
                    }
                }
            }

            var sitesCompound = dto.GetCompound(KeyReservedSites);
            if (sitesCompound != null)
            {
                foreach (var pair in sitesCompound.Compounds)
                {
                    if (TryParseSite(pair.Value, out var site))
                    {
                        data.ReservedSites.Add(site);
                    }
                }
            }

            return data;
        }

#if !BUILD_TESTS
        internal static TagCompound ToTagCompound(ArcWorldData data)
        {
            var dto = ToTagCompoundDto(data);
            return TagCompoundBridge.ToTagCompound(dto);
        }

        internal static ArcWorldData FromTagCompound(TagCompound tag)
        {
            var dto = TagCompoundBridge.FromTagCompound(tag);
            return FromTagCompoundDto(dto);
        }
#endif

        private static TagCompoundDto NonArcDto()
        {
            var dto = new TagCompoundDto();
            dto.SetInt(KeyIsArc, 0);
            dto.SetInt(KeyDataVersion, 0);
            return dto;
        }

        private static TagCompoundDto ToRectCompound(IntRect rect)
        {
            var dto = new TagCompoundDto();
            dto.SetInt(KeyX, rect.X);
            dto.SetInt(KeyY, rect.Y);
            dto.SetInt(KeyWidth, rect.Width);
            dto.SetInt(KeyHeight, rect.Height);
            return dto;
        }

        private static TagCompoundDto ToSiteCompound(ArcReservedSite site)
        {
            var dto = new TagCompoundDto();
            dto.SetString(KeySiteKind, site.Kind.ToString());
            dto.SetInt(KeyX, site.X);
            dto.SetInt(KeyY, site.Y);
            dto.SetInt(KeyRadius, site.Radius);
            return dto;
        }

        private static bool TryParseRect(TagCompoundDto rectDto, out IntRect rect)
        {
            rect = default;
            if (rectDto == null)
            {
                return false;
            }

            var x = rectDto.GetInt(KeyX, int.MinValue);
            var y = rectDto.GetInt(KeyY, int.MinValue);
            var width = rectDto.GetInt(KeyWidth, 0);
            var height = rectDto.GetInt(KeyHeight, 0);

            if (x == int.MinValue || y == int.MinValue)
            {
                return false;
            }

            rect = new IntRect(x, y, width, height);
            return rect.IsValid;
        }

        private static bool TryParseSite(TagCompoundDto siteDto, out ArcReservedSite site)
        {
            site = null;
            if (siteDto == null)
            {
                return false;
            }

            var kindText = siteDto.GetString(KeySiteKind);
            if (string.IsNullOrWhiteSpace(kindText) || !Enum.TryParse(kindText, ignoreCase: true, out ArcReservedSiteKind kind) || kind == ArcReservedSiteKind.Unknown)
            {
                return false;
            }

            var x = siteDto.GetInt(KeyX, int.MinValue);
            var y = siteDto.GetInt(KeyY, int.MinValue);
            if (x == int.MinValue || y == int.MinValue)
            {
                return false;
            }

            var radius = siteDto.GetInt(KeyRadius, 0);
            site = new ArcReservedSite(kind, x, y, radius);
            return true;
        }
    }
}
