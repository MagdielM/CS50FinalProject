using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RectangleFLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Game1 {
    class PlayerCharacter {
        public Vector2 position;
        public Vector2 movementVector = new Vector2(0f, 0f);
        public Vector2 adjustedMovementVector = new Vector2(0f, 0f);
        private Vector2 moveSpeed = new Vector2(200f, 0f);
        private Vector2 jumpStrength = new Vector2(0f, 330f);
        private int remainingJumps = 1;
        public List<Tile> surroundingTiles;
        public RectangleF playerBounds;
        private Keys lastHorizontalArrowKey;
        private Keys lastVerticalArrowKey;
        public bool isCollidingHorizontally;
        public bool isCollidingVertically;
        public PlayerAnimator playerAnimator;
        private Subject<PlayerEvent> playerEventStream;
        public PlayerState playerState { get; set; }
        private Keys[] lastGeneralKeyArray;

        public PlayerCharacter(Point spawnPoint) {
            playerEventStream = new Subject<PlayerEvent>();
            position = spawnPoint.ToVector2();
            surroundingTiles = new List<Tile>();
            playerAnimator = new PlayerAnimator();
            playerEventStream.Subscribe(ReactToEvent);
            lastGeneralKeyArray = Array.Empty<Keys>();
        }

        private void ReactToEvent(PlayerEvent playerEvent) {
            switch (playerState) {
                case PlayerState.OnGround:
                    switch (playerEvent) {
                        case PlayerEvent.Jumped:
                            movementVector.Y = 0;
                            movementVector -= jumpStrength;
                            playerState = PlayerState.InAir;
                            break;
                    }
                    break;
                case PlayerState.InAir:
                    switch (playerEvent) {
                        case PlayerEvent.Jumped:
                            remainingJumps--;
                            movementVector.Y = 0;
                            movementVector -= jumpStrength;
                            break;
                        case PlayerEvent.CollidedWithGround:
                            remainingJumps = 1;
                            playerState = PlayerState.OnGround;
                            break;
                    }
                    break;
            }
        }

        public void LoadInit(Dictionary<string, FrameDataLibrary.FrameReference> spriteSet, string defaultAnimation) {
            playerAnimator.LoadInit(spriteSet, defaultAnimation);
            playerBounds = new RectangleF(new Rectangle(position.ToPoint(), spriteSet["Main"].FrameData.SpriteDimensions.ToPoint()));
            InputHandler.LatestHorizontalArrowKey.Subscribe(ManageHorizontalInput);
            InputHandler.LatestVerticalArrowKey.Subscribe(ManageVerticalInput);
        }

        public void Update(GameTime gameTime) {
            DetectSurroundingTiles();
            adjustedMovementVector = new Vector2(
                movementVector.X * (float)gameTime.ElapsedGameTime.TotalSeconds,
                movementVector.Y * (float)gameTime.ElapsedGameTime.TotalSeconds);
            if (movementVector.Y < 0) {
                movementVector += MainGame.risingGravity;
            }
            else {
                movementVector += MainGame.fallingGravity;
            }
            Move();
            if (!isCollidingVertically) {
                playerState = PlayerState.InAir;
            }
            playerAnimator.Update(gameTime, playerState, movementVector, isCollidingHorizontally);
        }

        private void DetectSurroundingTiles() {
            foreach (Tile tile in MainGame.currentLevel.TileArray) {
                if (tile.tileBounds.Left <= playerBounds.Right + 196 && tile.tileBounds.Right >= playerBounds.Left - 196 &&
                    tile.tileBounds.Top <= playerBounds.Bottom + 128 && tile.tileBounds.Bottom >= playerBounds.Top - 128) {
                    if (!surroundingTiles.Contains(tile)) {
                        surroundingTiles.Add(tile);
                    }
                }
                else {
                    surroundingTiles.Remove(tile);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch) {
            playerAnimator.Draw(spriteBatch, General.RoundRectanglePoints(playerBounds));
        }

        public void ManageHorizontalInput(Keys key) {
            switch (key) {
                case Keys.Left:
                    if (key != lastHorizontalArrowKey) {
                        movementVector.X = 0;
                        movementVector -= moveSpeed;
                    }
                    break;
                case Keys.Right:
                    if (key != lastHorizontalArrowKey) {
                        movementVector.X = 0;
                        movementVector += moveSpeed;
                    }
                    break;
                default:
                    movementVector.X = 0;
                    break;
            }
            lastHorizontalArrowKey = key;

        }

        public void ManageVerticalInput(Keys key) {
            switch (key) {
                case Keys.Up:
                    if (key != lastVerticalArrowKey && remainingJumps > 0) {
                        playerEventStream.OnNext(PlayerEvent.Jumped);
                    }
                    break;
                case Keys.None:
                    if (playerState == PlayerState.InAir && movementVector.Y < 0) {
                        movementVector.Y = 0;
                    }
                    break;
            }
            lastVerticalArrowKey = key;
        }

        public void ManageGeneralInput(Keys key) {
        }

        private void Move() {
            Vector2 horizontalMovementVector = new Vector2(adjustedMovementVector.X, 0);
            Vector2 verticalMovementVector = new Vector2(0, adjustedMovementVector.Y);
            isCollidingHorizontally = MoveHorizontally(horizontalMovementVector);
            isCollidingVertically = MoveVertically(verticalMovementVector);
        }

        private bool MoveVertically(Vector2 verticalVector) {
            playerBounds.Offset(verticalVector);
            foreach (Tile tile in surroundingTiles) {
                if (playerBounds.Intersects(tile.tileBounds) && tile.TileCode != Level.TileCodes.Empty) {
                    movementVector.Y = 0;
                    playerBounds.Offset(-verticalVector);
                    if (verticalVector.Y >= 0) {
                        Tile landingTile = tile;
                        playerBounds.Offset(new Vector2(0, landingTile.tileBounds.Top - playerBounds.Bottom));
                        playerEventStream.OnNext(PlayerEvent.CollidedWithGround);
                        return true;
                    }
                    else if (verticalVector.Y < 0) {
                        playerEventStream.OnNext(PlayerEvent.CollidedWithCeiling);
                        return true;
                    }
                }
            }
            return false;
        }

        private bool MoveHorizontally(Vector2 horizontalVector) {
            playerBounds.Offset(horizontalVector);
            foreach (Tile tile in MainGame.currentLevel.TileArray) {
                if (playerBounds.Intersects(tile.tileBounds) && tile.TileCode != Level.TileCodes.Empty
                    && (playerBounds.Bottom > tile.tileBounds.Top)) {
                    playerBounds.Offset(-horizontalVector);
                    playerEventStream.OnNext(PlayerEvent.CollidedWithWall);
                    return true;
                }
            }
            return false;
        }

        #region Player States and Events

        public enum PlayerState {
            OnGround,
            InAir,
            Dead
        }
        public enum PlayerEvent {
            Jumped,
            CollidedWithWall,
            CollidedWithGround,
            CollidedWithCeiling,
            Died
        }

        #endregion
    }
}