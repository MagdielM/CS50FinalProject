using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1 {
    internal class Cursor {
        private readonly Texture2D cursorImage;
        private Vector2 virtualBounds = new Vector2(MainGame.virtualWidth, MainGame.virtualHeight);
        private Vector2 windowBounds = new Vector2(MainGame.preferredBackBufferWidth, MainGame.preferredBackBufferHeight);
        private Vector2 position = new Vector2(MainGame.virtualWidth / 2, MainGame.virtualHeight / 2);
        public bool isOutdated = true;

        public Cursor(Texture2D image) {
            cursorImage = image;
        }

        public void Update() {
            position = Vector2.Clamp(Mouse.GetState().Position.ToVector2() * (virtualBounds / windowBounds), Vector2.Zero, windowBounds);
            isOutdated = false;
        }

        public void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Draw(cursorImage, position, Color.White);
        }
    }
}
