using Microsoft.Xna.Framework;

namespace Game1 {
    internal class Camera {
        public Matrix Transform { get; internal set; }

        public void Follow(PlayerCharacter target) {
            Matrix offset = Matrix.CreateTranslation(MainGame.virtualWidth / 2, MainGame.virtualHeight / 2, 0);
            Matrix position = Matrix.CreateTranslation(
                -target.playerBounds.Location.X - (target.playerBounds.Width / 2),
                -target.playerBounds.Location.Y - (target.playerBounds.Height / 2),
                0);
            Transform = position * offset;
        }
    }
}
