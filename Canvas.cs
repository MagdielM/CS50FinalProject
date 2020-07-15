using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using PostSharp.Patterns.Contracts;
using SliceDataLibrary;

namespace Game1 {
    public class Canvas {
        public Rectangle RootBounds { get => Root.ElementBounds; }
        public List<CanvasElement> Children { get => Root.Children; }
        public RootElement Root { get; set; }

        public Canvas(ref SliceReference canvasTexture, List<CanvasElement> hierarchy = null) {
            Root = new RootElement(ref canvasTexture, hierarchy);
        }

        #region Hierarchy Management
        public void AddChild<T>(T child) where T : CanvasElement => Root.AddChild(child);

        public void PrependChild<T>(T child) where T : CanvasElement => Root.PrependChild(child);

        public void RemoveChild<T>(T child) where T : CanvasElement => Root.RemoveChild(child);
        public void RemoveChild(int index) => Root.RemoveChild(index);
        #endregion

        public void Draw(SpriteBatch spriteBatch) {
            Root.Draw(spriteBatch);
        }

        public bool PerformClick(Point hit) {
            return Root.PerformClick(hit);
        }

        public class CanvasElement {
            public SliceReference CanvasTexture { get; protected set; }
            public Rectangle ElementBounds { get => elementBounds; set => elementBounds = value; }
            public Point Location { get => elementBounds.Location; set => elementBounds.Location = value; }
            public Point Center { get => elementBounds.Center; }
            public Point Size { get => elementBounds.Size; }
            public int X { get => elementBounds.X; set => elementBounds.X = value; }
            public int Y { get => elementBounds.Y; set => elementBounds.Y = value; }
            public int Height { get => elementBounds.Height; set => elementBounds.Height = value; }
            public int Width { get => elementBounds.Width; set => elementBounds.Width = value; }
            public int Top { get => elementBounds.Top; }
            public int Bottom { get => elementBounds.Bottom; }
            public int Left { get => elementBounds.Left; }
            public int Right { get => elementBounds.Right; }
            public List<CanvasElement> Children { get; }
            public CanvasElement ParentElement { get; protected set; }
            internal Rectangle elementBounds;

            #region Constructors
            public CanvasElement([Required] ref SliceReference canvasTexture,
                                 [NonEmpty] Rectangle elementBounds,
                                 List<CanvasElement> nestedElements = null) {
                this.CanvasTexture = canvasTexture;
                this.elementBounds = elementBounds;
                Children = nestedElements ?? new List<CanvasElement>();
            }

            public CanvasElement([Required] ref SliceReference canvasTexture,
                                 Point origin,
                                 [NonEmpty] Point size,
                                 List<CanvasElement> nestedElements = null) {
                this.CanvasTexture = canvasTexture;
                elementBounds = new Rectangle(origin, size);
                Children = nestedElements ?? new List<CanvasElement>();
            }

            public CanvasElement([Required] ref SliceReference canvasTexture,
                                 Point origin,
                                 List<CanvasElement> nestedElements = null) {
                this.CanvasTexture = canvasTexture;
                elementBounds = new Rectangle(origin, Point.Zero);
                Children = nestedElements ?? new List<CanvasElement>();
            }

            public CanvasElement([Required] ref SliceReference canvasTexture,
                                 List<CanvasElement> nestedElements = null) {
                this.CanvasTexture = canvasTexture;
                elementBounds = new Rectangle(Point.Zero, Point.Zero);
                Children = nestedElements ?? new List<CanvasElement>();
            }

            public CanvasElement(CanvasElement parentElement) {
                ParentElement = parentElement;
            }

            public CanvasElement(CanvasElement parentElement,
                                 [Required] ref SliceReference canvasTexture,
                                 [NonEmpty] Rectangle elementBounds,
                                 List<CanvasElement> nestedElements = null) {
                CanvasTexture = canvasTexture;
                this.elementBounds = elementBounds;
                ParentElement = parentElement;
                Children = nestedElements ?? new List<CanvasElement>();
            }

