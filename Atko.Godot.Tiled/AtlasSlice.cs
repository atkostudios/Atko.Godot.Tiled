using Atko.Godot.Tiled.Tmx;
using Godot;

namespace Atko.Godot.Tiled
{

    public class AtlasSlice : Node2D
    {

        public TmxLayer TmxBackground { get; set; }

        public override void _Ready()
        {
            base._Ready();

            
            if (TmxBackground != null)
            {
                AddChild(TmxBackground);
                
                if (TmxBackground.IsInfinite) {
                    var offsetX = (TmxBackground.Width / 2) * TmxBackground.TileWidth;
                    var offsetY = (TmxBackground.Height / 2) * TmxBackground.TileHeight;
                    
                    TmxBackground.SetPosition(new Vector2(offsetX, offsetY));
                }
            }
        }

    }

}