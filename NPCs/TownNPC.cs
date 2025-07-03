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

        public void CreateShop(int npcType, string shopName, int[] items, int[] itemsHardmode)
        {
            var shop = new NPCShop(npcType, shopName);

            foreach (int item in items)
                shop.Add(item);

            //Item(s) Hardmode
            foreach (int item in itemsHardmode)
                shop.Add(item, Condition.Hardmode);

            shop.Register();
        }

    }
}