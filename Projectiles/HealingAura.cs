using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

public class HealingAura : ModProjectile
{
    public override string Texture => "PokeNPCS/Sprites/Projectiles/HealingAura"; // Ruta
    public override void SetDefaults()
    {
        Projectile.width = 40;  
        Projectile.height = 40;
        Projectile.aiStyle = 0;
        Projectile.friendly = false;
        Projectile.hostile = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 120;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Main.projFrames[Projectile.type] = 6;
    }

    public override void AI()
    {
        // Animación: cambia de frame cada 20 ticks
        Projectile.frameCounter++;
        if (Projectile.frameCounter >= 20)
        {
            Projectile.frameCounter = 0;
            Projectile.frame++;
            if (Projectile.frame >= Main.projFrames[Projectile.type])
                Projectile.frame = 0;
        }
        // Empujar NPCs hostiles
        foreach (NPC npc in Main.npc)
        {
            if (npc.active && !npc.friendly && npc.Distance(Projectile.Center) < 80f)
            {
                Vector2 push = npc.Center - Projectile.Center;
                push.Normalize();
                push *= 4f; // 4 bloques de empuje (≈ 4 px/tick)
                npc.velocity += push;
            }
        }

        // Si quieres también empujar jugadores enemigos en PvP:
        foreach (Player p in Main.player)
        {
            if (p.active && !p.dead && p.hostile && p.team != Main.player[Projectile.owner].team)
            {
                if (p.Distance(Projectile.Center) < 80f)
                {
                    Vector2 push = p.Center - Projectile.Center;
                    push.Normalize();
                    push *= 5f;
                    p.velocity += push;
                }
            }
        }
    }
}