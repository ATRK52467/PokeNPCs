using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.GameContent.Bestiary;
using System;

namespace PokeNPCS.NPCs
{
    public class NurseJoy : TownNPC

    {
        private int specialFrameStage = 0; //Gestos especiales 0 = ninguno, 1 = sprite 6, 2 = sprite 7
        private int talkTimer = 0;
        private bool talkForward = true; // Dirección de la animación
        private int idleTimer = 0;
        private int specialIdleFrameTimer = 0;
        private int specialIdleFrame = -1; // -1 = no hay animación especial activa
        //Variables de caminata para Joy
        private int wanderTimer = 0;
        private int wanderDuration = 0;
        private int wanderSwitchTimer = 0;
        private int wanderDirection = 0;
        //Variables de respiración
        private int idleBreathStage = 0;
        private int idleBreathTimer = 0;

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
            NPC.homeless = false;
            NPC.townNPC = true;
            NPC.friendly = true;
            NPC.width = 18;
            NPC.height = 40;
            NPC.aiStyle = 7;
            NPC.damage = 10;
            NPC.defense = 10;
            NPC.lifeMax = 250;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f;
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
            Main.npcFrameCount[NPC.type] = 14; // Número de frames de animación
        }

        // Comportamiento de IA del NPC
        private int healCooldown = 0;
        public override void AI()
        {
            //Detener si está haciendo un gesto especial
            if (specialIdleFrame >= 0)
            {
                NPC.velocity.X = 0f;
                return;
            }

            // Detenerse al estar en conversación
            if (Main.player[Main.myPlayer].talkNPC == NPC.whoAmI)
            {
                NPC.velocity.X = 0f;
                return;
            }

            int closestPlayerIndex = -1;
            float closestDistance = float.MaxValue;
            // Buscar jugador activo y más cercano
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player p = Main.player[i];
                if (p.active && !p.dead)
                {
                    float dist = Vector2.Distance(NPC.Center, p.Center);
                    if (dist < closestDistance)
                    {
                        closestDistance = dist;
                        closestPlayerIndex = i;
                    }
                }
            }

            if (closestPlayerIndex == -1)
            {
                NPC.velocity.X = 0f;
                return;
            }

            Player player = Main.player[closestPlayerIndex];
            float maxFollowDistance = 128f; // 8 bloques
            float minFollowDistance = 80f;  // 5 bloques

            bool isInRange = closestDistance < maxFollowDistance && player.statLife < player.statLifeMax2;

