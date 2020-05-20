using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace General {
    class PlayerCharacter {
        public Vector2 Position;

        AnimationController PlayerAnimator;

        public PlayerCharacter(Vector2 spawnPoint) {
            Position = spawnPoint;
            PlayerAnimator = new AnimationController();
        }

        public void Initialize() {
        }

        public void LoadInit(Dictionary<string, AnimatedSprite> spriteSet, string defaultAnimation) {
            PlayerAnimator.LoadInit(spriteSet, defaultAnimation);

        }
        public void Update(GameTime gameTime) {
            PlayerAnimator.Update(gameTime);
        }
        public void Draw(SpriteBatch spriteBatch) {
            PlayerAnimator.Draw(spriteBatch, Position);
        }
        public void Flip() {
            PlayerAnimator.SetTag(PlayerAnimator.activeAnim.ActiveTagName);
        }
        public void Deflip() {
            PlayerAnimator.SetTag(PlayerAnimator.activeAnim.ActiveTagName);
        }
    }
}