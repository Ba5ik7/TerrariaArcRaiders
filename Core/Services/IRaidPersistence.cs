#nullable enable

using TerrariaArcRaiders.Core.Models;

namespace TerrariaArcRaiders.Core.Services
{
    public interface IRaidPersistence
    {
        TagCompoundDto Save(Stash stash, RaidSession? sessionSnapshot);

        void Load(TagCompoundDto? data, Stash stash, out RaidSession? sessionSnapshot);
    }
}