            if (isInRange)
            {
                float horizontalDistance = Math.Abs(NPC.Center.X - player.Center.X);

                // Si está muy cerca, se queda quieta
                if (horizontalDistance <= minFollowDistance)
                {
                    NPC.velocity.X = 0f;
                }
                else
                {
                    // Solo se mueve en X (pegada al suelo)
                    float speed = 1.5f;
                    if (NPC.Center.X < player.Center.X)
                        NPC.velocity.X = speed;
                    else
                        NPC.velocity.X = -speed;
                }

                // Si está en el suelo y encuentra una pared, puede "saltar" como otros NPCs
                if (NPC.collideX && NPC.velocity.Y == 0f && NPC.velocity.X != 0f)
                {
                    NPC.velocity.Y = -5f; // Salta un pequeño escalón
                }

                // Lógica de curación
                if (healCooldown <= 0)
                {
                    healCooldown = 60;
                    Vector2 projVelocity = Vector2.Normalize(player.Center - NPC.Center) * 10f;

                    //Tipo de proyectil
                    int projType = ProjectileID.RubyBolt;
                    if (Main.hardMode)
                        projType = ProjectileID.AmberBolt;

                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, projVelocity, projType, 0, 0f, Main.myPlayer);

                    int healAmount = 10;
                    player.statLife += healAmount;
                    if (player.statLife > player.statLifeMax2)
                        player.statLife = player.statLifeMax2;

                    player.HealEffect(healAmount);
                }
            }
            else
            {
                NPC.velocity.X = 0f;
            }
            // Verificar si hay jugadores cercanos que necesiten curación
            bool playerNeedsHealingNearby = false;
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player p = Main.player[i];
                if (p.active && !p.dead)
                {
                    float dist = Vector2.Distance(NPC.Center, p.Center);
                    if (dist < 128f && p.statLife < p.statLifeMax2)
                    {
                        playerNeedsHealingNearby = true;
                        break;
                    }
                }
            }

            // Si no hay jugadores que necesiten curación se cura sola
            if (!playerNeedsHealingNearby && NPC.life < NPC.lifeMax && healCooldown <= 0)
            {
                healCooldown = 60;

                int selfHealAmount = 10;

                NPC.life += selfHealAmount;
                if (NPC.life > NPC.lifeMax)
                    NPC.life = NPC.lifeMax;

                NPC.HealEffect(selfHealAmount, true);

                // Aura de empuje
                Projectile.NewProjectile(
                    NPC.GetSource_FromAI(),
                    NPC.Center,
                    Vector2.Zero, // sin movimiento
                    ModContent.ProjectileType<HealingAura>(),
                    0, // sin daño
                    0f, // sin retroceso clásico
                    Main.myPlayer
                );
            }

            if (healCooldown > 0)
                healCooldown--;

            // Movimiento autónomo
            if (!playerNeedsHealingNearby)
            {
                // Iniciar caminata si no está caminando
                if (wanderTimer <= 0 && Main.rand.NextBool(600)) // ≈ cada 10 segundos
                {
                    wanderDuration = Main.rand.Next(60, 180); // duración de la caminata: 1 a 3 segundos
                    wanderSwitchTimer = Main.rand.Next(20, 60); // tiempo para cambiar dirección
                    wanderDirection = Main.rand.NextBool() ? 1 : -1; // 1 = derecha, -1 = izquierda
                    wanderTimer = wanderDuration;
                }

                if (wanderTimer > 0)
                {
                    NPC.direction = wanderDirection;

                    // Mover en la dirección actual
                    NPC.velocity.X = wanderDirection * 1.2f;

                    wanderTimer--;
                    wanderSwitchTimer--;

                    // Cambiar de dirección a mitad del trayecto
                    if (wanderSwitchTimer <= 0 && wanderTimer > 0)
                    {
                        wanderDirection *= -1;
                        NPC.direction = wanderDirection; // actualizar también aquí por si cambia
                        wanderSwitchTimer = Main.rand.Next(20, 60);
                    }

                    // Saltar si choca
                    if (NPC.collideX && NPC.velocity.Y == 0f)
                    {
                        NPC.velocity.Y = -4f;
                    }
                }
                else
                {
                    NPC.velocity.X = 0f;
                }
            }
            // Gesto especial (idle)
            if (specialIdleFrame < 0 && NPC.velocity.X == 0f && talkTimer <= 0 && idleBreathStage == 0 && Main.rand.NextBool(600))
            {
                int chance = Main.rand.Next(10);

                if (chance < 2)
                {
                    specialIdleFrame = 5;
                    specialIdleFrameTimer = 30;
                    specialFrameStage = 1;
                }
                else if (chance >= 4 && chance < 7)
                {
                    specialIdleFrame = 7;
                    specialIdleFrameTimer = 60;
                    specialFrameStage = 0;
                }

                // Reinicia respiración para que no interrumpa
                idleBreathStage = 0;
                idleBreathTimer = 0;
                idleTimer = 0;
            }
        }

        // Método para obtener el diálogo de la enfermera
        public override string GetChat()
        {
            //Diálogos y variables
            string[] dialogues = new string[] { };
            Player player = Main.LocalPlayer;
            // Reacción según vida del jugador
            float vidaActual = player.statLife;
            float vidaMaxima = player.statLifeMax2;
            float porcentajeVida = vidaActual / vidaMaxima;

            //Diálogos según vida del jugador
            if (porcentajeVida < 0.25f)
                return "¡Estás muy malherido! ¡No te muevas, comenzaré la curación de inmediato!";
            else if (porcentajeVida < 0.5f)
                return "¡Eso no se ve bien! Quédate quieto, voy a ayudarte ahora mismo.";
            else if (porcentajeVida < 1.0f)
                return "¡Te ves un poco herido! Ven, déjame revisarte antes de que empeore.";

            // Diálogos especiales por eventos globales
            if (Main.bloodMoon)
            {
                string[] bloodMoonDialogues = new string[]
                       {
                        "¡Oh no! Espero que mis pacientes estén a salvo...",
                        "¡No salgas sin protección, está muy peligroso!",
                        "Mis medicinas no sirven contra esos monstruos...",
                        "¡Hoy no es día para enfermarse, la sala está más loca que un Magikarp tratando de aprender Surf!"
                       };
                return bloodMoonDialogues[Main.rand.Next(bloodMoonDialogues.Length)];
            }
            if (Main.eclipse)
            {
                string[] eclipseDialogues = new string[]
                        {
                        "¡Qué extraño clima! Pero no me detendrá de cuidar a mis pacientes.",
                        "¡Ni siquiera la oscuridad me impide trabajar!",
                        "¿Un eclipse? ¡Seguro que Rayquaza está jugando con el interruptor de la luz otra vez!",
                        };
                return eclipseDialogues[Main.rand.Next(eclipseDialogues.Length)];
            }
            if (Main.invasionType == InvasionID.MartianMadness)
            {
                string[] martianDialogues = new string[]
                        {
                        "¡Marcianos llegan y se van, pero un Gengar travieso se queda!",
                        "¡Los pokémon están a salvo aquí!",
                        "¡Cuiden a los Pokémon de los rayos láser!",
                        };
                return martianDialogues[Main.rand.Next(martianDialogues.Length)];
            }
            if (Main.invasionType == InvasionID.PirateInvasion)
            {
                string[] pirateDialogues = new string[]
                        {
                        "¡Los piratas no tienen nada que hacer contra una enfermera como yo!",
                        "¡Guarden las medicinas, vienen por el botín!",
                        "¡¿Equipo Rocket?! ¡No, son piratas! ¡Pero no importa, los Pokémon están a salvo!",
                        };
                return pirateDialogues[Main.rand.Next(pirateDialogues.Length)];
            }

            //Horario de diálogos
            // 12AM - 3AM
            if (!Main.dayTime && Main.time >= 16200 && Main.time < 32400)
            {
                dialogues = new string[]
                {
            "¿Vamos a comenzar tan temprano? ¡Estoy lista para ayudar!",
            "Todavía es de madrugada... pero siempre hay trabajo que hacer.",
            "Mis pacientes duermen, pero yo sigo vigilando.",
                };
                return dialogues[Main.rand.Next(dialogues.Length)];
            }

            // 4AM - 7AM y 9AM - 11AM
            else if ((Main.dayTime && Main.time >= 0 && Main.time < 9000) || (Main.time >= 16200 && Main.time < 23400))
            {
                dialogues = new string[]
                {
            "¡Buenos días! ¿Cómo puedo ayudarte hoy?",
            "Un nuevo día, nuevas oportunidades para sanar.",
            "¿Ya desayunaste? Yo ya curé a tres Pokémon esta mañana.",
                };
                return dialogues[Main.rand.Next(dialogues.Length)];
            }

            // 12PM - 2PM
            else if (Main.dayTime && Main.time >= 27000 && Main.time < 34200)
            {
                dialogues = new string[]
                {
            "Buenas tardes, ¿cómo puedo ayudarte hoy?",
            "Hora de almorzar... o de una revisión médica.",
            "A esta hora suelo organizar mis pociones.",
                };
                return dialogues[Main.rand.Next(dialogues.Length)];
            }

            // 4PM - 6PM
            else if (Main.dayTime && Main.time >= 41400 && Main.time < 48600)
            {
                dialogues = new string[]
                {
            "¿Has tenido un buen día hasta ahora?",
            "Ya casi termina el día, ¡pero sigo disponible!",
            "Las curas de la tarde son mis favoritas.",
                };
                return dialogues[Main.rand.Next(dialogues.Length)];
            }

            // 8PM - 11PM
            else if (!Main.dayTime && Main.time >= 1800 && Main.time < 12600)
            {
                dialogues = new string[]
                {
            "Buenas noches, aventurero. ¿Necesitas curación antes de descansar?",
            "A esta hora suelo revisar historias clínicas.",
            "Espero que tus Pokémon tengan dulces sueños.",
                };
                return dialogues[Main.rand.Next(dialogues.Length)];
            }

            //Generales
            dialogues = new string[]
            {
        "¿Necesitas ayuda con tus Pokémon?",
        "¡Cuidar a los demás es mi especialidad!",
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

        public override void FindFrame(int frameHeight)
        {
            // 1) Hablar (si talkTimer > 0, y parado)
            if (talkTimer > 0 && NPC.velocity.X == 0f)
            {
                talkTimer--;
                NPC.frameCounter++;
                if (NPC.frameCounter >= 5)
                {
                    NPC.frameCounter = 0;
                    if (talkForward)
                    {
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y >= frameHeight * 5) talkForward = false;
                    }
                    else
                    {
                        NPC.frame.Y -= frameHeight;
                        if (NPC.frame.Y <= frameHeight) talkForward = true;
                    }
                }
                return;
            }

            // 2) Sentada (sólo si realmente está sobre una silla y sin moverse)
            bool noPlayersNearby = true;
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player p = Main.player[i];
                if (p.active && !p.dead && Vector2.Distance(NPC.Center, p.Center) < 400f) // ajusta distancia
                {
                    noPlayersNearby = false;
                    break;
                }
            }

            if (Main.dayTime == false && noPlayersNearby)
            {
                NPC.frameCounter++;
                if (NPC.frameCounter >= 15)
                {
                    NPC.frameCounter = 0;
                    if (NPC.frame.Y < 12 * frameHeight || NPC.frame.Y >= 13 * frameHeight)
                        NPC.frame.Y = 12 * frameHeight;
                    else
                        NPC.frame.Y += frameHeight;
                }
                return;
            }

            // 3) Gestos especiales (guiño, mirar, etc.)
            if (specialIdleFrame >= 0)
            {
                NPC.velocity.X = 0f;

                NPC.frame.Y = frameHeight * specialIdleFrame;
                specialIdleFrameTimer--;

                if (specialIdleFrameTimer <= 0)
                {
                    if (specialFrameStage == 1)
                    {
                        specialIdleFrame = 6; // segundo frame del gesto
                        specialIdleFrameTimer = 30;
                        specialFrameStage = 2;
                        NPC.frame.Y = frameHeight * specialIdleFrame;
                    }
                    else
                    {
                        // Finaliza gesto
                        specialIdleFrame = -1;
                        specialFrameStage = 0;
                    }
                }
                return;
            }

            // 4) Bestiario (icono de bestiario)
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

            // 5) Caminar (si tiene velocidad X)
            if (NPC.velocity.X != 0f)
            {
                NPC.frameCounter++;
                if (NPC.frameCounter >= 10)
                {
                    NPC.frameCounter = 0;
                    if (NPC.direction == -1)
                    {
                        // caminar izq (frames 9 y 10: índices 8 y 9)
                        if (NPC.frame.Y < frameHeight * 8 || NPC.frame.Y > frameHeight * 9)
                            NPC.frame.Y = frameHeight * 8;
                        else
                            NPC.frame.Y = NPC.frame.Y == frameHeight * 8
                                ? frameHeight * 9
                                : frameHeight * 8;
                    }
                    else
                    {
                        // caminar der (frames 11 y 12: índices 10 y 11)
                        if (NPC.frame.Y < frameHeight * 10 || NPC.frame.Y > frameHeight * 11)
                            NPC.frame.Y = frameHeight * 10;
                        else
                            NPC.frame.Y = NPC.frame.Y == frameHeight * 10
                                ? frameHeight * 11
                                : frameHeight * 10;
                    }
                }
                return;
            }

            // 6) Idle (respiración lenta y posibilidad de gesto especial)
            idleTimer++;

            if (idleBreathStage == 0 && (NPC.frame.Y >= frameHeight * 10 || NPC.frame.Y <= frameHeight * 8))
                NPC.frame.Y = 0;

            NPC.frameCounter++;

            if (idleBreathStage == 0 && NPC.frameCounter >= 120) // Esperar 2 segundos
            {
                NPC.frameCounter = 0;
                NPC.frame.Y = frameHeight * 4;
                idleBreathStage = 1;
                idleBreathTimer = 60; // Mantener inhalación por 1 segundo
            }
            else if (idleBreathStage == 1)
            {
                idleBreathTimer--;
                if (idleBreathTimer <= 0)
                {
                    NPC.frame.Y = 0;
                    idleBreathStage = 0;
                }
            }
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

        // Añadir tienda de objetos
        public override void AddShops()
        {
            CreateShop(Type,
                "Default",
            new int[] { //Items Prehardmode
                ItemID.LesserHealingPotion,
                ItemID.HealingPotion
            },
            new int[] { //Items Hardmode
                ItemID.GreaterHealingPotion,
                ItemID.SuperHealingPotion
            }
            );
        }


        // Botones de chat
        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = "Ver tienda";
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            if (firstButton)
            {
                shopName = "Default"; // Nombre de la tienda registrada en AddShops
                talkTimer = 60; //Animación de hablar
            }
        }
    }
}
