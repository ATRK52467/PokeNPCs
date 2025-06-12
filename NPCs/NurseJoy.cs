using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.GameContent.Bestiary;
using Terraria.ModLoader.Utilities;
using Terraria.Localization;

namespace PokeNPCS.NPCs
{
    public class NurseJoy : ModNPC
    {
        private int specialFrameStage = 0; //Gestos especiales 0 = ninguno, 1 = sprite 6, 2 = sprite 7
        private int talkTimer = 0;
        private bool talkForward = true; // Dirección de la animación
        private int idleTimer = 0;
        private int specialIdleFrameTimer = 0;
        private int specialIdleFrame = -1; // -1 = no hay animación especial activa

        public override string Texture => "PokeNPCS/Sprites/NPCs/NurseJoy"; // Ruta del sprite de Nurse Joy
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
            NPC.homeless = false; // No es un NPC sin hogar
            NPC.townNPC = true;
            NPC.friendly = true;
            NPC.width = 18;
            NPC.height = 40;
            NPC.aiStyle = 7;
            NPC.damage = 10;
            NPC.defense = 15;
            NPC.lifeMax = 250;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f;
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
            Main.npcFrameCount[NPC.type] = 12; // Número de frames de animación
        }

        //Ataque del NPC
        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            damage = 25;       // Daño del ataque
            knockback = 2f;    // Retroceso
        }
        public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
        {
            projType = ProjectileID.CrimsonHeart;
            attackDelay = 1; // Tiempo entre ataques
        }


        //Mensajes iniciales de bienvenida
        public override string GetChat()
        {
            string[] dialogues = new string[]
            {
                "¿Necesitas ayuda con tus Pokémon?",
                "¡Cuidar a los demás es mi especialidad!",
                "Una buena salud es lo más importante.",
                "¡Bienvenido a mi pequeña clínica!",
                "¿Has visto a mis hermanas en otras ciudades?",
                "Siempre estoy aquí para ayudarte."
            };

            return dialogues[Main.rand.Next(dialogues.Length)];
        }

        public override bool CanTownNPCSpawn(int numTownNPCs)
        {
            return true; // Siempre puede aparecer si hay espacio(de momento)
        }

        //Bestiario
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement("Una amable enfermera dedicada a cuidar Pokémon y aventureros por igual.")
            });
        }

        public override void AddShops()
        {
            var npcShop = new NPCShop(Type, "Default")
                .Add(ItemID.HealingPotion); // Por ahora

            npcShop.Register();
        }

        public override void FindFrame(int frameHeight)
        {
            //Gesto especial
            if (specialIdleFrame >= 0)
            {
                specialIdleFrameTimer--;

                if (specialIdleFrameTimer <= 0)
                {
                    if (specialFrameStage == 1)
                    {
                        // Cambiar al segundo frame (7)
                        specialIdleFrame = 6;
                        specialIdleFrameTimer = 30; // otros 0.5 seg
                        specialFrameStage = 2;
                        NPC.frame.Y = frameHeight * specialIdleFrame;
                    }
                    else
                    {
                        // Terminar la animación
                        specialIdleFrame = -1;
                        specialFrameStage = 0;
                        NPC.frame.Y = 0;
                    }
                }
            }

            //Iconos de bestiario
            if (NPC.IsABestiaryIconDummy)
            {
                NPC.frameCounter++;

                if (NPC.frameCounter < 30)
                {
                    NPC.frame.Y = 0; // Frame idle
                }
                else if (NPC.frameCounter < 60)
                {
                    NPC.frame.Y = frameHeight * 7; // Frame guiño
                }
                else
                {
                    NPC.frameCounter = 0; // Reiniciar ciclo
                }

                return;
            }

            if (talkTimer > 0 && NPC.velocity.X == 0)
            {
                talkTimer--;

                // Animación de hablar (frames 1 a 5)
                NPC.frameCounter++;
                if (NPC.frameCounter >= 5)
                {
                    NPC.frameCounter = 0;

                    if (talkForward)
                    {
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y >= frameHeight * 5)
                        {
                            talkForward = false;
                        }
                    }
                    else
                    {
                        NPC.frame.Y -= frameHeight;
                        if (NPC.frame.Y <= frameHeight)
                        {
                            talkForward = true;
                        }
                    }
                }
            }

            else if (NPC.velocity.X != 0)
            {
                NPC.frameCounter++;

                if (NPC.direction == -1)
                {
                    // Caminar izquierda: sprites 9 y 10
                    if (NPC.frameCounter >= 10)
                    {
                        NPC.frameCounter = 0;

                        if (NPC.frame.Y < frameHeight * 8 || NPC.frame.Y > frameHeight * 9)
                        {
                            NPC.frame.Y = frameHeight * 8;
                        }
                        else
                        {
                            NPC.frame.Y += frameHeight;
                            if (NPC.frame.Y > frameHeight * 9)
                                NPC.frame.Y = frameHeight * 8;
                        }
                    }
                }
                else
                {
                    // Caminar derecha: sprites 11 y 12
                    if (NPC.frameCounter >= 10)
                    {
                        NPC.frameCounter = 0;

                        if (NPC.frame.Y < frameHeight * 10 || NPC.frame.Y > frameHeight * 11)
                        {
                            NPC.frame.Y = frameHeight * 10;
                        }
                        else
                        {
                            NPC.frame.Y += frameHeight;
                            if (NPC.frame.Y > frameHeight * 11)
                                NPC.frame.Y = frameHeight * 10;
                        }
                    }
                }
            }

            else
            {
                idleTimer++;
                // Si venía caminando o hablando, resetear a idle
                if (NPC.frame.Y >= frameHeight * 10 || NPC.frame.Y <= frameHeight * 8)
                {
                    NPC.frame.Y = 0;
                }

                if (specialIdleFrame >= 0)
                {
                    // Mostrar frame especial (mirar, guiñar, etc.)
                    NPC.frame.Y = frameHeight * specialIdleFrame;
                    specialIdleFrameTimer--;

                    if (specialIdleFrameTimer <= 0)
                    {
                        specialIdleFrame = -1;
                        NPC.frame.Y = 0; // volver al idle base
                    }
                }
                else
                {
                    // Alternancia entre idle (0) y respiración (5)
                    NPC.frameCounter++;
                    if (NPC.frameCounter >= 60)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y = NPC.frame.Y == 0 ? frameHeight * 5 : 0;
                    }

                    // Solo permitir gesto especial si NO camina NI habla
                    if (idleTimer >= 120 && NPC.velocity.X == 0 && talkTimer <= 0)
                    {
                        idleTimer = 0;
                        int chance = Main.rand.Next(10);

                        // 20% de probabilidad de mirar a los lados
                        if (chance < 2)
                        {
                            specialIdleFrame = 5;
                            specialIdleFrameTimer = 30; // 0.5 segundos(30 ticks)
                            specialFrameStage = 1;
                            NPC.frame.Y = frameHeight * specialIdleFrame;
                        }
                        // 30% de probabilidad de guiñar
                        else if (chance > 3 && chance < 6)
                        {
                            int[] specialFrames = new int[] { 7 };
                            specialIdleFrame = specialFrames[Main.rand.Next(specialFrames.Length)];
                            specialIdleFrameTimer = 60;
                            NPC.frame.Y = frameHeight * specialIdleFrame;
                        }
                    }
                }

            }

        }
        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = "Ver tienda"; // Botón 1
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            if (firstButton)
            {
                shopName = "Default"; // Nombre de la tienda registrada en AddShops
                talkTimer = 60; // Esto activa la animación de hablar durante 1 segundo
            }
        }


    }
}
