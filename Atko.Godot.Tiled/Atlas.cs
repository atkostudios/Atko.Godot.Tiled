using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Atko.Godot.Tiled.Tmx;
using Godot;

namespace Atko.Godot.Tiled {
    public class Atlas : Node2D {
        public string FilePath;
        public string Orientation;
        public string RenderOrder;
        public int Width;
        public int Height;
        public int TileWidth;
        public int TileHeight;
        public bool Infinite;
        public List<TmxTileset> TmxTilesets = new List<TmxTileset>();
        public TileSet Tileset;
        public List<TmxObjectGroup> ObjectGroups = new List<TmxObjectGroup>();
        public List<TmxObject> LoadedObjects = new List<TmxObject>();
        public List<AtlasSlice> Slices = new List<AtlasSlice>();
        public Action<TmxObject> tmxObjectHandler;
        
        public Atlas(string filePath, Action<TmxObject> callback = null) {
            FilePath = filePath;
            tmxObjectHandler = callback;
            
            var file = new File();

            if (!file.FileExists(FilePath)) {
                GD.Print("No file exists at " + FilePath);
                file.Close();
                return;
            }

            file.Open(FilePath, (int)File.ModeFlags.Read);
            var tmx = XElement.Parse(file.GetAsText());
            file.Close();
            
            Orientation = tmx.Attribute("orientation")?.Value;
            RenderOrder = tmx.Attribute("renderorder")?.Value;
            Width = tmx.Attribute("width")?.Value.ToInt() ?? 0;
            Height = tmx.Attribute("height")?.Value.ToInt() ?? 0;
            TileWidth = tmx.Attribute("tilewidth")?.Value.ToInt() ?? 0;
            TileHeight = tmx.Attribute("tileheight")?.Value.ToInt() ?? 0;
            Infinite = tmx.Attribute("infinite")?.Value == "1";
            Tileset = BuildTileset(tmx.Elements("tileset").ToList());
            
            foreach (var tmxElement in tmx.Elements()) {
                switch (tmxElement.Name.ToString()) {
                    case "layer":
                        LoadLayer(tmxElement);
                        break;
                    case "objectgroup":
                        LoadObjectGroup(tmxElement);
                        break;
                }
            }
            
            Slices.ForEach(slice => InsertSlice(slice));
        }

        public void InsertSlice(AtlasSlice slice, int position = -1)
        {
            AddChild(slice);

            if (position == -1) return;
            
            MoveChild(slice, position);
        }
        
        TileSet BuildTileset(List<XElement> tmxElements) {
            var tileset = new TileSet();
            
            TmxTilesets = tmxElements.Select(t => BuildTmxTileset(t)).ToList();
            
            TmxTilesets.ForEach(tmxTileset => {
                for (var i = 0; i < tmxTileset.TileCount; i++) {
                    var gid = tmxTileset.FirstGid + i;
                    
                    tileset.CreateTile(gid);
                    tileset.TileSetName(gid, tmxTileset.TmxName + " Gid: " + gid);
                    tileset.TileSetTexture(gid, tmxTileset.SpriteSheetTexture);
                    tileset.TileSetRegion(gid, GetRegionInTileset(gid, tmxTileset));
                }
                
                tmxTileset.CollisionTiles.ForEach(t => {
                    tileset.TileSetShape(t.Id + 1, t.ObjectId, t.GetShape());
                    tileset.TileSetShapeTransform(t.Id + 1, t.ObjectId, t.GetShapeTransform());
                });
            });

            return tileset;
        }

        Rect2 GetRegionInTileset(int gid, TmxTileset tileset) {
            return new Rect2(GetPositionInTileset(gid, tileset), TileWidth, TileHeight);
        }

        Vector2 GetPositionInTileset(int gid, TmxTileset tileset) {
            var x = (gid - tileset.FirstGid) % tileset.Columns;
            var y = (gid - tileset.FirstGid) / tileset.Columns;
            var posX = x * TileWidth + tileset.Spacing * x + tileset.Margin;
            var posY = y * TileHeight + tileset.Spacing * y + tileset.Margin;
            
            return new Vector2(posX, posY);
        }
        
        TmxTileset BuildTmxTileset(XElement tmxElement) {
            var fp = FilePath.Split("res://")[0];
            var directory = fp.Split("/").Take(FilePath.Split("/").Length - 2);

            return new TmxTileset(tmxElement, "res://" + string.Join("/", directory));
        }

        void LoadLayer(XElement tmxElement) {
            Slices.Add(new AtlasSlice
            {
                TmxBackground = new TmxLayer(tmxElement, Tileset, TileWidth, TileHeight, Infinite)
            });
        }

        void LoadObjectGroup(XElement tmxElement) {
            ObjectGroups.Add(new TmxObjectGroup(tmxElement, tmxObjectHandler));
        }
    }
}