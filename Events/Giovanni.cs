using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using PokeNPCS.Events;

namespace PokeNPCS.Events
{
    public class Giovanni : ModNPC
    {
        public override string Texture => "PokeNPCS/Sprites/NPCs/DarkJoy";
        public static int HeadIndex;
        public override void SetStaticDefaults()
        {
            //Main.npcFrameCount[NPC.type] = Main.npcFrameCount[NPCID.ArmsDealer]; Reactivar en futuro o cambiar
            Main.npcFrameCount[NPC.type] = 1;
        }

        public override void SetDefaults()
        {
            NPC.width = 40;
            NPC.height = 60;
            NPC.damage = 80;
            NPC.defense = 20;
            NPC.lifeMax = 5000;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 10, 0, 0);
            NPC.aiStyle = 3;
            AIType = NPCID.EyeofCthulhu;
            NPC.boss = true;
            NPC.npcSlots = 10f;
            NPC.GetGlobalNPC<RocketInvasionNPC>().IsRocketMember = true;
            Music = MusicID.Boss2;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.ShowNameOnHover = true;         // Muestra el nombre al pasar el mouse
            NPC.hide = false;                   // Asegura que sea visible
            NPC.dontCountMe = false;           // Importante para que aparezca como jefe
            NPC.lavaImmune = true;
            NPC.netAlways = true;
            NPC.dontTakeDamageFromHostiles = true;
            NPC.noTileCollide = false;
            NPC.noGravity = false;
        }

        public override void OnKill()
        {
            if (!Main.dedServ)
            {
                Main.NewText("¡El Equipo Rocket ha sido vencido otra vez...!", 255, 50, 255);
                RocketInvasion.Progress++; //100%
                RocketInvasion.Active = false; // Desactiva la invasión
                RocketInvasion.Progress = 0;

            }
        }

        public override void AI()
        {
            NPC.timeLeft = 600; // ❗ Reinicia el contador de vida cada frame para evitar despawn

        }

    }
}
