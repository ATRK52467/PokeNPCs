using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

public class HealingAura : ModProjectile
{
    public override string Texture => "PokeNPCS/Sprites/Projectiles/HealingAura"; // Ruta
    public override void SetDefaults()
    {
        Projectile.width = 80;  // 5 bloques * 16
        Projectile.height = 80;
        Projectile.aiStyle = 0;
        Projectile.friendly = false;
        Projectile.hostile = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 10;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
    }

    public override void AI()
    {
        // Empujar NPCs hostiles
        foreach (NPC npc in Main.npc)
        {
            if (npc.active && !npc.friendly && npc.Distance(Projectile.Center) < 80f)
            {
                Vector2 push = npc.Center - Projectile.Center;
                push.Normalize();
                push *= 5f; // 5 bloques de empuje (≈ 5 px/tick)
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