using Terraria;
using Terraria.ModLoader;
using System;

namespace PokeNPCS.Systems
{
    public class RocketInvasionSystem : ModSystem
    {
        public override void PostUpdateWorld()
        {
            // Solo si no hay invasión activa
            if (!Events.RocketInvasion.Active)
            {
                // Verifica cambio de día
                if (Main.dayTime && Main.time == 0)
                {
                    TryStartRocketInvasion();
                }
            }
        }

        private void TryStartRocketInvasion()
        {
            //Probabilidad invasión (0.1 = 10% cada amanecer)
            float chance = 0.1f;

            if (Main.rand.NextFloat() < chance)
            {
                Events.RocketInvasion.Start();
                Main.NewText("¡Preparense para los problemas, y más vale que teman!...", 255, 50, 255);
            }
        }
    }
}
