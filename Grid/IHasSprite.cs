using Godot;

namespace NorthEdge.addons.EntityGridAddon.Grid;

public interface IHasSprite
{
    public Color Color { get; set; }

    public Vector2 Size { get; }
}