            public CanvasElement(CanvasElement parentElement,
                                 [Required] ref SliceReference canvasTexture,
                                 Point origin,
                                 [NonEmpty] Point size,
                                 List<CanvasElement> nestedElements = null) {
                CanvasTexture = canvasTexture;
                elementBounds = new Rectangle(origin, size);
                ParentElement = parentElement;
                Children = nestedElements ?? new List<CanvasElement>();
            }

            public CanvasElement(CanvasElement parentElement,
                                 [Required] ref SliceReference canvasTexture,
                                 [NonEmpty] Point origin,
                                 List<CanvasElement> nestedElements = null) {
                CanvasTexture = canvasTexture;
                elementBounds = new Rectangle(origin, Point.Zero);
                ParentElement = parentElement;
                Children = nestedElements ?? new List<CanvasElement>();
            }

            public CanvasElement(CanvasElement parentElement,
                                 [Required] ref SliceReference canvasTexture,
                                 List<CanvasElement> nestedElements = null) {
                CanvasTexture = canvasTexture;
                elementBounds = new Rectangle(Point.Zero, Point.Zero);
                ParentElement = parentElement;
                Children = nestedElements ?? new List<CanvasElement>();
            }
            #endregion Constructors

            #region Hierarchy Management

            public void AddChild<T>([Required] T child) where T : CanvasElement {
                Children.Add(child);
                child.ParentElement = this;
                Point relativeLocation = child.ElementBounds.Location;
                child.elementBounds.Location = Location;
                child.Offset(relativeLocation);
            }

            public void PrependChild<T>([Required] T child) where T : CanvasElement {
                Children.Insert(0, child);
                child.ParentElement = this;
                Point relativeLocation = child.ElementBounds.Location;
                child.elementBounds.Location = Location;
                child.Offset(relativeLocation);
            }

            #region RemoveChild()

            public void RemoveChild<T>([Required] T child) where T : CanvasElement {
                Children.Remove(child);
            }

            public void RemoveChild(int index) {
                Children.RemoveAt(index);
            }

            #endregion RemoveChild()

            #endregion Hierarchy Management

            #region Positioning

            #region Inflate()

            public void Inflate(float x = 0, float y = 0) {
                ElementBounds.Inflate(x, y);
            }

            public void Inflate(int x = 0, int y = 0) {
                ElementBounds.Inflate(x, y);
            }

            #endregion Inflate()

            #region Offset()

            public void Offset(Point amount) {
                ElementBounds.Offset(amount);
            }

            public void Offset(Vector2 amount) {
                ElementBounds.Offset(amount);
            }

            public void Offset(int x = 0, int y = 0) {
                ElementBounds.Offset(x, y);
            }

            public void Offset(float x = 0, float y = 0) {
                ElementBounds.Offset(x, y);
            }

            #endregion Offset()

            public void Reposition(Point position) {
                Point offset = position - Location;
                Location = position;
                foreach (CanvasElement element in Children) {
                    element.Reposition(position + offset);
                }
            }

            public virtual void Centralize() {
                if (ParentElement != null) {
                    X = ParentElement.Center.X - Center.X;
                    Y = ParentElement.Center.Y - Center.Y;
                }
                else {
                    Offset(Point.Zero);
                }
            }

            #endregion Positioning

            public virtual void Initialize(ref SliceReference canvasTexture,
                [NonEmpty] Rectangle elementBounds) {
                this.CanvasTexture = canvasTexture;
                ElementBounds = elementBounds;
            }

            public virtual void Draw(SpriteBatch spriteBatch) {
                if (Children?.Count > 0) {
                    foreach (CanvasElement nestedElement in Children) {
                        nestedElement.Draw(spriteBatch);
                    }
                }
            }

            public virtual bool PerformClick(Point hit) {
                if (ElementBounds.Contains(hit) && Children != null) {
                    foreach (CanvasElement nestedElement in Children) {
                        nestedElement.PerformClick(hit);
                    }
                }
                return false;
            }
        }

        public class RootElement : CanvasElement {
            #region Constructors

            public RootElement([Required] ref SliceReference canvasTexture, List<CanvasElement> hierarchy = null)
                : base(General.nullCanvasElement, ref canvasTexture, Point.Zero,
                    new Point(MainGame.virtualWidth, MainGame.virtualHeight), hierarchy) { }

