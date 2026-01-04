#nullable enable

using System.Collections.Generic;

namespace TerrariaArcRaiders.Core.Services
{
    // Lightweight, tModLoader-free DTO to represent TagCompound-like data.
    public sealed class TagCompoundDto
    {
        private readonly Dictionary<string, int> _ints = new();
        private readonly Dictionary<string, string> _strings = new();
        private readonly Dictionary<string, TagCompoundDto> _compounds = new();

        public void SetInt(string key, int value) => _ints[key] = value;
        public int GetInt(string key, int defaultValue = 0) => _ints.TryGetValue(key, out var value) ? value : defaultValue;

        public void SetString(string key, string value) => _strings[key] = value;
        public string? GetString(string key) => _strings.TryGetValue(key, out var value) ? value : null;

        public void SetCompound(string key, TagCompoundDto compound) => _compounds[key] = compound;
        public TagCompoundDto? GetCompound(string key) => _compounds.TryGetValue(key, out var value) ? value : null;
    }
}
