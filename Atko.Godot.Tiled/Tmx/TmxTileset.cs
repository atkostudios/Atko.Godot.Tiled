using System.Collections.Generic;
using System.Linq;
using Godot;
using System.Xml.Linq;
using Atko.Godot.Tiled.RelativePathing;

namespace Atko.Godot.Tiled.Tmx {
    public class TmxTileset {
        public string TmxName;
        public string TsxPath;
        public string SpriteSheetPath;
        public int FirstGid;
        public string Source;
        public Sprite SpriteSheet;
        public Texture SpriteSheetTexture;
        public float SpriteSheetWidth;
        public float SpriteSheetHeight;
        public float TileWidth;
        public float TileHeight;
        public int TileCount;
        public int Columns;
        public int Margin;
        public int Spacing;
        public int Rows;
        public List<TmxCollisionTile> CollisionTiles;
        
        public TmxTileset(XElement tmxElement, string mapFileDirectory, int textureFlags = 0) {
            FirstGid = tmxElement.Attribute("firstgid")?.Value.ToInt() ?? -1;
            Source = tmxElement.Attribute("source")?.Value ?? "none";
            XElement tsx = tmxElement;
            TsxPath = mapFileDirectory;

            if (Source != "none") {
                TsxPath = RelativePath.GetPath(mapFileDirectory, Source);
                var tsxFile = new File();
                tsxFile.Open(TsxPath, (int) File.ModeFlags.Read);
                tsx = XElement.Parse(tsxFile.GetAsText());
                tsxFile.Close();
            }
            
            TmxName = tsx.Attribute("name")?.Value;
            TileWidth = tsx.Attribute("tilewidth")?.Value.ToFloat() ?? 0;
            TileHeight = tsx.Attribute("tileheight")?.Value.ToFloat() ?? 0;
            TileCount = tsx.Attribute("tilecount")?.Value.ToInt() ?? 0;
            Margin = tsx.Attribute("margin")?.Value.ToInt() ?? 0;
            Spacing = tsx.Attribute("spacing")?.Value.ToInt() ?? 0;
            Columns = tsx.Attribute("columns")?.Value.ToInt() ?? 0;

            var img = tsx.Element("image");
            var fp = TsxPath.Split("res://")[0];
            var directory = Source != "none" ? fp.Split("/").Take(fp.Split("/").Length - 1) : fp.Split("/");
            SpriteSheetPath = RelativePath.GetPath(string.Join("/", directory), img.Attribute("source").Value);
            SpriteSheetWidth = img.Attribute("width")?.Value.ToFloat() ?? 0;
            SpriteSheetHeight = img.Attribute("height")?.Value.ToFloat() ?? 0;
            SpriteSheetTexture = (Texture) GD.Load(SpriteSheetPath);
            SpriteSheet = new Sprite();
            SpriteSheetTexture.SetFlags(textureFlags);
            SpriteSheet.SetTexture(SpriteSheetTexture);

            CollisionTiles = tsx
                .Elements()
                .Where(e => e.Name.ToString() == "tile")
                .Select(tmx => new TmxCollisionTile(tmx))
                .ToList();

            Rows = (int)SpriteSheetHeight / (int)TileHeight;
        }
    }
}
