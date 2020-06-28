using Microsoft.Xna.Framework;
using System;
using static Game1.PlayerCharacter;

namespace Game1 {
    class PlayerAnimator : Animator {

        public void Update(GameTime gameTime, PlayerState playerState, Vector2 movementVector, bool isCollidingHorizontally) {
            UpdateCurrentAnimation(playerState, movementVector, isCollidingHorizontally);
            base.Update(gameTime);
        }

        private void UpdateCurrentAnimation(PlayerState playerState, Vector2 movementVector, bool isCollidingHorizontally) {
            switch (playerState) {
                case PlayerState.OnGround:
                    if (movementVector.X < 0 && !isCollidingHorizontally) {
                        if (ActiveCategory != "flipped") {
                            SetCategory("flipped", "Run");
                        }
                        if (ActiveTag != "Run") {
                            SetTag("Run");
                        }
                    }
                    else if (movementVector.X > 0 && !isCollidingHorizontally) {
                        if (ActiveCategory != "normal") {
                            SetCategory("normal", "Run");
                        }
                        if (ActiveTag != "Run") {
                            SetTag("Run");
                        }
                    }
                    else if (ActiveTag != "Idle") {
                        SetTag("Idle");
                    }
                    break;

                case PlayerState.InAir:
                    if (movementVector.X < 0 && ActiveCategory != "flipped") {
                        SetCategory("flipped", ActiveTag);
                    }
                    else if (movementVector.X > 0 && ActiveCategory != "normal") {
                        SetCategory("normal", ActiveTag);
                    }
                    if (movementVector.Y < 0) {
                        if (ActiveTag != "Jump Ascent" && ActiveTag != "Jump Ascending") {
                            SetTag("Jump Ascent");
                        }
                        else if (ActiveTag == "Jump Ascent" &&
                            frameToDraw >= SpriteSet["Main"].FrameData.CategorizedTags[ActiveCategory]["Jump Ascent"].EndFrame) {
                            SetTag("Jump Ascending");
                        }
                    }
                    else if (movementVector.Y > 0) {
                        if (ActiveTag != "Jump Descent" && ActiveTag != "Jump Descending") {
                            SetTag("Jump Descent");
                        }
                        else if (ActiveTag == "Jump Descent" &&
                            frameToDraw >= SpriteSet["Main"].FrameData.CategorizedTags[ActiveCategory]["Jump Descent"].EndFrame) {
                            SetTag("Jump Descending");
                        }
                    }
                    break;
            }
        }
    }
}
