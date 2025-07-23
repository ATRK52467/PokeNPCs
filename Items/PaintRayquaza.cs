using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace PokeNPCS.Items
{
    public class PaintRayquazaItem : ModItem
    {

        public override string Texture => "PokeNPCS/Sprites/Paintings/Rayquaza";
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.maxStack = 99;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.value = 100;
            Item.createTile = ModContent.TileType<Tiles.PaintRayquazaTile>();
        }
    }
}
