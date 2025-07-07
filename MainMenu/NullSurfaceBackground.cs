using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace PokeNPCS.MainMenu
{
    /// <summary>
    /// Fondo superficial "nulo" para evitar que Terraria dibuje montañas, cielo, nubes, etc.
    /// </summary>
    internal class NullSurfaceBackground : ModSurfaceBackgroundStyle
    {
        public override void ModifyFarFades(float[] fades, float transitionSpeed)
        {
            for (int i = 0; i < fades.Length; i++)
            {
                if (i == Slot)
                {
                    fades[i] += transitionSpeed;
                    if (fades[i] > 1f)
                        fades[i] = 1f;
                }
                else
                {
                    fades[i] -= transitionSpeed;
                    if (fades[i] < 0f)
                        fades[i] = 0f;
                }
            }
        }

        // Usamos un píxel transparente para evitar que se dibuje fondo vanilla
        private static readonly string TexPath = "PokeNPCS/MainMenu/BlankPixel";

        public override int ChooseCloseTexture(ref float scale, ref double parallax, ref float a, ref float b) => BackgroundTextureLoader.GetBackgroundSlot(TexPath);
        public override int ChooseFarTexture() => BackgroundTextureLoader.GetBackgroundSlot(TexPath);
        public override int ChooseMiddleTexture() => BackgroundTextureLoader.GetBackgroundSlot(TexPath);

        // Impedimos que dibuje el fondo cercano
        public override bool PreDrawCloseBackground(SpriteBatch spriteBatch) => false;
    }
}
