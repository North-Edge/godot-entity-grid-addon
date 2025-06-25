using NorthEdge.addons.EntityGridAddon.Inspector;
using System.Collections.Generic;
using NorthEdge.GameGrid;
using System;
using Godot;

namespace NorthEdge.addons.EntityGridAddon.Grid;

[Tool]
public partial class EntityGrid: GridContainer
{
	private PackedScene _entityScene;
	
	[Signal]
	public delegate void GridElementMouseEnteredEventHandler(Node node, InputEvent inputEvent);

	[Signal]
	public delegate void GridElementAddedEventHandler(Node node);

	[Signal]
	public delegate void GridElementRemovedEventHandler(Node node);

	[Export] 
	private PackedScene EntityScene
	{
		get => _entityScene;
		set => SetEntityScene(value);
	}

	[Export]
	private Button RefreshButton { get; set; }

	private EntityGrid SetEntityScene(PackedScene value)
	{
		_entityScene = value;

		return Refresh();
	}

	private int _rows = 5;
	private int _columns = 5;

	public readonly Random Random = new(Guid.NewGuid().GetHashCode()); 

	[ExportGroup("Dimensions")]
	[Export(PropertyHint.Range, "1,99,1")]
	public int Rows
	{
		get => _rows;
		set {
			_rows = value;
			_Ready();
		}
	}

	[ExportGroup("Dimensions")]
	[Export(PropertyHint.Range, "1,99,1")]
	public new int Columns
	{
		get => _columns;
		set {
			_columns = value;
			_Ready();
		}
	}

	private readonly Grid2d<IGridEntity> _grid;

	public EntityGrid()
	{
		_grid = new Grid2d<IGridEntity>(null, OnElementAdded, OnElementRemoved);
	}
	
	public override void _Ready()
	{
		Refresh();
		QueueRedraw();
	}

	private void Resize(int rows, int columns)
	{
		_columns = columns;
		_rows = rows;
	}

	public EntityGrid SetClamp(Func<IGridEntity, IGridEntity> clamp, bool applyNow)
	{
		_grid.SetClamp(clamp, applyNow);

		return this;
	}
	
	private void OnElementRemoved(object sender, Grid2d<IGridEntity>.ElementEventArgs e)
	{
		if (e.element is Node node)
		{
			EmitSignal(SignalName.GridElementRemoved, node);
			RemoveChild(node);
			node.QueueFree();
		}
	}

	private void OnElementAdded(object sender, Grid2d<IGridEntity>.ElementEventArgs e)
	{
		if (EntityScene != null && e.element == null && sender is Grid2d<IGridEntity> grid)
		{
			var entity = EntityScene.Instantiate();

			if (entity is IGfxEntity gfxEntity)
			{
				gfxEntity.Position = new Vector2(e.i * (48 + 4), e.j * (48 + 4));
				gfxEntity.X = e.i;
				gfxEntity.Y = e.j;

				grid[e.i, e.j] = gfxEntity;
			}

			AddChild(entity);
			EmitSignal(SignalName.GridElementAdded, entity);
		}
	}

	private bool _ParseProperty(EditorInspectorPlugin inspectorPlugin, PropertyInfo propertyInfo)
	{
		if (propertyInfo.Name == nameof(RefreshButton))
		{
			var button = new Button();

			button.ButtonUp += RefreshGrid;
			button.Text = "Refresh";
			
			inspectorPlugin.AddPropertyEditor(propertyInfo.Name, button, false);

			return true;
		}

		// hide the columns property (use our own)
		return propertyInfo.Name.Equals(nameof(Columns).ToLower());
	}

	public IList<IGfxEntity> GetNeighbours(IList<Vector2I> neighbours, IGfxEntity entity, bool matchState = true, 
										   Func<IGfxEntity, IGfxEntity> onEntityAdded = null)
	{
		var result = new List<IGfxEntity>();

		foreach (var coords in neighbours)
		{
			var i = entity.X + coords.X;
			var j = entity.Y + coords.Y;

			if (i >= 0 && j >= 0 && j < Rows && i < Columns)
			{
				var neighbour = _grid[i, j];

				if (neighbour is IGfxEntity gridEntity && (!matchState || gridEntity.State == entity.State))
				{
					result.Add(gridEntity);
					
					if (onEntityAdded != null)
						onEntityAdded(gridEntity);
				}
			}
		}
		
		return result;
	}

	public IEnumerable<(int i, int j, IGridEntity element)> Iterate()
	{
		return _grid.Iterate();
	}

	private void RefreshGrid()
	{
		_grid.Clear();
		_grid.Resize(_rows, _columns);
	}

	public EntityGrid Refresh()
	{
		RefreshGrid();

		return this;
	}
}
