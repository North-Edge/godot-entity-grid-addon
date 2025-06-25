#if TOOLS

using System.Collections.Generic;
using Godot.Collections;
using Godot;

namespace NorthEdge.addons.EntityGridAddon.Editors;

[Tool]
public partial class NeighboursEditor : EditorProperty
{
	private readonly Godot.Collections.Dictionary<NeighbourEnum, Vector2I> _coordinates = new() {
		{ NeighbourEnum.TopLeft,      Vector2I.Up + Vector2I.Left    },
		{ NeighbourEnum.TopCenter,    Vector2I.Up                    },
		{ NeighbourEnum.TopRight,     Vector2I.Up + Vector2I.Right   },
		{ NeighbourEnum.MiddleLeft,   Vector2I.Left                  },
		{ NeighbourEnum.MiddleCenter, Vector2I.Zero                  },
		{ NeighbourEnum.MiddleRight,  Vector2I.Right                 },
		{ NeighbourEnum.BottomLeft,   Vector2I.Down + Vector2I.Left  },
		{ NeighbourEnum.BottomCenter, Vector2I.Down                  },
		{ NeighbourEnum.BottomRight,  Vector2I.Down + Vector2I.Right }
	};

	private readonly Godot.Collections.Dictionary<NeighbourEnum, CheckBox> _checkBoxes = new();
	private readonly GridContainer _gridContainer;
	private Array<Vector2I> _neighbours;
	private bool _updating;
	
	public NeighboursEditor(): this(null) {}

	public NeighboursEditor(Array<Vector2I> neighbours = null)
	{
		_gridContainer = new GridContainer();
		_neighbours = neighbours ?? [];

		foreach (var keyValue in _coordinates)
		{
			var isCenter = keyValue.Key == NeighbourEnum.MiddleCenter;
			var checkbox = new CheckBox {
				TooltipText = keyValue.Key.ToString(),
				ButtonPressed = isCenter,
				Disabled = isCenter,
				ToggleMode = true
			};

			checkbox.Toggled += on => OnCheckBoxToggle(keyValue, on);
			_checkBoxes[keyValue.Key] = checkbox;
			_gridContainer.AddChild(checkbox);
			_gridContainer.Columns = 3;

			AddFocusable(checkbox);
		}
		
		AddChild(_gridContainer);
	}

	public override void _UpdateProperty()
	{
		_neighbours = GetEditedObject().Get(GetEditedProperty()).As<Array<Vector2I>>();
		_updating = true;

		foreach (var keyValue in _coordinates)
		{
			if (keyValue.Key == NeighbourEnum.MiddleCenter)
				continue;

			if (_checkBoxes.TryGetValue(keyValue.Key, out var checkBox))
			{
				checkBox.ButtonPressed = _neighbours.Contains(keyValue.Value);
			}
		}
		_updating = false;
	}

	private void OnCheckBoxToggle(KeyValuePair<NeighbourEnum, Vector2I> neighbour,  bool toggledOn)
	{
		if (_updating)
			return;

		if (toggledOn)
		{
			if (_neighbours.Contains(neighbour.Value) == false)
			{
				_neighbours.Add(neighbour.Value);
				EmitChanged(GetEditedProperty(), _neighbours);
			}
		}
		else
		{
			_neighbours.Remove(neighbour.Value);
			EmitChanged(GetEditedProperty(), _neighbours);
		}
	}
}
#endif