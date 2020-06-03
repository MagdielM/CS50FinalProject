using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Game1 {
    class PlayerCharacter {
        public Vector2 position;
        public Vector2 moveSpeed = new Vector2(1.4f, 0);
        private Keys lastKey;
        private AnimationController playerAnimator;

        public PlayerCharacter(Vector2 spawnPoint) {
            position = spawnPoint;
            playerAnimator = new AnimationController();
        }

        public void Initialize() {
        }

        public void LoadInit(Dictionary<string, FrameDataLibrary.SpriteReference> spriteSet, string defaultAnimation) {
            playerAnimator.LoadInit(spriteSet, defaultAnimation);
            InputHandler.LatestHorizontalArrowKey.Subscribe(ManageHorizontalInput);
        }

        public void Update(GameTime gameTime) {
            playerAnimator.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch) {
            playerAnimator.Draw(spriteBatch, position);
        }

        public void ManageHorizontalInput(Keys key) {
            switch (key) {
                case Keys.Left:
                    position -= moveSpeed;
                    if (key != lastKey) {
                        Flip();
                        playerAnimator.SetTag("Run");
                    }
                    break;
                case Keys.Right:
                    position += moveSpeed;
                    if (key != lastKey) {
                        Deflip();
                        playerAnimator.SetTag("Run");
                    }
                    break;
                default:
                    if (key != lastKey) {
                        playerAnimator.SetTag("Idle");
                    }
                    break;
            }
            lastKey = key;
        }

        public void ManageVerticalInput(Keys key) {

        }

        public void Flip() {
            playerAnimator.SetCategory("flipped",
                playerAnimator.ActiveAnimation.FrameData.ActiveTagName,
                playerAnimator.ActiveAnimation.FrameData.ActiveTag.StartFrame);
        }

        public void Deflip() {
            playerAnimator.SetCategory("normal",
                playerAnimator.ActiveAnimation.FrameData.ActiveTagName,
                playerAnimator.ActiveAnimation.FrameData.ActiveTag.StartFrame);
        }
    }
}