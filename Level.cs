using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1 {
    class Level {
        #region Tile Codes and Definitions
        public enum TileCodes {
            Empty,
            Platform,
            PlatformLeftCorner,
            PlatformRightCorner,
            Ground,
            GroundLeftCorner,
            GroundRightCorner,
            GroundStone,
            DeepGroundLeftCorner,
            DeepGround,
            DeepGroundRightCorner
        }

        public Dictionary<TileCodes, Rectangle> TileDefinitions = new Dictionary<TileCodes, Rectangle>() {
            [TileCodes.Empty] = new Rectangle(0, 80, 32, 32),
            [TileCodes.PlatformLeftCorner] = new Rectangle(128, 96, 32, 32),
            [TileCodes.Platform] = new Rectangle(160, 96, 32, 32),
            [TileCodes.PlatformRightCorner] = new Rectangle(224, 96, 32, 32),
            [TileCodes.GroundLeftCorner] = new Rectangle(224, 160, 32, 32),
            [TileCodes.Ground] = new Rectangle(0, 160, 32, 32),
            [TileCodes.GroundRightCorner] = new Rectangle(96, 160, 32, 32),
            [TileCodes.DeepGroundLeftCorner] = new Rectangle(224, 192, 32, 32),
            [TileCodes.DeepGround] = new Rectangle(0, 192, 32, 32),
            [TileCodes.DeepGroundRightCorner] = new Rectangle(96, 192, 32, 32)
        };
        #endregion

        public Texture2D levelAtlas;

        public List<List<Tile>> TileArray { get; private set; } = new List<List<Tile>>();
        public Level(Texture2D atlas, TileCodes[,] tileArray) {
            levelAtlas = atlas;
            for (int i = 0; i < tileArray.GetLength(0); i++) {
                TileArray.Insert(i, new List<Tile>());
                for (int j = 0; j < tileArray.GetLength(1); j++) {
                    Point position = new Point(32 * j, 32 * i);
                    TileArray[i].Insert(j, GenerateTile(tileArray[i, j], position));
                }
            }
        }

        public Tile GenerateTile(TileCodes tileCode, Point position) {
            return new Tile(tileCode, position, ref levelAtlas, TileDefinitions[tileCode]);
        }

        public void Draw(SpriteBatch spriteBatch) {
            foreach (List<Tile> tileRow in TileArray) {
                foreach(Tile tile in tileRow) {
                    tile.Draw(spriteBatch);
                }
            }
        }
    }
}
