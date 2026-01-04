using Terraria.ModLoader.IO;
using TerrariaArcRaiders.Core.Services;

namespace TerrariaArcRaiders.Adapters.Systems
{
    internal static class TagCompoundBridge
    {
        public static TagCompound ToTagCompound(TagCompoundDto dto)
        {
            var tag = new TagCompound();
            foreach (var pair in dto.Ints)
            {
                tag[pair.Key] = pair.Value;
            }

            foreach (var pair in dto.Strings)
            {
                tag[pair.Key] = pair.Value;
            }

            foreach (var pair in dto.Compounds)
            {
                tag[pair.Key] = ToTagCompound(pair.Value);
            }

            return tag;
        }

        public static TagCompoundDto FromTagCompound(TagCompound tag)
        {
            var dto = new TagCompoundDto();

            foreach (var pair in tag)
            {
                var key = pair.Key;
                var value = pair.Value;
                if (value is int i)
                {
                    dto.SetInt(key, i);
                }
                else if (value is string s)
                {
                    dto.SetString(key, s);
                }
                else if (value is TagCompound nested)
                {
                    dto.SetCompound(key, FromTagCompound(nested));
                }
                // ignore other types for forward compatibility
            }

            return dto;
        }
    }
}