            #endregion Constructors

            public List<CanvasElement> Hierarchy { get => Children; }
        }

        public class EmptyElement : CanvasElement {
            #region Constructors

            public EmptyElement(ref SliceReference canvasTexture,
                                Rectangle elementBounds,
                                List<CanvasElement> hierarchy = null)
                                : base(ref canvasTexture, elementBounds, hierarchy) { }

            public EmptyElement(ref SliceReference canvasTexture,
                                Point origin,
                                Point size,
                                List<CanvasElement> hierarchy = null)
                                : base(ref canvasTexture, origin, size, hierarchy) { }

            #endregion Constructors
        }

        public class RectangleElement : CanvasElement {
            internal Rectangle sourceRectangle;

            #region Constructors

            public RectangleElement([Required] RectangleElement previous)
                                    : base(previous.ParentElement) {
                CanvasTexture = previous.CanvasTexture;
                ElementBounds = previous.elementBounds;
                sourceRectangle = previous.sourceRectangle;
            }

            public RectangleElement([Required] ref SliceReference canvasTexture,
                                    [Required] string rectangleSlice,
                                    List<CanvasElement> hierarchy = null)
                                    : base(ref canvasTexture, hierarchy) {
                sourceRectangle = canvasTexture.SliceData.Slices[rectangleSlice];
                elementBounds.Width = sourceRectangle.Width;
                elementBounds.Height = sourceRectangle.Height;
            }

            public RectangleElement([Required] ref SliceReference canvasTexture,
                                    Point origin,
                                    [Required] string rectangleSlice,
                                    List<CanvasElement> hierarchy = null)
                                    : base(ref canvasTexture, origin, hierarchy) {
                sourceRectangle = canvasTexture.SliceData.Slices[rectangleSlice];
                elementBounds.Width = sourceRectangle.Width;
                elementBounds.Height = sourceRectangle.Height;
            }

            public RectangleElement(CanvasElement parentElement,
                                    [Required] ref SliceReference canvasTexture,
                                    [Required] string rectangleSlice,
                                    List<CanvasElement> hierarchy = null)
                                    : base(parentElement, ref canvasTexture, hierarchy) {
                sourceRectangle = canvasTexture.SliceData.Slices[rectangleSlice];
                elementBounds.Width = sourceRectangle.Width;
                elementBounds.Height = sourceRectangle.Height;
            }

            public RectangleElement(CanvasElement parentElement,
                                    [Required] ref SliceReference canvasTexture,
                                    Point origin,
                                    [Required] string rectangleSlice,
                                    List<CanvasElement> hierarchy = null)
                                    : base(parentElement, ref canvasTexture, origin, hierarchy) {
                sourceRectangle = canvasTexture.SliceData.Slices[rectangleSlice];
                elementBounds.Width = sourceRectangle.Width;
                elementBounds.Height = sourceRectangle.Height;
            }

            #endregion Constructors

            public override void Draw(SpriteBatch spriteBatch) {
                spriteBatch.Draw(CanvasTexture.Sprite, ElementBounds, sourceRectangle, Color.White);
                base.Draw(spriteBatch);
            }

            public void Draw(SpriteBatch spriteBatch, Point origin) {
                spriteBatch.Draw(CanvasTexture.Sprite, origin.ToVector2(), sourceRectangle: sourceRectangle, Color.White);
                base.Draw(spriteBatch);
            }
        }

        public class LineElement : CanvasElement {
            private readonly bool isVertical;
            private readonly RectangleElement line;
            private readonly RectangleElement lineEnd;
            private readonly RectangleElement lineStart;

            #region Constructors

            public LineElement([Required] ref SliceReference canvasTexture,
                               bool isVertical,
                               [Required] RectangleElement line,
                               RectangleElement lineStart = null,
                               RectangleElement lineEnd = null,
                               List<CanvasElement> hierarchy = null)
                               : base(ref canvasTexture, hierarchy) {
                this.isVertical = isVertical;
                this.line = line;
                this.lineStart = lineStart;
                this.lineEnd = lineEnd;
            }

