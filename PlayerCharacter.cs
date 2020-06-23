using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RectangleFLib;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Game1 {
    class PlayerCharacter {
        public Vector2 position;
        public Vector2 movementVector = new Vector2(0f, 0f);
        public Vector2 adjustedMovementVector = new Vector2(0f, 0f);
        private Vector2 moveSpeed = new Vector2(200f, 0f);
        private Vector2 jumpStrength = new Vector2(0f, 330f);
        public Tile landingTile;
        public int remainingJumps = 1;
        public RectangleF playerBounds;
        private Keys lastHorizontalArrowKey;
        private Keys lastVerticalArrowKey;
        public bool isCollidingHorizontally;
        public bool isCollidingVertically;
        private CharacterAnimationController playerAnimator;
        private Subject<PlayerEvent> playerEventStream;
        public BehaviorSubject<PlayerState> PlayerStateStream { get; set; }

        public PlayerCharacter(Point spawnPoint) {
            PlayerStateStream = new BehaviorSubject<PlayerState>(PlayerState.OnGround);
            playerEventStream = new Subject<PlayerEvent>();
            position = spawnPoint.ToVector2();
            playerAnimator = new CharacterAnimationController();
            playerEventStream.Subscribe(ReactToEvent);
        }

        private void ReactToEvent(PlayerEvent playerEvent) {
            switch (PlayerStateStream.Value) {
                case PlayerState.OnGround:
                    switch (playerEvent) {
                        case PlayerEvent.Jumped:
                            movementVector.Y = 0;
                            movementVector -= jumpStrength;
                            PlayerStateStream.OnNext(PlayerState.InAir);
                            break;
                    }
                    break;
                case PlayerState.InAir:
                    switch (playerEvent) {
                        case PlayerEvent.Jumped:
                            remainingJumps -= 1;
                            movementVector.Y = 0;
                            movementVector -= jumpStrength;
                            break;
                        case PlayerEvent.CollidedWithGround:
                            remainingJumps = 1;
                            PlayerStateStream.OnNext(PlayerState.OnGround);
                            break;
                    }
                    break;
            }
        }

        public void LoadInit(Dictionary<string, FrameDataLibrary.SpriteReference> spriteSet, string defaultAnimation) {
            playerAnimator.LoadInit(spriteSet, defaultAnimation);
            playerBounds = new RectangleF(new Rectangle(position.ToPoint(), spriteSet["Main"].FrameData.SpriteDimensions.ToPoint()));
            InputHandler.LatestHorizontalArrowKey.Subscribe(ManageHorizontalInput);
            InputHandler.LatestVerticalArrowKey.Subscribe(ManageVerticalInput);
        }

        public void Update(GameTime gameTime) {
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
                PlayerStateStream.OnNext(PlayerState.InAir);
            }
            playerAnimator.Update(gameTime, PlayerStateStream.Value, movementVector, isCollidingHorizontally);
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
            foreach (List<Tile> tileRow in MainGame.currentLevel.TileArray) {
                foreach (Tile tile in tileRow) {
                    if (playerBounds.Intersects(tile.tileBounds) && tile.TileCode != Level.TileCodes.Empty) {
                        movementVector.Y = 0;
                        playerBounds.Offset(-verticalVector);
                        if (verticalVector.Y >= 0) {
                            landingTile = tile;
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
            }
            return false;
        }

        private bool MoveHorizontally(Vector2 horizontalVector) {
            playerBounds.Offset(horizontalVector);
            foreach (List<Tile> tileRow in MainGame.currentLevel.TileArray) {
                foreach (Tile tile in tileRow) {
                    if (playerBounds.Intersects(tile.tileBounds) && tile.TileCode != Level.TileCodes.Empty
                        && (playerBounds.Bottom > tile.tileBounds.Top)) {
                        playerBounds.Offset(-horizontalVector);
                        playerEventStream.OnNext(PlayerEvent.CollidedWithWall);
                        return true;
                    }
                }
            }
            return false;
        }

        #region Player States and Events

        public enum PlayerState {
            OnGround,
            InAir,
        }
        public enum PlayerEvent {
            Jumped,
            CollidedWithWall,
            CollidedWithGround,
            CollidedWithCeiling
        }

        #endregion
    }
}