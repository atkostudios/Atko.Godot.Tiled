using Godot;
using Atko.Godot.Tiled;

public class Init : Node2D
{
    public override void _Ready()
    {
        AddChild(new Atlas("res://examples/test.tmx"));
    }
}
