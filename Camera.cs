using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1 {
    class Camera {
        public Matrix Transform { get; private set; }

        public void Follow(PlayerCharacter target) {
            Matrix offset = Matrix.CreateTranslation(MainGame.virtualWidth / 2, MainGame.virtualHeight / 2, 0);
            Matrix position = Matrix.CreateTranslation(
                -target.playerBounds.Location.X - target.playerBounds.Width / 2, 
                -target.playerBounds.Location.Y - target.playerBounds.Height / 2, 
                0);
            Transform = position * offset;
        }
    }
}
