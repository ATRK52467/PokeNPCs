using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace PokeNPCS;

public class ArticunoPhaseMusic : ModSceneEffect
{
    public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/ArticunoTheme1");

    public override SceneEffectPriority Priority => SceneEffectPriority.BossLow;

    public override bool IsSceneEffectActive(Player player)
    {
        // Si hay un Articuno activo
        foreach (var npc in Main.npc)
        {
            if (npc.active && npc.type == ModContent.NPCType<NPCs.Articuno>())
            {
                return true;
            }
        }
        return false;
    }
}
