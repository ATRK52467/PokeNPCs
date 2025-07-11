using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using PokeNPCS.Events;

namespace PokeNPCS.NPCs
{
    public class Spyral : ModNPC
    {
        public static int HeadIndex;

        public override string Texture => "PokeNPCS/Sprites/NPCs/Spyral";
        private bool fotoTomadaHoy = false;
        private int fotoCooldown = 0;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 15;
            NPCID.Sets.DangerDetectRange[NPC.type] = 300;
            NPCID.Sets.HatOffsetY[NPC.type] = 4;

            // ✅ Clave para comportamiento TownPet
            NPCID.Sets.IsTownPet[NPC.type] = true;
            NPCID.Sets.NoTownNPCHappiness[NPC.type] = true;

        }

        public override void SetDefaults()
        {
            NPC.friendly = true;
            NPC.width = 36;
            NPC.height = 38;
            NPC.aiStyle = 7;
            NPC.damage = 0;
            NPC.defense = 15;
            NPC.lifeMax = 250;
            NPC.knockBackResist = 0.5f;
            NPC.townNPC = true;
            NPC.homeless = false;
            NPC.noTileCollide = false;
            NPC.housingCategory = -1;

        }
        public override bool CanChat()
        {
            return true;
        }

        public override bool CanTownNPCSpawn(int numTownNPCs)
        {
            return RocketInvasion.HasBeatenRocketInvasion;
        }

        public override string GetChat()
        {
            string[] chats = {
                "*Se frota contra ti*",
                "*Le toma una foto*",
                "¿Tienes comida?"
            };
            return chats[Main.rand.Next(chats.Length)];
        }

        public override void FindFrame(int frameHeight)
        {
            if (fotoCooldown > 0)
            {
                // Mostrar frame especial de la foto mientras dura el cooldown
                NPC.frame.Y = 14 * frameHeight;
                fotoCooldown--;
                return;
            }

            // Animación normal
            NPC.frameCounter += 0.1;
            if (NPC.frameCounter >= Main.npcFrameCount[NPC.type] - 1)
            {
                NPC.frameCounter = 0;
            }

            NPC.frame.Y = (int)NPC.frameCounter * frameHeight;
        }

        public override void AI()
        {
            if (Main.dayTime && (int)Main.time == 6000 && !fotoTomadaHoy)
            {
                // Activar el cooldown para el sprite de la foto
                fotoCooldown = 60; // 2 segundos a 60 fps

                // Mostrar mensaje solo una vez
                if (Main.netMode != NetmodeID.Server)
                {
                    Main.NewText("¡Foto tomada! Enviando reporte a Giovanni...", 175, 75, 255);
                }
                //Falta sonido de foto

                fotoTomadaHoy = true;
                NPC.netUpdate = true;
            }

            // Reiniciar al siguiente día
            if (!Main.dayTime && fotoTomadaHoy)
            {
                fotoTomadaHoy = false;
            }
        }


        public override bool CanGoToStatue(bool toKingStatue) => false;

        // ✅ Permitirle aceptar cualquier casa
        public override bool CheckConditions(int left, int right, int top, int bottom)
        {
            return true;
        }
    }
}