            public LineElement([Required] ref SliceReference canvasTexture,
                               Point origin,
                               bool isVertical,
                               [Required] RectangleElement line,
                               RectangleElement lineStart = null,
                               RectangleElement lineEnd = null,
                               List<CanvasElement> hierarchy = null)
                               : base(ref canvasTexture, origin, hierarchy) {
                this.isVertical = isVertical;
                this.line = line;
                this.lineStart = lineStart;
                this.lineEnd = lineEnd;
            }

            public LineElement([Required] ref SliceReference canvasTexture,
                               Point origin,
                               [NonEmpty] Point size,
                               bool isVertical,
                               [Required] RectangleElement line,
                               RectangleElement lineStart = null,
                               RectangleElement lineEnd = null,
                               List<CanvasElement> hierarchy = null)
                               : base(ref canvasTexture, origin, size, hierarchy) {
                this.isVertical = isVertical;
                this.line = line;
                this.lineStart = lineStart;
                this.lineEnd = lineEnd;
            }

            public LineElement(CanvasElement parentElement,
                               [Required] ref SliceReference canvasTexture,
                               bool isVertical,
                               [Required] RectangleElement line,
                               RectangleElement lineStart = null,
                               RectangleElement lineEnd = null,
                               List<CanvasElement> hierarchy = null)
                               : base(parentElement, ref canvasTexture, hierarchy) {
                this.isVertical = isVertical;
                this.line = line;
                this.lineStart = lineStart;
                this.lineEnd = lineEnd;
            }

            public LineElement(CanvasElement parentElement,
                               [Required] ref SliceReference canvasTexture,
                               Point origin,
                               bool isVertical,
                               [Required] RectangleElement line,
                               RectangleElement lineStart = null,
                               RectangleElement lineEnd = null,
                               List<CanvasElement> hierarchy = null)
                               : base(parentElement, ref canvasTexture, origin, hierarchy) {
                this.isVertical = isVertical;
                this.line = line;
                this.lineStart = lineStart;
                this.lineEnd = lineEnd;
            }

            public LineElement(CanvasElement parentElement,
                               [Required] ref SliceReference canvasTexture,
                               Point origin,
                               [NonEmpty] Point size,
                               bool isVertical,
                               [Required] RectangleElement line,
                               RectangleElement lineStart = null,
                               RectangleElement lineEnd = null,
                               List<CanvasElement> hierarchy = null)
                               : base(parentElement, ref canvasTexture, origin, size, hierarchy) {
                this.isVertical = isVertical;
                this.line = line;
                this.lineStart = lineStart;
                this.lineEnd = lineEnd;
            }

            #endregion Constructors

            public override void Draw(SpriteBatch spriteBatch) {
                int end = isVertical ? Bottom : Right;
                Point drawOrigin = Location;
                int offset;
                do {
                    offset = isVertical ? drawOrigin.Y : drawOrigin.X;
                    if (lineEnd != null && offset >= end - (isVertical ? lineEnd.Height : lineEnd.Width)) {
                        DrawAndOffset(lineEnd, ref drawOrigin);
                    }
                    else if (lineStart != null && offset == (isVertical ? Y : X)) {
                        DrawAndOffset(lineStart, ref drawOrigin);
                    }
                    else {
                        DrawAndOffset(line, ref drawOrigin);
                    }
                } while (offset < end);
                base.Draw(spriteBatch);

                void DrawAndOffset(RectangleElement element, ref Point origin) {
                    element.Draw(spriteBatch, origin);
                    if (isVertical) {
                        origin.Y += element.Height;
                    }
                    else {
                        origin.X += element.Width;
                    }
                }
            }
        }

        public class BoxElement : CanvasElement {
            private readonly RectangleElement bottomLeftCorner;
            private readonly RectangleElement bottomRightCorner;
            private readonly RectangleElement genericCorner;
            private readonly LineElement horizontalLine;
            private readonly RectangleElement topLeftCorner;
            private readonly RectangleElement topRightCorner;
            private readonly LineElement verticalLine;
            private Rectangle horizontalTopRectangle;
            private Rectangle horizontalBottomRectangle;
            private Rectangle verticalLeftRectangle;
            private Rectangle verticalRightRectangle;

