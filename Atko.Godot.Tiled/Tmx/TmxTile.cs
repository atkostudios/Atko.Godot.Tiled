using Godot;

// CREDIT
// https://github.com/marshallward/TiledSharp/blob/master/TiledSharp/src/Layer.cs

namespace Atko.Godot.Tiled.Tmx {
    public class TmxTile : Node2D {
        const uint FLIPPED_HORIZONTALLY_FLAG = 0x80000000;
        const uint FLIPPED_VERTICALLY_FLAG = 0x40000000;
        const uint FLIPPED_DIAGONALLY_FLAG = 0x20000000;
        
        public int Gid { get; set; } = 0;
        public uint RawGid { get; set; } = 0;
        public float X { get; set; }
        public float Y { get; set; }
        public bool IsDiagonallyFlipped { get; set; }
        public bool IsHorizontallyFlipped { get; set; }
        public bool IsVerticallyFlipped { get; set; }

        public TmxTile(uint rawGid, int x, int y) {
            RawGid = rawGid;
            X = x;
            Y = y;

            IsDiagonallyFlipped = (rawGid & FLIPPED_DIAGONALLY_FLAG) != 0;
            IsHorizontallyFlipped = (rawGid & FLIPPED_HORIZONTALLY_FLAG) != 0;
            IsVerticallyFlipped = (rawGid & FLIPPED_VERTICALLY_FLAG) != 0;
            
            // Zero the bit flags
            rawGid &= ~(FLIPPED_DIAGONALLY_FLAG |
                        FLIPPED_HORIZONTALLY_FLAG |
                        FLIPPED_VERTICALLY_FLAG);

            Gid = (int) rawGid;
        }
    }
}