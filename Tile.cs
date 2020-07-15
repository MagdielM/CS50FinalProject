using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RectangleFLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1 {
    class Tile {
        public Level.TileCodes TileCode { get; }
        public Point Position { get; }
        public RectangleF tileBounds;
        private Rectangle tileAtlasCoordinates;
        private Texture2D tileSprite;

        public Tile(Level.TileCodes tileCode, Point position, ref Texture2D sprite, Rectangle coordinates) {
            TileCode = tileCode;
            Position = position;
            tileBounds = new RectangleF(new Rectangle(position, new Point(32)));
            tileSprite = sprite;
            tileAtlasCoordinates = coordinates;
        }

        public void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Draw(tileSprite, tileBounds, tileAtlasCoordinates, Color.White);
        }
    }
}
