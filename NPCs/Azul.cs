using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.GameContent.Bestiary;
using System;
using System.Collections.Generic;

namespace PokeNPCS.NPCs
{
    public class Azul : ModNPC
    {
        public override string Texture => "PokeNPCS/Sprites/NPCs/Azul";

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
    }
}