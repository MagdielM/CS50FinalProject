using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Game1 {
    class PlayerCharacter {
        public Vector2 Position;
        AnimationController PlayerAnimator;

        public PlayerCharacter(Vector2 spawnPoint) {
            Position = spawnPoint;
        }

        public void Initialize() {
        }

        public void LoadInit(Dictionary<string, FrameDataLibrary.SpriteReference> spriteSet, string defaultAnimation) {
            PlayerAnimator.LoadInit(spriteSet, defaultAnimation);
        }
        public void Update(GameTime gameTime) {
            PlayerAnimator.Update(gameTime);
        }
        public void Draw(SpriteBatch spriteBatch) {
            PlayerAnimator.Draw(spriteBatch, Position);
        }
        public void Flip() {
            PlayerAnimator.SetCategory("flipped", 
                PlayerAnimator.ActiveAnimation.FrameData.ActiveTagName, 
                PlayerAnimator.ActiveAnimation.FrameData.ActiveTag.StartFrame);
        }
        public void Deflip() {
            PlayerAnimator.SetCategory("normal",
                PlayerAnimator.ActiveAnimation.FrameData.ActiveTagName,
                PlayerAnimator.ActiveAnimation.FrameData.ActiveTag.StartFrame);
        }
    }
}