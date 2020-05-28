using FrameDataLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

class AnimationController {
    public Dictionary<string, SpriteReference> SpriteSet;
    public SpriteReference ActiveAnimation { get; set; }
    public string ActiveCategory { get; set; }
    private string defaultAnimName;
    private int frameToDraw = 0;
    private double cumulativeDelta;

    /// <summary>
    /// Empty constructor. LoadInit() should be called before use.
    /// </summary>
    public AnimationController() { }

    /// <summary>
    /// Convenience constructor. This is equivalent to creating the empty object and calling LoadInit(spriteSet, default).
    /// </summary>
    public AnimationController(Dictionary<string, SpriteReference> spriteSet, string defaultAnimation) {
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
        KeyValuePair<string, string> animation = SpriteSet[defaultAnimation].FrameData.DefaultAnimation;
        int frame = SpriteSet[defaultAnimation].FrameData.ActiveTag.StartFrame;
        SetAnimation(defaultAnimation, animation.Key, animation.Value, frame);
    }

    public void Update(GameTime gameTime) {
        if (cumulativeDelta < ActiveAnimation.FrameData.ActiveTag.FrameTime) {
            cumulativeDelta += gameTime.ElapsedGameTime.TotalMilliseconds;
        }
        else {
            frameToDraw++;
            if (frameToDraw > ActiveAnimation.FrameData.ActiveTag.EndFrame) {
                if (!ActiveAnimation.FrameData.ActiveTag.Continuous) {
                    ActiveAnimation = SpriteSet[defaultAnimName];
                }
                frameToDraw = ActiveAnimation.FrameData.ActiveTag.StartFrame;
            }
            cumulativeDelta = 0;
        }
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 location) {
        Rectangle sourceRect = new Rectangle(
            (int)ActiveAnimation.FrameData.SpriteDimensions.X * (frameToDraw), 0,
            (int)ActiveAnimation.FrameData.SpriteDimensions.X,
            (int)ActiveAnimation.FrameData.SpriteDimensions.Y);
        Rectangle destRect = new Rectangle(
            (int)location.X, (int)location.Y,
            (int)ActiveAnimation.FrameData.SpriteDimensions.X,
            (int)ActiveAnimation.FrameData.SpriteDimensions.Y);
        spriteBatch.Draw(ActiveAnimation.Sprite, destRect, sourceRect, Color.White);
    }

    #region Animation Control
    /// <summary>
    /// 
    /// </summary>
    /// <param name="animationName"></param>
    /// <param name="category"></param>
    /// <param name="animationTag"></param>
    /// <param name="frame"></param>
    public void SetAnimation(string animationName, string category, string animationTag, int frame) {
        ActiveAnimation = SpriteSet[animationName];
        SetCategory(category, animationTag, frame);
    }
    public void SetCategory(string category, string animationTag, int frame) {
        ActiveCategory = category;
        SetTag(animationTag, frame);
    }
    public void SetTag(string animTag, int frame) {
        ActiveAnimation.FrameData.ActiveTag = ActiveAnimation.FrameData.CategorizedTags[ActiveCategory][animTag];
        SetFrame(frame);
    }
    private void SetFrame(int frame) {
        frameToDraw = frame;
    }
    #endregion

    /// <summary>
    /// Shorthand for AnimationController.ActiveAnimation.FrameData.GetFrameFromDefault(frame);
    /// </summary>
    /// <param name="frame">The zero-indexed relative frame to convert.</param>
    /// <returns>The absolute frame in the <see cref="FrameData.TagData"/> based on the relative frame entered.</returns>
    public int GetFrameFromActive(int frame) {
        return ActiveAnimation.FrameData.GetFrameFromDefault(frame);
    }
}
