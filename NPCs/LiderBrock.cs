using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace PokeNPCS.NPCs
{
    public class LiderBrock : ModNPC
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
            button = "Batallar";
        }

        //Método para mensaje del chat al hacer clic en el botón
        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            if (firstButton)
            {
                Main.NewText("¡Prepárate para enfrentarme!", 255, 240, 20); //Mensaje amarillo
            }
        }
    }
}
