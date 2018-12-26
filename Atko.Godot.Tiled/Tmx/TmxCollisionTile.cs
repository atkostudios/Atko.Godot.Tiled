using System.Xml.Linq;
using Godot;

namespace Atko.Godot.Tiled.Tmx {
    public class TmxCollisionTile {
        public int Id;
        public float Width;
        public float Height;
        public float X;
        public float Y;
        public int ObjectId;
        
        public TmxCollisionTile(XElement tmxElement) {
            Id = tmxElement.Attribute("id") != null ? tmxElement.Attribute("id").Value.ToInt() : -1;

            if (tmxElement.Element("objectgroup") != null) {
                var objGroup = tmxElement.Element("objectgroup");

                if (objGroup.Element("object") != null) {
                    var obj = objGroup.Element("object");

                    Width = obj.Attribute("width")?.Value.ToFloat() ?? 0;
                    Height = obj.Attribute("height")?.Value.ToFloat() ?? 0;
                    X = obj.Attribute("x")?.Value.ToFloat() ?? 0;
                    Y = obj.Attribute("y")?.Value.ToFloat() ?? 0;
                    ObjectId = obj.Attribute("id")?.Value.ToInt() ?? -1;
                }
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
}