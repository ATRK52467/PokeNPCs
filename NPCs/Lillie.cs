using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.GameContent.Bestiary;
using System;
using System.Collections.Generic;


namespace PokeNPCS.NPCs
{
    public class Lillie : TownNPC
    {
        public override string Texture => "PokeNPCS/Sprites/NPCs/Lillie";
        public static int HeadIndex;
        public override void SetStaticDefaults()
        {
            NPCID.Sets.ExtraFramesCount[Type] = 0;
            NPCID.Sets.AttackFrameCount[Type] = 1;
            NPCID.Sets.DangerDetectRange[Type] = 700;
            NPCID.Sets.HatOffsetY[Type] = 0;
        }

        public override void SetDefaults()
        {
            base.SetDefaults(); //cargar de TownNPC
            NPC.width = 18;
            NPC.height = 40;
            NPC.damage = 10;
            NPC.defense = 10;
            NPC.lifeMax = 150;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f;
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
            Main.npcFrameCount[NPC.type] = 4; // Número de frames de animación
        }

        public override void FindFrame(int frameHeight)
        {
            int secondTicker = (int)(Main.GameUpdateCount / 60) % 4; // Cambia cada 60 ticks (~1 seg)

            if (NPC.IsABestiaryIconDummy)
            {
                NPC.frameCounter++;
                if (NPC.frameCounter < 30)
                    NPC.frame.Y = 0;
                else if (NPC.frameCounter < 60)
                    NPC.frame.Y = frameHeight * 7;
                else
                    NPC.frameCounter = 0;
                return;
            }

            // Cambiar entre 4 frames (0, 1, 2, 3), uno por segundo
            NPC.frame.Y = frameHeight * secondTicker;


        }
        public override bool CanTownNPCSpawn(int numTownNPCs)
        {
            return NPC.downedBoss2; // Lillie puede aparecer después de derrotar al primer jefe
        }

        public override void AI()
        {
            // Aquí puedes agregar lógica adicional para el comportamiento de Lillie
            // Por ejemplo, puedes hacer que se mueva o interactúe con el entorno
        }


        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement("Lillie es una chica tímida y amable que adora a los Pokémon .")
            });
        }
    }
}