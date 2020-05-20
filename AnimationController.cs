using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using General;

class AnimationController {
    public Dictionary<string, AnimatedSprite> mCharacterSpriteSet;
    public AnimatedSprite activeAnim;
    public string activeCategory;
    public string defaultAnimName;
    private int frameToDraw = 0;
    private double cumulativeDelta;

    public AnimationController() {
        
    }

    public AnimationController(Dictionary<string, AnimatedSprite> spriteSet, string defaultAnimation) {
        mCharacterSpriteSet = spriteSet;
        defaultAnimName = defaultAnimation;
        mCharacterSpriteSet.TryGetValue(defaultAnimName, out activeAnim);
        SetFrame();
    }

    public void Update(GameTime gameTime) {
        if (cumulativeDelta < activeAnim.ActiveTag.FrameTime) {
            cumulativeDelta += gameTime.ElapsedGameTime.TotalMilliseconds;
        }
        else {
            frameToDraw++;
            if (frameToDraw > activeAnim.ActiveTag.EndFrame) {
                if (!activeAnim.ActiveTag.Continuous) {
                    mCharacterSpriteSet.TryGetValue(defaultAnimName, out activeAnim);
                }
                frameToDraw = activeAnim.ActiveTag.StartFrame;
            }
            cumulativeDelta = 0;
        }
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 location) {
        Rectangle sourceRect = new Rectangle(
            (int)activeAnim.SpriteDimensions.X * (frameToDraw), 0,
            (int)activeAnim.SpriteDimensions.X,
            (int)activeAnim.SpriteDimensions.Y);
        Rectangle destRect = new Rectangle(
            (int)location.X, (int)location.Y, (int)activeAnim.SpriteDimensions.X, (int)activeAnim.SpriteDimensions.Y);
        spriteBatch.Draw(activeAnim.Spritesheet, destRect, sourceRect, Color.White);
    }

    #region Animation Control
    public void SetAnim(string animName, string category, string animTag) {
        activeAnim = mCharacterSpriteSet[animName];
        SetCategory(category, animTag);
    }
    public void SetCategory(string category, string animTag) {
        activeCategory = category;
        SetTag(animTag);
    }
    public void SetTag(string animTag) {
        activeAnim.ActiveTag = activeAnim.CategorizedTags[activeCategory][animTag];
        SetFrame();
    }
    private void SetFrame() {
        frameToDraw = activeAnim.ActiveTag.StartFrame;
    }
    #endregion

    // Populates fields with animation data. Typically called from LoadContent().
    public void LoadInit(Dictionary<string, AnimatedSprite> spriteSet, string defaultAnimation) {
        mCharacterSpriteSet = spriteSet;
        defaultAnimName = defaultAnimation;
    }
}
