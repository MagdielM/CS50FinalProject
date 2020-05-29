using FrameDataLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Game1 {
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    class MainGame : Game {

        #region General References

        readonly int virtualWidth = 256;
        readonly int virtualHeight = 144;
        Rectangle screen;
        RenderTarget2D renderTarget;
        readonly GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        #endregion

        #region Game References

        public PlayerCharacter Player;
        private int score = 0;

        #endregion

        #region Content References

        Texture2D background1;
        private SpriteFont font;
        Dictionary<string, SpriteReference> playerSpriteSet;

        #endregion


        public MainGame() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content. Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize() {
            Player = new PlayerCharacter(new Vector2(50, 50));
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.ApplyChanges();
            renderTarget = new RenderTarget2D(GraphicsDevice, virtualWidth, virtualHeight);
            screen = new Rectangle(new Point(0, 0), new Point(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight));
            Player.Initialize();
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            background1 = Content.Load<Texture2D>("Sprites/Assets/Background_1");
            font = Content.Load<SpriteFont>("Sprites/PixelFont");

            var playerData = Content.Load<FrameData>("Sprites/PlatformerPack/Player/player_frame_data");
            playerSpriteSet = new Dictionary<string, SpriteReference> {
                {
                    "Main",
                    new SpriteReference(
                Content.Load<Texture2D>(playerData.ImagePath), playerData)
                }
            };

            Player.LoadInit(playerSpriteSet, playerSpriteSet.Keys.ElementAt(0));
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent() {

        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime) {

            // Checks for held keys
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            score++;
            Player.Update(gameTime);
            InputHandler.Update();
            foreach (Keys key in InputHandler.InputOutKeys.Value) {
                Player.ManageInput(key);
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {

            // Draw to internal resolution render target.
            GraphicsDevice.SetRenderTarget(renderTarget);
            GraphicsDevice.Clear(Color.DeepSkyBlue);
            Vector2 size = font.MeasureString("Score: " + score);
            spriteBatch.Begin
                (SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp,
                null, null, null, null);
            spriteBatch.Draw(background1, new Rectangle(0, 0, virtualWidth, virtualHeight), Color.White);
            spriteBatch.DrawString(font, "Score: " + score, new Vector2(Window.ClientBounds.Width / 2 - size.X / 2, 50), Color.Black);
            Player.Draw(spriteBatch);
            spriteBatch.End();

            // Draw internal render target to back buffer.
            GraphicsDevice.SetRenderTarget(null);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp);
            spriteBatch.Draw(renderTarget, screen, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
