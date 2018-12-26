using System.Linq;
using System.Xml.Linq;
using Godot;

namespace Atko.Godot.Tiled.Tmx {
    public class TmxObject {
        public int TmxId;
        public int Gid;
        public float X;
        public float Y;
        public float Width;
        public float Height;
        public System.Collections.Generic.Dictionary<string, string> Properties = new System.Collections.Generic.Dictionary<string, string>();

        public TmxObject(XElement tmxElement) {
            TmxId = tmxElement.Attribute("id")?.Value.ToInt() ?? -1;
//            Gid = tmxElement.Attribute("gid")?.Value.ToInt() ?? -1;
            X = tmxElement.Attribute("x")?.Value.ToFloat() ?? 0;
            Y = tmxElement.Attribute("y")?.Value.ToFloat() ?? 0;
            Width = tmxElement.Attribute("width")?.Value.ToFloat() ?? 0;
            Height = tmxElement.Attribute("height")?.Value.ToFloat() ?? 0;

            tmxElement.Element("properties")?
                .Elements()
                .ToList()
                .ForEach(p => {
                    var name = p.Attribute("name")?.Value;
                    var value = p.Attribute("value")?.Value;
                    Properties.Add(name, value);
                });
        }
    }
}