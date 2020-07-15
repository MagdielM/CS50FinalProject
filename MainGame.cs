using FrameDataLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SliceDataLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using static Game1.Canvas;

namespace Game1 {
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    class MainGame : Game {
        #region General References

        public const int virtualWidth = 384;
        public const int virtualHeight = 216;
        public const int preferredBackBufferWidth = 1280;
        public const int preferredBackBufferHeight = 720;
        public float fontHeight;
        private KeyboardState previousKeyboardState;
        private Rectangle screen;
        private Rectangle virtualBounds;
        private Vector2[] debugLinePositions;
        public SpriteBatch spriteBatch;
        readonly GraphicsDeviceManager graphics;
        private Texture2D blackScreenFilter;
        private RenderTarget2D renderTarget;
        public Cursor cursor;
        private Canvas pauseScreenCanvas;
        public static Subject<GlobalEvent> globalEventStream = new Subject<GlobalEvent>();
        private GlobalState globalState = GlobalState.Running;



        #endregion

        #region Game References

        public PlayerCharacter Player { get; private set; }
        private Camera camera;
        public static Vector2 risingGravity = new Vector2(0, 16f);
        public static Vector2 fallingGravity = new Vector2(0, 26f);
        public static Level currentLevel;

        #region Global States and Events

        public enum GlobalState {
            AtStart,
            Running,
            Paused,
            Ending
        }

        public enum GlobalEvent {
            Start,
            Resume,
            Pause,
            End
        }

        #endregion

        #endregion

        #region Content References

        Texture2D background1;
        private SpriteFont font;
        Dictionary<string, FrameReference> playerSpriteSet;
        private const string level1TilemapPath = "Sprites/PlatformerPack/raw files/tilemap";
        private const string defaultUiAtlasPath = "Sprites/PlatformerPack/raw files/ui_3x3";
        private const string defaultUiDataPath = "Sprites/PlatformerPack/raw files/ui_3x3_data";
        private const string cursorPath = "Sprites/PlatformerPack/raw files/cursor";
        private const string playerFrameDataPath = "Sprites/PlatformerPack/Player/player_frame_data";
        private const string background1Path = "Sprites/Assets/Background_1";
        private const string pixelFontPath = "Sprites/PixelFont";

        #region UI Resource Paths
        private const string lineHorizontal = "line_horizontal";
        private const string lineHorizontalStart = "line_horizontal_start";
        private const string lineHorizontalEnd = "line_horizontal_end";
        private const string lineVertical = "line_vertical";
        private const string lineVerticalStart = "line_vertical_start";
        private const string lineVerticalEnd = "line_vertical_end";
        private const string genericCorner = "corner";
        #endregion

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
            graphics.PreferredBackBufferWidth = preferredBackBufferWidth;
            graphics.PreferredBackBufferHeight = preferredBackBufferHeight;
            graphics.ApplyChanges();
            blackScreenFilter = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Color[] c = new Color[1];
            c[0] = Color.White * 0.55f;
            blackScreenFilter.SetData(c);
            renderTarget = new RenderTarget2D(GraphicsDevice, virtualWidth, virtualHeight);
            screen = new Rectangle(Point.Zero, new Point(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight));
            virtualBounds = new Rectangle(Point.Zero, new Point(virtualWidth, virtualHeight));
            camera = new Camera();
            previousKeyboardState = Keyboard.GetState();
            globalEventStream.Subscribe(ReactToEvent);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            background1 = Content.Load<Texture2D>(background1Path);
            font = Content.Load<SpriteFont>(pixelFontPath);
            fontHeight = font.MeasureString("A").Y;

            SliceReference uiTexture = new SliceReference(
                Content.Load<Texture2D>(defaultUiAtlasPath),
                Content.Load<SliceData>(defaultUiDataPath));

            pauseScreenCanvas = new Canvas(ref uiTexture);
            pauseScreenCanvas.AddChild(
                new BoxElement(
                    ref uiTexture, new Rectangle(0, 0, 96, 128),
                    new LineElement(
                        ref uiTexture, false,
                        new RectangleElement(
                            ref uiTexture, lineHorizontal),
                        new RectangleElement(
                            ref uiTexture, lineHorizontalStart),
                        new RectangleElement(
                            ref uiTexture, lineHorizontalEnd)),
                    new LineElement(
                        ref uiTexture, true,
                        new RectangleElement(
                            ref uiTexture, lineVertical),
                        new RectangleElement(
                            ref uiTexture, lineVerticalStart),
                        new RectangleElement(
                            ref uiTexture, lineVerticalEnd)),
                    new RectangleElement(
                        ref uiTexture, genericCorner)));
            pauseScreenCanvas.Children[0].Centralize();
            
            cursor = new Cursor(Content.Load<Texture2D>(cursorPath));

            debugLinePositions = new Vector2[] {
                Vector2.Zero,
                new Vector2(0, fontHeight),
                new Vector2(0, fontHeight * 2),
                new Vector2(0, fontHeight * 3),
                new Vector2(0, fontHeight * 4),
                new Vector2(0, fontHeight * 5),
                new Vector2(0, fontHeight * 6)
            };
            currentLevel = new Level(Content.Load<Texture2D>(level1TilemapPath), LevelDefinitions.level1Tiles);

            var playerData = Content.Load<FrameData>(playerFrameDataPath);
            playerSpriteSet = new Dictionary<string, FrameReference> {
                {
                    "Main",
                    new FrameReference(
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
            KeyboardState currentKeyboardState = Keyboard.GetState();
            switch (globalState) {
                case GlobalState.Running:
                    Player.Update(gameTime);
                    foreach (Keys key in InputHandler.InputOutKeys.Value) {
                        Player.ManageGeneralInput(key);
                    }
                    camera.Follow(Player);
                    InputHandler.Update();
                    if (currentKeyboardState.IsKeyDown(Keys.Escape) && previousKeyboardState.IsKeyUp(Keys.Escape)) {
                        globalEventStream.OnNext(GlobalEvent.Pause);
                    }
                    base.Update(gameTime);
                    break;

                case GlobalState.Paused:
                    cursor.Update();
                    if (currentKeyboardState.IsKeyDown(Keys.Escape) && previousKeyboardState.IsKeyUp(Keys.Escape)) {
                        globalEventStream.OnNext(GlobalEvent.Resume);
                    }
                    break;
            }
            previousKeyboardState = currentKeyboardState;
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

            if (globalState == GlobalState.Paused) {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp,
                    null, null, null, null);
                spriteBatch.Draw(blackScreenFilter, virtualBounds, Color.Black);
                pauseScreenCanvas.Draw(spriteBatch);
                if (!cursor.isOutdated) {
                    cursor.Draw(spriteBatch);
                }
                spriteBatch.End();
            }

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

        private void ReactToEvent(GlobalEvent globalEvent) {
            switch (globalEvent) {
                case GlobalEvent.Pause:
                    previousKeyboardState = Keyboard.GetState();
                    globalState = GlobalState.Paused;
                    break;

                case GlobalEvent.Resume:
                    cursor.isOutdated = true;
                    globalState = GlobalState.Running;
                    break;
            }
        }
    }
}
