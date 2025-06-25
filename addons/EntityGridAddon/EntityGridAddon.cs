#if TOOLS

using NorthEdge.addons.EntityGridAddon.Inspector;
using NorthEdge.addons.EntityGridAddon.Grid;
using Godot;

namespace NorthEdge.addons.EntityGridAddon;

[Tool]
public partial class EntityGridAddon : EditorPlugin
{
	private readonly DelegationInspector _delegationInspector = new();

	public override void _EnterTree()
	{
		var script = GD.Load<Script>("res://addons/EntityGridAddon/Grid/EntityGrid.cs");
		var texture = GD.Load<Texture2D>("res://addons/EntityGridAddon/Grid/grid.svg");
		
		AddCustomType(nameof(EntityGrid), nameof(GridContainer), script, texture);
		AddInspectorPlugin(_delegationInspector);
	}

	public override void _ExitTree()
	{
		RemoveInspectorPlugin(_delegationInspector);
		RemoveCustomType(nameof(EntityGrid));
	}
}
#endif