            public BoxElement([Required] BoxElement previous)
                : base(previous.ParentElement) {
                CanvasTexture = previous.CanvasTexture;
                ElementBounds = previous.ElementBounds;
                bottomLeftCorner = previous.bottomLeftCorner;
                bottomRightCorner = previous.bottomRightCorner;
                genericCorner = previous.genericCorner;
                horizontalLine = previous.horizontalLine;
                topLeftCorner = previous.topLeftCorner;
                topRightCorner = previous.topRightCorner;
                verticalLine = previous.verticalLine;
                horizontalTopRectangle = previous.horizontalTopRectangle;
                horizontalBottomRectangle = previous.horizontalBottomRectangle;
                verticalLeftRectangle = previous.verticalLeftRectangle;
                verticalRightRectangle = previous.verticalRightRectangle;
            }

            #region Constructors

            public BoxElement([Required] ref SliceReference canvasTexture,
                              [NonEmpty] Rectangle rootBounds,
                              [Required] LineElement mHorizontalLine,
                              [Required] LineElement mVerticalLine,
                              [Required] RectangleElement mGenericCorner,
                              RectangleElement mTopLeftCorner = null,
                              RectangleElement mTopRightCorner = null,
                              RectangleElement mBottomLeftCorner = null,
                              RectangleElement mBottomRightCorner = null,
                              List<CanvasElement> hierarchy = null)
                              : base(ref canvasTexture, rootBounds, hierarchy) {
                horizontalLine = mHorizontalLine;
                verticalLine = mVerticalLine;
                genericCorner = mGenericCorner;
                topLeftCorner = mTopLeftCorner ?? new RectangleElement(mGenericCorner);
                topRightCorner = mTopRightCorner ?? new RectangleElement(mGenericCorner);
                bottomLeftCorner = mBottomLeftCorner ?? new RectangleElement(mGenericCorner);
                bottomRightCorner = mBottomRightCorner ?? new RectangleElement(mGenericCorner);
                SetDrawBounds();
            }

            public BoxElement([Required] ref SliceReference canvasTexture,
                              [NonEmpty] Point origin,
                              [NonEmpty] Point size,
                              [Required] LineElement mHorizontalLine,
                              [Required] LineElement mVerticalLine,
                              [Required] RectangleElement mGenericCorner,
                              RectangleElement mTopLeftCorner = null,
                              RectangleElement mTopRightCorner = null,
                              RectangleElement mBottomLeftCorner = null,
                              RectangleElement mBottomRightCorner = null,
                              List<CanvasElement> hierarchy = null)
                              : base(ref canvasTexture, origin, size, hierarchy) {
                horizontalLine = mHorizontalLine;
                verticalLine = mVerticalLine;
                genericCorner = mGenericCorner;
                topLeftCorner = mTopLeftCorner ?? new RectangleElement(mGenericCorner);
                topRightCorner = mTopRightCorner ?? new RectangleElement(mGenericCorner);
                bottomLeftCorner = mBottomLeftCorner ?? new RectangleElement(mGenericCorner);
                bottomRightCorner = mBottomRightCorner ?? new RectangleElement(mGenericCorner);
                SetDrawBounds();
            }

            public BoxElement(CanvasElement parentElement,
                              [Required] ref SliceReference canvasTexture,
                              [Required][NonEmpty] Rectangle rootBounds,
                              [Required] LineElement mHorizontalLine,
                              [Required] LineElement mVerticalLine,
                              [Required] RectangleElement mGenericCorner,
                              RectangleElement mTopLeftCorner = null,
                              RectangleElement mTopRightCorner = null,
                              RectangleElement mBottomLeftCorner = null,
                              RectangleElement mBottomRightCorner = null,
                              List<CanvasElement> hierarchy = null)
                              : base(parentElement, ref canvasTexture, rootBounds, hierarchy) {
                horizontalLine = mHorizontalLine;
                verticalLine = mVerticalLine;
                genericCorner = mGenericCorner;
                topLeftCorner = mTopLeftCorner ?? new RectangleElement(mGenericCorner);
                topRightCorner = mTopRightCorner ?? new RectangleElement(mGenericCorner);
                bottomLeftCorner = mBottomLeftCorner ?? new RectangleElement(mGenericCorner);
                bottomRightCorner = mBottomRightCorner ?? new RectangleElement(mGenericCorner);
                SetDrawBounds();
            }

