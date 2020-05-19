using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using State;
using Stateless;
using System.Collections.Generic;

namespace General {
    class PlayerCharacter {
        public Vector2 Position;
        public StateMachine<CharStates, CharTriggers> PlayerState;
        AnimationController PlayerAnimator;

        public PlayerCharacter(Vector2 spawnPoint) {
            Position = spawnPoint;
            PlayerAnimator = new AnimationController();
            Actions.IdleBehavior += () => PlayerAnimator.SetTag("Idle");
            Actions.RunBehavior += () => PlayerAnimator.SetTag("Run");
            Actions.JumpBehavior += () => PlayerAnimator.SetTag("Run");
            Actions.FallBehavior += () => PlayerAnimator.SetTag("Fall");
            PlayerState = StateMachines.PlayerStateMachine();
        }
        public void Initialize(Dictionary<string, AnimatedSprite> spriteSet, string defaultAnimation) {
            PlayerAnimator.Initialize(spriteSet, defaultAnimation);

        }
        public void Update(GameTime gameTime) {
            PlayerAnimator.Update(gameTime);
        }
        public void Draw(SpriteBatch spriteBatch) {
            PlayerAnimator.Draw(spriteBatch, Position);
        }
        public void Flip() {
            PlayerAnimator.shouldFlip = true;
            PlayerAnimator.SetTag(PlayerAnimator.activeAnim.ActiveTagName);
        }
        public void Deflip() {
            PlayerAnimator.shouldFlip = false;
            PlayerAnimator.SetTag(PlayerAnimator.activeAnim.ActiveTagName);
        }
    }
}