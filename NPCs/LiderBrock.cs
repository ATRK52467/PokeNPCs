using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace PokeNPCS.NPCs
{
    public class LiderBrock : TownNPC
    {
        public override string Texture => "PokeNPCS/Sprites/NPCs/LiderBrock";
        public override void SetDefaults()
        {
            NPC.width = 18;
            NPC.height = 40;
            NPC.aiStyle = 7; // Estilo aldeano
            NPC.friendly = true;
            NPC.townNPC = true;
            NPC.lifeMax = 250;
            NPC.defense = 15;
            NPC.noGravity = false;
        }

        public override string GetChat()
        {
            return "Soy Brock, el líder de gimnasio de Ciudad Plateada. ¿Vienes por una batalla?";
        }
        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = "Batalla";
            button2 = "Tienda";
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            if (firstButton)
            {
                Main.NewText("¡Ve, Charmander!", 255, 100, 20);
            }
            else
            {
                shopName = "Default";
            }
        }

        public override void AddShops()
        {
            var shop = new NPCShop(Type, "Default");

            // Tus items normales
            shop.Add(ItemID.LesserHealingPotion);

            // Condicional Hardmode
            shop.Add(ItemID.GreaterHealingPotion, Condition.Hardmode);

            // Intenta cargar Pokemod
            if (ModLoader.TryGetMod("Pokemod", out Mod pokemod))
            {
                if (pokemod.TryFind("PokeballItem", out ModItem pokeBall))
                {
                    shop.Add(pokeBall.Type);
                }

                if (pokemod.TryFind("UltraBall", out ModItem ultraBall))
                {
                    shop.Add(ultraBall.Type, Condition.Hardmode);
                }
            }
            else
            {
                Main.NewText("Pokemod no está cargado, no se pueden vender Poké Balls.", 255, 0, 0);
            }

            shop.Register();
        }


    }
}