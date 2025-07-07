using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace PokeNPCS.MainMenu
{
    public class PokeNPCSMainMenu : ModMenu
    {
        public override int Music => MusicLoader.GetMusicSlot(ModContent.GetInstance<PokeNPCS>(), "Sounds/Music/Musica");
        private int currentFrame = 0;
        private int frameTimer = 0;

        private const int TotalFrames = 26; // Ajusta al número de frames reales
        private const int FrameDuration = 5; // Ajusta la velocidad de la animación

        public class Cinder
        {
            public int Time;
            public int Lifetime;
            public int IdentityIndex;
            public float Scale;
            public float Depth;
            public Color DrawColor;
            public Vector2 Velocity;
            public Vector2 Center;



            public Cinder(int lifetime, int identity, float depth, Color color, Vector2 startingPosition, Vector2 startingVelocity)
            {
                Lifetime = lifetime;
                IdentityIndex = identity;
                Depth = depth;
                DrawColor = color;
                Center = startingPosition;
                Velocity = startingVelocity;
            }
        }

        public static List<Cinder> Cinders { get; internal set; } = new();

        public override string DisplayName => "PokeNPCS Menu";

        public override Asset<Texture2D> Logo => ModContent.Request<Texture2D>("PokeNPCS/MainMenu/Logo");

        public override ModSurfaceBackgroundStyle MenuBackgroundStyle => ModContent.GetInstance<NullSurfaceBackground>();

        public override bool PreDrawLogo(SpriteBatch spriteBatch, ref Vector2 logoDrawCenter, ref float logoRotation, ref float logoScale, ref Color drawColor)
        {
            //Sprite Background Animation
            Texture2D spriteSheet = ModContent.Request<Texture2D>("PokeNPCS/MainMenu/MenuBackgroundAnimated").Value;

            // 📌 Calculate frame size (vertical layout)
            int frameHeight = spriteSheet.Height / TotalFrames;
            Rectangle sourceRect = new Rectangle(0, frameHeight * currentFrame, spriteSheet.Width, frameHeight);

            // 📌 Advance animation frame
            frameTimer++;
            if (frameTimer >= FrameDuration)
            {
                frameTimer = 0;
                currentFrame = (currentFrame + 1) % TotalFrames;
            }

            // 📌 Compute scaling to fill the entire screen
            Vector2 drawOffset = Vector2.Zero;
            float xScale = (float)Main.screenWidth / spriteSheet.Width;
            float yScale = (float)Main.screenHeight / frameHeight;
            float scale = xScale;

            if (xScale != yScale)
            {
                if (yScale > xScale)
                {
                    scale = yScale;
                    drawOffset.X -= (spriteSheet.Width * scale - Main.screenWidth) * 0.5f;
                }
                else
                {
                    drawOffset.Y -= (frameHeight * scale - Main.screenHeight) * 0.5f;
                }
            }

            // 📌 Draw background frame
            spriteBatch.Draw(spriteSheet, drawOffset, sourceRect, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

            // CINDER PARTICLES
            Texture2D cinderTexture = ModContent.Request<Texture2D>("PokeNPCS/MainMenu/Cinder").Value;

            static Color GetCinderColor()
            {
                if (Main.rand.NextBool(3))
                    return Color.Lerp(Color.Gray, Color.WhiteSmoke, Main.rand.NextFloat());
                return Color.Lerp(Color.CornflowerBlue, Color.SkyBlue, Main.rand.NextFloat(0.8f));
            }

            // Generar nuevos cinders aleatorios
            for (int i = 0; i < 1; i++)
            {
                if (Main.rand.NextBool(4))
                {
                    int lifetime = Main.rand.Next(200, 300);
                    float depth = Main.rand.NextFloat(1.8f, 5f);
                    Vector2 startingPosition = new Vector2(Main.screenWidth * Main.rand.NextFloat(-0.1f, 1.1f), Main.screenHeight * 1.05f);
                    Vector2 startingVelocity = -Vector2.UnitY.RotatedBy(Main.rand.NextFloat(-0.9f, 0.9f)) * 4f;
                    Color cinderColor = GetCinderColor();
                    Cinders.Add(new Cinder(lifetime, Cinders.Count, depth, cinderColor, startingPosition, startingVelocity));
                }
            }

            // Actualizar cinders existentes
            foreach (var cinder in Cinders)
            {
                cinder.Scale = Utils.GetLerpValue(cinder.Lifetime, cinder.Lifetime / 3, cinder.Time, true);
                cinder.Scale *= MathHelper.Lerp(0.6f, 0.9f, cinder.IdentityIndex % 6f / 6f);
                if (cinder.IdentityIndex % 13 == 12)
                    cinder.Scale *= 2f;

                float flySpeed = MathHelper.Lerp(3.2f, 14f, cinder.IdentityIndex % 21f / 21f);
                // Movimiento solo hacia arriba, con ligera variación en ángulo
                Vector2 idealVelocity = -Vector2.UnitY.RotatedBy(MathHelper.Lerp(-0.2f, 0.2f, (float)Math.Sin(cinder.Time / 20f + cinder.IdentityIndex)));
                idealVelocity = idealVelocity.SafeNormalize(Vector2.UnitY) * flySpeed;


                float movementInterpolant = MathHelper.Lerp(0.01f, 0.08f, Utils.GetLerpValue(45f, 145f, cinder.Time, true));
                cinder.Velocity = Vector2.Lerp(cinder.Velocity, idealVelocity, movementInterpolant);

                cinder.Time++;
                cinder.Center += cinder.Velocity;
            }

            // Eliminar cinders expirados
            Cinders.RemoveAll(c => c.Time >= c.Lifetime);

            // Dibujar cinders
            foreach (var cinder in Cinders)
            {
                spriteBatch.Draw(cinderTexture, cinder.Center, null, cinder.DrawColor, 0f, cinderTexture.Size() * 0.5f, cinder.Scale, SpriteEffects.None, 0f);
            }

            // Dibujar logo encima
            Vector2 drawPos = new Vector2(Main.screenWidth / 2f, 100f);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer, null, Main.UIScaleMatrix);
            spriteBatch.Draw(Logo.Value, drawPos, null, Color.White, logoRotation, Logo.Value.Size() * 0.5f, logoScale, SpriteEffects.None, 0f);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer, null, Main.UIScaleMatrix);

            // ✅ Fix daytime (prevent night-time tint)
            drawColor = Color.White;
            Main.time = 27000;
            Main.dayTime = true;

            return false;
        }

    }
}
