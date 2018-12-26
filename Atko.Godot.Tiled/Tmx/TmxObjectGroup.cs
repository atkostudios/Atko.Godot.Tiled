using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Godot;

namespace Atko.Godot.Tiled.Tmx {
    public class TmxObjectGroup {
        public int TmxId;
        public XElement TmxElement;
        public string TmxName;
        public List<TmxObject> TmxObjects = new List<TmxObject>();
        
        public TmxObjectGroup(XElement tmxElement, Action<TmxObject> objectHandler = null) {
            TmxElement = tmxElement;
            TmxId = TmxElement.Attribute("id")?.Value.ToInt() ?? -1;
            TmxName = TmxElement.Attribute("name")?.Value;

            foreach (var tmx in TmxElement.Elements()) {
                var o = new TmxObject(tmx);
                TmxObjects.Add(o);
                objectHandler?.Invoke(o);
            }
        }
    }
}