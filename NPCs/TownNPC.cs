using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace PokeNPCS.NPCs
{
    public class TownNPC : ModNPC
    {
        public override string Texture => "PokeNPCS/Sprites/NPCs/NurseJoy"; // Ruta default

        //Comportamiento del NPC
        public override void AI()
        {

        }

        public void CreateShop(
    int npcType,
    string shopName,
    int[] items,
    Dictionary<int, int> precios,
    int[] itemsHardmode,
    Dictionary<int, int> preciosHardmode
)
        {
            var shop = new NPCShop(npcType, shopName);

            // Items Pre-Hardmode
            foreach (int item in items)
            {
                if (precios != null && precios.TryGetValue(item, out int price))
                {
                    // Asigna el precio al Item
                    ItemLoader.GetItem(item).Item.value = price;
                }

                shop.Add(item);
            }

            // Items Hardmode
            foreach (int item in itemsHardmode)
            {
                if (preciosHardmode != null && preciosHardmode.TryGetValue(item, out int price))
                {
                    ItemLoader.GetItem(item).Item.value = price;
                }

                shop.Add(item, Condition.Hardmode);
            }

            shop.Register();
        }


    }
}