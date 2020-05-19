﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Security.Principal;

namespace General {
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    class Game1 : Game {

        #region General References
        int VirtualWidth = 256;
        int VirtualHeight = 144;
        Rectangle Screen;
        RenderTarget2D RenderTarget;
        GraphicsDeviceManager Graphics;
        SpriteBatch SpriteBatch;
        #endregion

        #region Game References

        private KeyboardState PreviousState;
        PlayerCharacter PlayerCharacter;
        private int Score = 0;

        #endregion

        #region Content References

        Texture2D background1;
        AnimationController playerAnimController;
        private SpriteFont font;
        Dictionary<string, AnimatedSprite> playerSpriteSet;

        #endregion


        public Game1() {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize() {
            // TODO: Add your initialization logic here
            PlayerCharacter = new PlayerCharacter(new Vector2(50, 50));
            Graphics.PreferredBackBufferWidth = 1280;
            Graphics.PreferredBackBufferHeight = 720;
            Graphics.ApplyChanges();
            RenderTarget = new RenderTarget2D(GraphicsDevice, VirtualWidth, VirtualHeight);
            Screen = new Rectangle(new Point(0, 0), new Point(Graphics.PreferredBackBufferWidth, Graphics.PreferredBackBufferHeight));
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {

            // Create a new SpriteBatch, which can be used to draw textures.
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            background1 = Content.Load<Texture2D>("Sprites/Assets/Background_1");
            font = Content.Load<SpriteFont>("Sprites/PixelFont");

            JObject FrameData = JObject.Parse(File.ReadAllText("Content/player_frame_data.json"));

            playerSpriteSet = new Dictionary<string, AnimatedSprite>();
            playerSpriteSet.Add("Main", new AnimatedSprite(
                Content.Load<Texture2D>("Sprites/PlatformerPack/Player/player"), FrameData));

            PlayerCharacter.Initialize(playerSpriteSet, playerSpriteSet.Keys.ElementAt(0));
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

            KeyboardState currentState = Keyboard.GetState();

            // Checks for initial key presses
            if (PreviousState.IsKeyUp(Keys.Left) && currentState.IsKeyDown(Keys.Left)) {
                PlayerCharacter.Flip();
                PlayerCharacter.PlayerState.Fire(State.CharTriggers.RUN_START);
            }
            if (PreviousState.IsKeyUp(Keys.Right) && currentState.IsKeyDown(Keys.Right)) {
                PlayerCharacter.Deflip();
                PlayerCharacter.PlayerState.Fire(State.CharTriggers.RUN_START);
            }

            //Checks for key releases
            if (PreviousState.IsKeyDown(Keys.Left) && currentState.IsKeyUp(Keys.Left)) {
                PlayerCharacter.PlayerState.Fire(State.CharTriggers.RUN_END);
            }
            if (PreviousState.IsKeyDown(Keys.Right) && currentState.IsKeyUp(Keys.Right)) {
                PlayerCharacter.PlayerState.Fire(State.CharTriggers.RUN_END);
            }

            PreviousState = currentState;

            // Checks for held keys
            if (currentState.IsKeyDown(Keys.Left)) {
                PlayerCharacter.Position.X -= 30 * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (currentState.IsKeyDown(Keys.Right)) {
                PlayerCharacter.Position.X += 30 * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            Score++;
            PlayerCharacter.Update(gameTime);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {

            // Draw to internal resolution render target.
            GraphicsDevice.SetRenderTarget(RenderTarget);
            GraphicsDevice.Clear(Color.DeepSkyBlue);
            Vector2 size = font.MeasureString("Score: " + Score);

            SpriteBatch.Begin
                (SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp,
                null, null, null, null);
            SpriteBatch.Draw(background1, new Rectangle(0, 0, VirtualWidth, VirtualHeight), Color.White);
            SpriteBatch.DrawString(font, "Score: " + Score, new Vector2(Window.ClientBounds.Width / 2 - size.X / 2, 50), Color.Black);
            PlayerCharacter.Draw(SpriteBatch);
            SpriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);
            SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp);
            SpriteBatch.Draw(RenderTarget, Screen, Color.White);
            SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
