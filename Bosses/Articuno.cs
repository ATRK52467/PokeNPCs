using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.GameContent.Bestiary;
using Terraria.DataStructures;
using Terraria.GameContent.RGB;
using PokeNPCS.NPCs.BossBars;
using Terraria.Audio;
using System;

namespace PokeNPCS.NPCs;

public class Articuno : ModNPC
{
    public override string Texture => "PokeNPCS/Sprites/Bosses/Articuno";

    // --- VARIABLES PARA IA PERSONALIZADA ---
    private Player targetPlayer = null; // Referencia al objetivo actual

    // Variables para el sistema de furia
    private int noHitTimer = 0;
    private bool isFuryMode = false;
    private int furyTimer = 0;
    private int furyCooldown = 0;
    private int reacquireCooldown = 0; // Ticks de espera tras evasión


    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 3; // Número de frames de animación
        NPCID.Sets.DangerDetectRange[Type] = 700;
        NPCID.Sets.AttackFrameCount[Type] = 1;
    }

    public override void SetDefaults()
    {
        NPC.width = 60;
        NPC.height = 60;
        NPC.aiStyle = 14; // Harpy
        AIType = NPCID.Harpy;

        NPC.damage = 200;
        NPC.defense = 300;
        NPC.lifeMax = 500000;
        NPC.knockBackResist = 0f;

        NPC.noGravity = true;
        NPC.noTileCollide = false;
        NPC.value = Item.buyPrice(0, 30, 0, 0);

        NPC.boss = true;
        NPC.npcSlots = 10f;

        NPC.lavaImmune = true;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.BossBar = ModContent.GetInstance<ArticunoBossBar>();

    }

    // --- Animaciones ---
    public override void FindFrame(int frameHeight)
    {
        NPC.frameCounter++;
        if (NPC.frameCounter >= 10) // cambia frame cada 10 ticks
        {
            NPC.frameCounter = 0;
            NPC.frame.Y += frameHeight;
            if (NPC.frame.Y >= frameHeight * Main.npcFrameCount[NPC.type])
            {
                NPC.frame.Y = 0; // vuelve al primer frame
            }
        }
    }

    // --- Al aparecer ---
    public override void OnSpawn(IEntitySource source)
    {
        if (Main.netMode != NetmodeID.Server)
        {
            // Reproduce el sonido personalizado al aparecer
            SoundEngine.PlaySound(new SoundStyle("PokeNPCS/Sounds/Bosses/ArticunoSpawn")
            {
                Volume = 1.0f,
                PitchVariance = 0.1f
            }, NPC.Center);
        }
        base.OnSpawn(source);

        // Ajusta la vida según el número de jugadores activos
        int playerCount = 0;
        for (int i = 0; i < Main.maxPlayers; i++)
        {
            if (Main.player[i].active)
            {
                playerCount++;
            }
        }

        // Ajuste base
        int baseLife = NPC.lifeMax;
        float multiplier = 1f + 0.8f * Math.Max(0, playerCount - 1);
        NPC.lifeMax = (int)(baseLife * multiplier);
        NPC.life = NPC.lifeMax;  // IMPORTANTE: setear también la vida actual

    }

    // --- IA PERSONALIZADA PARA EL "ENOJO" ---
    public override void AI()
    {
        noHitTimer++;

        if (furyCooldown > 0)
        {
            furyCooldown--;
            return;
        }

        if (Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
        {
            NPC.TargetClosest();
            Player stuckTarget = Main.player[NPC.target];
            if (stuckTarget.active && !stuckTarget.dead)
            {
                Vector2 escapeDir = Vector2.Normalize(stuckTarget.Center - NPC.Center);
                NPC.velocity = escapeDir * 10f;
            }
            return;
        }

        if (!isFuryMode && noHitTimer >= 600)
        {
            isFuryMode = true;
            furyTimer = 300;
            noHitTimer = 0;
        }

        NPC.TargetClosest();
        Player player = Main.player[NPC.target];
        float distance = Vector2.Distance(NPC.Center, player.Center);

        // Disminuir cooldown de reacercamiento si está activo
        if (reacquireCooldown > 0)
        {
            reacquireCooldown--;
        }

        // === IA en modo FURIA ===
        if (isFuryMode)
        {
            furyTimer--;

            NPC.noTileCollide = true;
            NPC.damage = 200;

            // Si está muy cerca, frena y espera 0.3s antes de volver a atacar
            if (distance < 120f)
            {
                NPC.velocity *= 0.95f;
                reacquireCooldown = 18; // 0.3 segundos (60 FPS)
            }
            else if (reacquireCooldown <= 0)
            {
                Vector2 rushDir = Vector2.Normalize(player.Center - NPC.Center);
                NPC.velocity = rushDir * 12f;
            }

            if (furyTimer <= 0)
            {
                isFuryMode = false;
                furyCooldown = 180;
                NPC.velocity *= 0.3f;
            }

            return;
        }

        // === IA NORMAL ===
        NPC.noTileCollide = false;
        NPC.damage = 120;

        if (distance > 200f && reacquireCooldown <= 0)
        {
            Vector2 dirNorm = Vector2.Normalize(player.Center - NPC.Center);
            NPC.velocity = dirNorm * 6f;
        }
        else
        {
            NPC.velocity *= 0.9f;
            reacquireCooldown = 18; // espera antes de volver a seguir
        }
    }

    // Reset al golpear al jugador
    public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
    {
        noHitTimer = 0;
    }

    // --- Movimiento asistido con suavizado ---
    private void MoveTowards(Vector2 target, float speed, float turnResistance)
    {
        Vector2 move = target - NPC.Center;
        float length = move.Length();
        if (length > speed)
        {
            move *= speed / length;
        }
        move = (NPC.velocity * turnResistance + move) / (turnResistance + 1f);
        length = move.Length();
        if (length > speed)
        {
            move *= speed / length;
        }
        NPC.velocity = move;
    }

    // --- Al morir ---
    public override void OnKill()
    {
        if (Main.netMode == NetmodeID.MultiplayerClient)
            return;

        // Verifica si el mod Pokemod está cargado
        if (ModLoader.TryGetMod("Pokemod", out Mod pokemod))
        {
            // Intenta encontrar el tipo de item "CaughtPokemonItem" en el mod Pokemod
            if (pokemod.TryFind("CaughtPokemonItem", out ModItem modItem))
            {
                // Genera en el suelo el item de Pokeball vacía (la base para el Pokémon capturado)
                int index = Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), modItem.Type);
                Item item = Main.item[index];

                // Comprueba que el item generado sea realmente un CaughtPokemonItem
                if (item.ModItem != null && item.ModItem.GetType().Name == "CaughtPokemonItem")
                {
                    dynamic caught = item.ModItem;

                    // Configura el contenido de la Pokeball
                    caught.SetPokemonData(
                        PokemonName: "Articuno",      // Nombre interno EXACTO en Pokemod
                        Shiny: false,                 // False si no quieres shiny
                        BallType: "PokeballItem",     // Tipo de Pokeball (ajusta si quieres UltraBall, etc.)
                        level: 70                     // Nivel del Pokémon
                    );

                }
            }
        }
    }
    //Ventisca y Rayo hielo

}
