using FrameDataLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using static Game1.Level;

namespace Game1 {
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    class MainGame : Game {

        #region General References

        public const int virtualWidth = 384;
        public const int virtualHeight = 216;
        Rectangle screen;
        RenderTarget2D renderTarget;
        readonly GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public float fontHeight;
        public Vector2[] debugLinePositions;
        #endregion

        #region Game References

        public PlayerCharacter Player { get; private set; }
        private Camera camera;
        public static Vector2 risingGravity = new Vector2(0, 16f);
        public static Vector2 fallingGravity = new Vector2(0, 26f);
        public static Level currentLevel;

        #endregion

        #region Content References

        Texture2D background1;
        private SpriteFont font;
        Dictionary<string, SpriteReference> playerSpriteSet;
        private string level1TilemapPath = "Sprites/PlatformerPack/raw files/tilemap";

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
            Player = new PlayerCharacter(new Point(56, 256));
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.ApplyChanges();
            renderTarget = new RenderTarget2D(GraphicsDevice, virtualWidth, virtualHeight);
            screen = new Rectangle(new Point(0, 0), new Point(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight));
            camera = new Camera();
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
            fontHeight = font.MeasureString("Height test").Y;
            debugLinePositions = new Vector2[] {
                Vector2.Zero,
                new Vector2(0, fontHeight),
                new Vector2(0, fontHeight * 2),
                new Vector2(0, fontHeight * 3),
                new Vector2(0, fontHeight * 4),
                new Vector2(0, fontHeight * 5),
                new Vector2(0, fontHeight * 6)
            };

            currentLevel = new Level(Content.Load<Texture2D>(level1TilemapPath), LevelDefinitions.level1);

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
/*            foreach (Keys key in InputHandler.InputOutKeys.Value) {
                Player.ManageGeneralInput(key);
            }*/
            Player.Update(gameTime);
            camera.Follow(Player);
            InputHandler.Update();
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
            spriteBatch.Begin
                (SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp,
                null, null, null, null);
            spriteBatch.Draw(background1, new Rectangle(0, 0, virtualWidth, virtualHeight), Color.White);
            spriteBatch.End();

            spriteBatch.Begin
                (SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp,
                null, null, null, camera.Transform);
            currentLevel.Draw(spriteBatch, Player.surroundingTiles);
            Player.Draw(spriteBatch);
            spriteBatch.End();

            // Draw internal render target to back buffer.
            GraphicsDevice.SetRenderTarget(null);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
            spriteBatch.Draw(renderTarget, screen, Color.White);
            spriteBatch.DrawString(font, $"movementVector: {Player.movementVector}", debugLinePositions[0], Color.Black);
            spriteBatch.DrawString(font, $"surroundingTiles: {Player.surroundingTiles.Count}", debugLinePositions[1], Color.Black);
            spriteBatch.DrawString(font, $"levelTiles: {currentLevel.TileArray.Count}", debugLinePositions[2], Color.Black);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
