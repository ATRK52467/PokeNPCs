using Terraria.ModLoader;
using Terraria.GameContent.UI.BigProgressBar;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;

namespace PokeNPCS.NPCs.BossBars
{
    public class ArticunoBossBar : ModBossBar
    {
        public void DrawBar(SpriteBatch spriteBatch, NPC npc, float lifePercent)
        {
            if (!npc.active || npc.life <= 0) return;

            Vector2 screenPos = new Vector2(Main.screenWidth / 2f, 50);
            int width = 300;
            int height = 20;

            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)screenPos.X - width / 2, (int)screenPos.Y, width, height), Color.DarkBlue * 0.5f);

            int filledWidth = (int)(width * lifePercent);
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)screenPos.X - width / 2, (int)screenPos.Y, filledWidth, height), Color.Cyan);
        }
    }
}