            public BoxElement(CanvasElement parentElement,
                              [Required] ref SliceReference canvasTexture,
                              [NonEmpty] Point origin,
                              [NonEmpty] Point size,
                              [Required] LineElement mHorizontalLine,
                              [Required] LineElement mVerticalLine,
                              [Required] RectangleElement mGenericCorner,
                              RectangleElement mTopLeftCorner = null,
                              RectangleElement mTopRightCorner = null,
                              RectangleElement mBottomLeftCorner = null,
                              RectangleElement mBottomRightCorner = null,
                              List<CanvasElement> hierarchy = null)
                              : base(parentElement, ref canvasTexture, origin, size, hierarchy) {
                horizontalLine = mHorizontalLine;
                verticalLine = mVerticalLine;
                genericCorner = mGenericCorner;
                topLeftCorner = mTopLeftCorner ?? new RectangleElement(mGenericCorner);
                topRightCorner = mTopRightCorner ?? new RectangleElement(mGenericCorner);
                bottomLeftCorner = mBottomLeftCorner ?? new RectangleElement(mGenericCorner);
                bottomRightCorner = mBottomRightCorner ?? new RectangleElement(mGenericCorner);
                SetDrawBounds();
            }

            #endregion Constructors

            private void SetDrawBounds() {
                topLeftCorner.Location = Location;
                topRightCorner.Location = Location;
                bottomLeftCorner.Location = Location;
                bottomRightCorner.Location = Location;
                horizontalTopRectangle.Location = Location;
                horizontalBottomRectangle.Location = Location;
                verticalLeftRectangle.Location = Location;
                verticalRightRectangle.Location = Location;

                topRightCorner.X = Right - topRightCorner.Width;
                topRightCorner.Y = Top;
                bottomLeftCorner.X = Left;
                bottomLeftCorner.Y = Bottom - bottomLeftCorner.Height;

                bottomRightCorner.Location = new Point(Right, Bottom)
                    - bottomRightCorner.Size;

                horizontalTopRectangle.X += topLeftCorner.Width;
                horizontalTopRectangle.Width = Width - topLeftCorner.Width
                    - topRightCorner.Width;

                horizontalBottomRectangle.X += bottomLeftCorner.Width;
                horizontalBottomRectangle.Width = Width - bottomLeftCorner.Width
                    - bottomRightCorner.Width;

                horizontalBottomRectangle.Y = Bottom
                    - (bottomLeftCorner.Height < bottomRightCorner.Height
                    ? bottomLeftCorner.Height : bottomRightCorner.Height);

                verticalLeftRectangle.Y += topLeftCorner.Height;
                verticalLeftRectangle.Height = Height - topLeftCorner.Height
                    - bottomLeftCorner.Height;

                verticalRightRectangle.Y += topRightCorner.Height;
                verticalRightRectangle.Height = Height - topRightCorner.Height
                    - bottomRightCorner.Height;

                verticalRightRectangle.X = Right
                    - (topRightCorner.Width < bottomRightCorner.Width
                    ? topRightCorner.Width : bottomRightCorner.Width);
            }

            public override void Centralize() {
                base.Centralize();
                SetDrawBounds();
            }

            public override void Draw(SpriteBatch spriteBatch) {
                horizontalLine.ElementBounds = horizontalTopRectangle;
                horizontalLine.Draw(spriteBatch);
                horizontalLine.ElementBounds = horizontalBottomRectangle;
                horizontalLine.Draw(spriteBatch);
                verticalLine.ElementBounds = verticalLeftRectangle;
                verticalLine.Draw(spriteBatch);
                verticalLine.ElementBounds = verticalRightRectangle;
                verticalLine.Draw(spriteBatch);
                topLeftCorner.Draw(spriteBatch);
                topRightCorner.Draw(spriteBatch);
                bottomLeftCorner.Draw(spriteBatch);
                bottomRightCorner.Draw(spriteBatch);
                base.Draw(spriteBatch);
            }
        }
    }
}