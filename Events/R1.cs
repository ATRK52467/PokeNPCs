using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using PokeNPCS.Events;

namespace PokeNPCS.Events
{
    public class R1 : ModNPC
    {
        public override string Texture => "PokeNPCS/Sprites/NPCs/Azul";
        public override void SetStaticDefaults()
        {
            //Main.npcFrameCount[NPC.type] = Main.npcFrameCount[NPCID.ArmsDealer]; Reactivar en futuro o cambiar
            Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[NPC.type] = false;

        }

        public override void SetDefaults()
        {
            NPC.width = 40;
            NPC.height = 60;
            NPC.damage = 80;
            NPC.defense = 20;
            NPC.lifeMax = 500;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 10, 0, 0);
            NPC.aiStyle = 3;
            AIType = NPCID.Zombie;
            AIType = NPCID.PirateCorsair;
            NPC.boss = true;
            NPC.npcSlots = 10f;
            NPC.GetGlobalNPC<RocketInvasionNPC>().IsRocketMember = true;
            Music = MusicID.Boss2;
        }

        public override void OnKill()
        {
            //lógica para este enemigo al morir
        }


        public override void AI()
        {
            // Puedes personalizar su comportamiento aquí, de momento la hereda
        }
    }
}
