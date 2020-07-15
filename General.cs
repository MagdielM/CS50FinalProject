using Microsoft.Xna.Framework;
using RectangleFLib;
using System;
using static Game1.Canvas;

namespace Game1 {
    public static class General {
        public const CanvasElement nullCanvasElement = null;

        public static Rectangle RoundRectanglePoints(RectangleF r) {
            return new Rectangle((int)Math.Round(r.X), (int)Math.Round(r.Y), (int)Math.Round(r.Width), (int)Math.Round(r.Height));
        }
    }
}
