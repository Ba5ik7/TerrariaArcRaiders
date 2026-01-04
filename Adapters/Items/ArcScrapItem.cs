using Terraria.ID;
using Terraria.ModLoader;
using TerrariaArcRaiders.Core.Models;

namespace TerrariaArcRaiders.Adapters.Items
{
    public class ArcScrapItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = ArcScrap.MaxStack;
            Item.value = ArcScrap.DefaultValue;
            Item.rare = ItemRarityID.White;
        }
    }
}
