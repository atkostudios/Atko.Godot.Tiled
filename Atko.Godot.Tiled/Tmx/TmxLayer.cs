using System.Collections.Generic;
using System.Xml.Linq;
using Godot;

namespace Atko.Godot.Tiled.Tmx {
    public class TmxLayer : TileMap {
        public string TmxId;
        public string TmxName;
        public int Width;
        public int Height;
        public float TileWidth;
        public float TileHeight;
        public bool IsInfinite;
        public XElement Data;
        public List<TmxChunk> Chunks = new List<TmxChunk>();
        public TileSet Tileset;
        public bool LayerIsVisible;
        
        public TmxLayer(XElement tmxElement, TileSet tileset, float tileWidth, float tileHeight, bool infinite = false) {
            TmxId = tmxElement.Attribute("id")?.Value;
            TmxName = tmxElement.Attribute("name")?.Value;
            Width = tmxElement.Attribute("width")?.Value.ToInt() ?? 0;
            Height = tmxElement.Attribute("height")?.Value.ToInt() ?? 0;
            LayerIsVisible = tmxElement.Attribute("visible") == null;
            IsInfinite = infinite;
            Data = tmxElement.Element("data");
            TileWidth = tileWidth;
            TileHeight = tileHeight;
            Tileset = tileset;
            SetTileset(Tileset);
            SetCellSize(new Vector2(TileWidth, TileHeight));

            if (TmxName != "") {
                SetName(TmxName);
            }
            
            if (IsInfinite) {
                foreach (var dataEl in Data.Elements()) {
                    Chunks.Add(new TmxChunk(dataEl));
                }
            } else {
                Chunks.Add(new TmxChunk(Data, Width, Height));
            }
            
            Chunks.ForEach(GenerateChunk);
            SetVisible(LayerIsVisible);
        }
        
        void GenerateChunk(TmxChunk chunk) {
            for (var x = 0; x < chunk.Width; x++) {
                for (var y = 0; y < chunk.Height; y++) {
                    var tile = chunk.TileGrid[x][y];
                    SetCell(x + chunk.X, y + chunk.Y, tile.Gid, tile.IsHorizontallyFlipped, tile.IsVerticallyFlipped, tile.IsDiagonallyFlipped);
                }
            }
        }
    }
}