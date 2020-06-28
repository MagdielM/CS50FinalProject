using FrameDataLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

class Animator {
    protected Dictionary<string, SpriteReference> SpriteSet { get; set; }
    protected SpriteReference ActiveAnimation { get; set; }
    public string ActiveCategory { get; set; }
    public string ActiveTag { get; set; }
    protected string defaultAnimName;
    public int frameToDraw = 0;
    private double cumulativeDelta;

    /// <summary>
    /// Empty constructor. LoadInit() should be called before use.
    /// </summary>
    public Animator() { }

    /// <summary>
    /// Convenience constructor. This is equivalent to creating the empty object and calling LoadInit(spriteSet, default).
    /// </summary>
    public Animator(Dictionary<string, SpriteReference> spriteSet, string defaultAnimation) {
        LoadInit(spriteSet, defaultAnimation);
    }

    /// <summary>
    /// Populates fields with animation data. Typically called from LoadContent().
    /// </summary>
    /// <param name="spriteSet">The set of <see cref="SpriteReference"/>s this object will hold.</param>
    /// <param name="defaultAnimation">The name of the default animation in <paramref name="spriteSet"/>.</param>
    public void LoadInit(Dictionary<string, SpriteReference> spriteSet, string defaultAnimation) {
        SpriteSet = spriteSet;
        defaultAnimName = defaultAnimation;
        SetToDefault(defaultAnimation);

    }

    /// <summary>
    /// Convenience method to reset this object's ActiveAnimation to default.
    /// </summary>
    /// <param name="defaultAnimation">The name of the default animation in this object's SpriteSet.</param>
    /// <exception cref="KeyNotFoundException">
    /// Thrown when <paramref name="defaultAnimation"/> can't be found in <paramref name="spriteSet"/>.
    /// </exception>
    private void SetToDefault(string defaultAnimation) {
        FrameData animation = SpriteSet[defaultAnimation].FrameData;
        int frame = SpriteSet[defaultAnimation].FrameData.ActiveTag.StartFrame;
        SetAnimation(defaultAnimation, animation.DefaultAnimationCategory, animation.DefaultAnimationTag, frame);
    }

    public virtual void Update(GameTime gameTime) {
        if (cumulativeDelta < ActiveAnimation.FrameData.ActiveTag.FrameTime) {
            cumulativeDelta += gameTime.ElapsedGameTime.TotalMilliseconds;
        }
        else {
            frameToDraw++;
            if (frameToDraw > ActiveAnimation.FrameData.ActiveTag.EndFrame) {
                frameToDraw = ActiveAnimation.FrameData.ActiveTag.StartFrame;
            }
            cumulativeDelta = 0;
        }
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 location) {
        Rectangle sourceRectangle = new Rectangle(
            (int)ActiveAnimation.FrameData.SpriteDimensions.X * (frameToDraw), 0,
            (int)ActiveAnimation.FrameData.SpriteDimensions.X,
            (int)ActiveAnimation.FrameData.SpriteDimensions.Y);
        Rectangle destinationRectangle = new Rectangle(
            (int)location.X, (int)location.Y,
            (int)ActiveAnimation.FrameData.SpriteDimensions.X,
            (int)ActiveAnimation.FrameData.SpriteDimensions.Y);
        spriteBatch.Draw(ActiveAnimation.Sprite, destinationRectangle, sourceRectangle, Color.White);
    }

    public void Draw(SpriteBatch spriteBatch, Rectangle destinationRectangle) {
        Rectangle sourceRectangle = new Rectangle(
            (int)ActiveAnimation.FrameData.SpriteDimensions.X * (frameToDraw), 0,
            (int)ActiveAnimation.FrameData.SpriteDimensions.X,
            (int)ActiveAnimation.FrameData.SpriteDimensions.Y);
        spriteBatch.Draw(ActiveAnimation.Sprite, destinationRectangle, sourceRectangle, Color.White);
    }

    #region Animation Control

    /// <summary>
    /// 
    /// </summary>
    /// <param name="animationName"></param>
    /// <param name="category"></param>
    /// <param name="animationTag"></param>
    /// <param name="frame"></param>
    public void SetAnimation(string animationName, string category, string animationTag, int frame = 0) {
        ActiveAnimation = SpriteSet[animationName];
        SetCategory(category, animationTag, frame);
    }
    public void SetCategory(string category, string animationTag, int frame = 0) {
        ActiveCategory = category;
        SetTag(animationTag, frame);
    }
    public void SetTag(string animTag, int frame = 0) {
        ActiveAnimation.FrameData.ActiveTag = ActiveAnimation.FrameData.CategorizedTags[ActiveCategory][animTag];
        ActiveTag = animTag;
        SetFrame(frame);
    }
    private void SetFrame(int frame = 0) {
        frameToDraw = ActiveAnimation.FrameData.ActiveTag.GetFrameFromRelative(frame);
    }

    /// <summary>
    /// Shorthand for AnimationController.ActiveAnimation.FrameData.GetFrameFromDefault(frame);
    /// </summary>
    /// <param name="frame">The zero-indexed relative frame to convert.</param>
    /// <returns>The absolute frame in the <see cref="FrameData.TagData"/> based on the relative frame entered.</returns>
    public int GetFrameFromActiveDefault(int frame) {
        return ActiveAnimation.FrameData.GetFrameFromDefault(frame);
    }

    #endregion
}
