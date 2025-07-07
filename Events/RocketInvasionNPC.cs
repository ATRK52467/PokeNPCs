using Terraria;
using Terraria.ModLoader;
using PokeNPCS.Events;

namespace PokeNPCS.Events
{
    public class RocketInvasionNPC : GlobalNPC
    {
        public bool IsRocketMember = false;

        public override bool InstancePerEntity => true;

        public override void SetDefaults(NPC npc)
        {
            if (npc.type == ModContent.NPCType<R1>()
                /* || otros Rocket Grunts que quieras marcar */)
            {
                IsRocketMember = true;
            }

            if (npc.type == ModContent.NPCType<Giovanni>())
            {
                IsRocketMember = true;
            }
        }

        public override void OnKill(NPC npc)
        {
            if (IsRocketMember && RocketInvasion.Active)
            {
                RocketInvasion.OnRocketNPCKilled(npc);
            }
        }
        

    }
}
