using System.Linq;
using Microsoft.Xna.Framework;
using PokeNPCS.NPCs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Terraria.ModLoader.IO;

namespace PokeNPCS.Events
{
    public class RocketInvasion : ModSystem
    {
        public static bool Active = false;
        public static int Progress = 0;
        public const int Size = 100;
        public static bool HasBeatenRocketInvasion = false;

        private static int SpawnCooldown = 0;
        private const int SpawnInterval = 120;

        private static bool BossSpawned = false;

        public override void PreUpdateNPCs()
        {
            if (Active)
                HandleInvasion();
        }

        public override void PreUpdateInvasions()
        {
            if (Active)
            {
                Main.invasionType = -1;
                Main.invasionSize = Size - Progress;
                Main.invasionProgress = Progress;
                Main.invasionProgressMax = Size;
                Main.invasionProgressIcon = ModContent.NPCType<R1>();
                Main.invasionProgressNearInvasion = true;

                if (Progress >= Size)
                {
                    Main.invasionProgressNearInvasion = false;
                }
            }
        }

        public static void Start()
        {
            if (!Active)
            {
                Active = true;
                Progress = 89;//CAMBIAR PACO
                SpawnCooldown = 0;
                BossSpawned = false;

                Main.invasionType = -1;
                Main.invasionSize = Size;
                Main.invasionProgress = 0;
                Main.invasionProgressMax = Size;
                Main.invasionProgressIcon = ModContent.NPCType<R1>();
                Main.invasionProgressNearInvasion = true;

                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Main.NewText("[Equipo Rocket] ¡Ha comenzado la invasión!", 175, 75, 255);
                }
                else
                {
                    Terraria.Chat.ChatHelper.BroadcastChatMessage(
                        Terraria.Localization.NetworkText.FromLiteral("[Equipo Rocket] ¡Ha comenzado la invasión!"),
                        new Color(175, 75, 255));

                    NetMessage.SendData(MessageID.WorldData);
                }
            }
        }

        private void HandleInvasion()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            int activeRocket = 0;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && npc.GetGlobalNPC<RocketInvasionNPC>().IsRocketMember)
                    activeRocket++;
            }

            // 🔹 SI YA ESTAMOS EN FASE DE JEFE (Progress >= Size - 1) 🔹
            if (Progress >= Size - 1)
            {
                // Invocar a Giovanni si no está y no se ha spawneado aún
                if (!BossSpawned && !NPC.AnyNPCs(ModContent.NPCType<Giovanni>()))
                {
                    Vector2 bossPos = SpawnBossFarFromPlayer();
                    NPC.NewNPC(null, (int)bossPos.X, (int)bossPos.Y, ModContent.NPCType<Giovanni>());
                    BossSpawned = true;

                    if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        Main.NewText("[Equipo Rocket] ¡Giovanni ha aparecido!", 255, 0, 0);
                    }
                    else
                    {
                        Terraria.Chat.ChatHelper.BroadcastChatMessage(
                            Terraria.Localization.NetworkText.FromLiteral("[Equipo Rocket] ¡Giovanni ha aparecido!"),
                            new Microsoft.Xna.Framework.Color(255, 0, 0));
                    }
                }

                // 💥 NO TERMINAR LA INVASIÓN HASTA QUE GIOVANNI MUERA
                if (Progress >= Size && !NPC.AnyNPCs(ModContent.NPCType<Giovanni>()))
                {
                    Active = false;
                    Main.invasionProgressNearInvasion = false;
                    Main.invasionSize = 0;
                    BossSpawned = false;

                    HasBeatenRocketInvasion = true;
                }

                return;
            }

            // 🔹 FASE NORMAL DE GRUNTS 🔹
            if (activeRocket < 5)
            {
                SpawnCooldown--;
                if (SpawnCooldown <= 0)
                {
                    int[] spawnables = {
                ModContent.NPCType<R1>(),
            };

                    if (spawnables.Length > 0)
                    {
                        int choice = Main.rand.Next(spawnables.Length);
                        Vector2 pos = SpawnNearPlayer();
                        NPC.NewNPC(null, (int)pos.X, (int)pos.Y, spawnables[choice]);
                    }

                    SpawnCooldown = SpawnInterval;
                }
            }
        }


        private Vector2 SpawnNearPlayer()
        {
            Player closestPlayer = Main.player
                .Where(p => p.active && !p.dead)
                .OrderBy(p => Vector2.DistanceSquared(p.Center, new Vector2(Main.spawnTileX * 16, Main.spawnTileY * 16)))
                .FirstOrDefault();

            if (closestPlayer == null)
                closestPlayer = Main.player[Main.myPlayer];

            return closestPlayer.Center + new Vector2(Main.rand.Next(-800, 800), -400);
        }
        private static Vector2 SpawnBossFarFromPlayer()
        {
            Player closestPlayer = Main.player
                .Where(p => p.active && !p.dead)
                .OrderBy(p => Vector2.DistanceSquared(p.Center, new Vector2(Main.spawnTileX * 16, Main.spawnTileY * 16)))
                .FirstOrDefault();

            if (closestPlayer == null)
                closestPlayer = Main.player[Main.myPlayer];

            // Aparece entre -800 y +800 en X, y hasta 320 arriba en Y
            Vector2 offset = new Vector2(Main.rand.Next(-800, 801), -Main.rand.Next(0, 321));
            Vector2 spawnPos = closestPlayer.Center + offset;

            // Asegurarse de que esté dentro del mundo
            spawnPos.X = Math.Clamp(spawnPos.X, 100, Main.maxTilesX * 16 - 100);
            spawnPos.Y = Math.Clamp(spawnPos.Y, 100, Main.maxTilesY * 16 - 100);

            return spawnPos;
        }

        // Nuevo método para sumar progreso cuando un Rocket NPC muere
        public static void OnRocketNPCKilled(NPC npc)
        {
            if (!Active)
                return;

            bool isBoss = npc.type == ModContent.NPCType<Giovanni>();

            if (isBoss)
            {
                Progress = Size;
            }
            else if (Progress < Size - 1)
            {
                Progress++;
                if (Progress > Size - 1)
                    Progress = Size - 1;
            }

            Main.invasionSize = Size - Progress;

            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendData(MessageID.InvasionProgressReport);
            }
        }
        public override void SaveWorldData(TagCompound tag)
        {
            tag["HasBeatenRocketInvasion"] = HasBeatenRocketInvasion;
        }
        public override void LoadWorldData(TagCompound tag)
        {
            HasBeatenRocketInvasion = tag.GetBool("HasBeatenRocketInvasion");
        }


    }
}
