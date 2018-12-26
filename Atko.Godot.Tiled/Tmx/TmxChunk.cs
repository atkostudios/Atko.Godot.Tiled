using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Xml.Linq;
using Godot;

// CREDIT
// https://github.com/marshallward/TiledSharp/blob/master/TiledSharp/src/Layer.cs

namespace Atko.Godot.Tiled.Tmx {
    public class TmxChunk {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string EncodingMethod { get; set; }
        public string CompressionMethod { get; set; }
        public TmxTile[][] TileGrid { get; set; }

        public TmxChunk(XElement tmx, int width = 0, int height = 0) {
            Width = tmx.Attribute("width")?.Value.ToInt() ??  width;
            Height = tmx.Attribute("height")?.Value.ToInt() ??  height;
            X = tmx.Attribute("x")?.Value.ToInt() ??  0;
            Y = tmx.Attribute("y")?.Value.ToInt() ??  0;
            EncodingMethod = tmx.Attribute("encoding")?.Value ?? "xml";
            
            // .tmx maps with no encoding method use XML by default
            CompressionMethod = tmx.Attribute("compression")?.Value ?? "none";
            TileGrid = new TmxTile[Width][];
            for (var x = 0; x < Width; x++) TileGrid[x] = new TmxTile[Height];

            switch (EncodingMethod) {
                case "base64":
                    LoadBase64(tmx.Value.Trim());
                    break;
                case "csv":
                    LoadCsv(tmx.Value.Trim());
                    break;
                case "xml":
                    LoadXmlTiles(tmx.Elements("tile"));
                    break;
                default:
                    throw new ArgumentException("Invalid encoding method: " + EncodingMethod);
            }
        }

        void LoadBase64(string baseData) {
            var stream = GetBase64Stream(baseData);

            using (var reader = new BinaryReader(stream)) {
                for (var y = 0; y < Height; y++) {
                    for (var x = 0; x < Width; x++) {
                        TileGrid[x][y] = new TmxTile(reader.ReadUInt32(), x, y);
                    }
                }
            }
        }

        void LoadCsv(string csv) {
            var i = 0;

            foreach (var gid in csv.Split(",")) {
                var rawGid = uint.Parse(gid.Trim());
                var x = i % Width;
                var y = i / Width;
                
                TileGrid[x][y] = new TmxTile(rawGid, x, y);
                i++;
            }
        }

        void LoadXmlTiles(IEnumerable<XElement> tiles) {
            var i = 0;
            
            foreach (var tile in tiles) {
                var rawGid = tile.Attribute("gid") != null ? uint.Parse(tile.Attribute("gid").Value) : 0;
                var x = i % Width;
                var y = i / Width;
                
                TileGrid[x][y] = new TmxTile(rawGid, x, y);
                i++;
            }
        }

        Stream GetBase64Stream(string baseData) {
            var bytes = Convert.FromBase64String(baseData);
            Stream stream;

            switch (CompressionMethod) {
                case "none":
                    stream = new MemoryStream(bytes, false);
                    break;
                case "zlib":
                    var length = bytes.Length - 6;
                    var byteData = new byte[length];
                    System.Array.Copy(bytes, 2, byteData, 0, length);
                    var byteStream = new MemoryStream(byteData, false);
                    stream = new DeflateStream(byteStream, CompressionMode.Decompress);
                    break;
                case "gzip":
                    stream = new GZipStream(new MemoryStream(bytes, false), CompressionMode.Decompress);
                    break;
                default:
                    throw new ArgumentException("Invalid compression method given: " + CompressionMethod);
            }

            return stream;
        }
    }
}