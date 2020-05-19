using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace General {
    public struct AnimatedSprite {
        public Texture2D Spritesheet { get; }
        public double FrameTime { get; }
        public Dictionary<string, Dictionary<string, TagData>> Tags;
        public TagData ActiveTag;
        public string PreviousActiveTagName;
        public string ActiveTagName;
        public KeyValuePair<string, string> DefaultAnimation;
        public Vector2 SpriteDimensions { get; }

        public AnimatedSprite(Texture2D spritesheet, JObject frameData) {
            Spritesheet = spritesheet;
            FrameTime = 1000 / (int)frameData["frameRate"];
            SpriteDimensions = new Vector2((float)frameData["sourceSize"]["w"], (float)frameData["sourceSize"]["h"]);
            Tags = new Dictionary<string, Dictionary<string, TagData>>();

            foreach (JProperty category in frameData["frameTags"]) {
                Dictionary<string, TagData> thisCategory = new Dictionary<string, TagData>();
                JArray tags = (JArray)category.Value;
                foreach (JObject tag in tags) {
                    thisCategory.Add((string)tag["name"], new TagData(
                        (int)tag["from"], (int)tag["to"], (bool)tag["continuous"]));
                }
                Tags.Add(category.Name, thisCategory);
            }

            DefaultAnimation = new KeyValuePair<string, string>(
                frameData["defaultCategory"].Children<JProperty>().First().Name,
                (string)frameData["defaultCategory"].Children<JProperty>().First().Value);
            ActiveTag = Tags
                [DefaultAnimation.Key]
                [DefaultAnimation.Value];
            ActiveTagName = DefaultAnimation.Value;
            PreviousActiveTagName = ActiveTagName;
        }

        public struct TagData {
            public int StartFrame;
            public int EndFrame;
            public bool Continuous;

            public TagData(int startFrame, int endFrame, bool continuous) {
                StartFrame = startFrame;
                EndFrame = endFrame;
                Continuous = continuous;
            }
        }
    }
}
