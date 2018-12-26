using System;
using System.Linq;
using System.Xml.Linq;
using Godot;

namespace Atko.Godot.Tiled.Tmx {
    public class TmxTilesetTile {
        public event Action<int, int> UpdateAnimationTileEvent; 
        public int Id;
        public float Width;
        public float Height;
        public float X;
        public float Y;
        public int ObjectId;
        public bool IsAnimated;
        public bool IsCollidable;
        public int AnimationDelta;
        public TmxTileFrame[] Frames;
        public int FrameCursor;
        public System.Collections.Generic.Dictionary<string, string> Properties = new System.Collections.Generic.Dictionary<string, string>();

        public TmxTilesetTile(XElement tmxElement) {
            Id = tmxElement.Attribute("id")?.Value.ToInt() ?? -1;

            if (tmxElement.Element("objectgroup") != null) {
                var objGroup = tmxElement.Element("objectgroup");

                if (objGroup.Element("object") != null) {
                    var obj = objGroup.Element("object");

                    Width = obj.Attribute("width")?.Value.ToFloat() ?? 0;
                    Height = obj.Attribute("height")?.Value.ToFloat() ?? 0;
                    X = obj.Attribute("x")?.Value.ToFloat() ?? 0;
                    Y = obj.Attribute("y")?.Value.ToFloat() ?? 0;
                    ObjectId = obj.Attribute("id")?.Value.ToInt() ?? -1;
                    IsCollidable = true;
                }
            }

            if (tmxElement.Element("animation") != null) {
                IsAnimated = true;
                var animation = tmxElement.Element("animation");
                Frames = new TmxTileFrame[animation.Elements("frame").Count()];

                int i = 0;
                foreach (var frame in animation.Elements("frame")) {
                    Frames[i] = new TmxTileFrame {
                        TileId = frame.Attribute("tileid")?.Value.ToInt() ?? 0,
                        DurationInMilliseconds = frame.Attribute("duration")?.Value.ToInt() ?? 0
                    };

                    i++;
                }
            }
            
            tmxElement.Element("properties")?
                .Elements()
                .ToList()
                .ForEach(p => {
                    var name = p.Attribute("name")?.Value ?? "none";
                    var value = p.Attribute("value")?.Value ?? "none";
                    Properties.Add(name, value);
                });
        }

        public void Update(int deltaMilliseconds) {
            if (!IsAnimated) return;
            
            AnimationDelta += deltaMilliseconds;

            if (AnimationDelta >= Frames[FrameCursor].DurationInMilliseconds) {
                var prevFrame = Frames[FrameCursor];
                AnimationDelta = 0;
                FrameCursor++;

                if (FrameCursor >= Frames.Length) FrameCursor = 0;

                var newFrame = Frames[FrameCursor];

                UpdateAnimationTileEvent?.Invoke(prevFrame.TileId, newFrame.TileId);
            }
        }

        public Shape2D GetShape() {
            var shape = new RectangleShape2D();
            shape.SetExtents(new Vector2(Width / 2, Height / 2));
            return shape;
        }

        public Transform2D GetShapeTransform() {
            return new Transform2D(new Vector2(), new Vector2(), new Vector2(Width / 2, Height / 2));;
        }
    }

    public class TmxTileFrame {
        public int TileId { get; set; } = 0;
        public int DurationInMilliseconds { get; set; } = 0;
    }
}