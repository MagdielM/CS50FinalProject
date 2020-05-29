using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Game1 {
    class PlayerCharacter {
        public Vector2 position;
        public Vector2 moveSpeed = new Vector2(1, 0);
        private AnimationController playerAnimator;

        public PlayerCharacter(Vector2 spawnPoint) {
            position = spawnPoint;
        }

        public void Initialize() {
        }

        public void LoadInit(Dictionary<string, FrameDataLibrary.SpriteReference> spriteSet, string defaultAnimation) {
            playerAnimator.LoadInit(spriteSet, defaultAnimation);
        }

        public void Update(GameTime gameTime) {
            playerAnimator.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch) {
            playerAnimator.Draw(spriteBatch, position);
        }

        public void ManageInput(Keys key) {
            switch (key) {
                case Keys.Left:
                    position -= moveSpeed;
                    break;
                case Keys.Right:
                    position += moveSpeed;
                    break;
            }
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