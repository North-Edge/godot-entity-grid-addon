#if TOOLS

using Godot;

namespace NorthEdge.addons.EntityGridAddon.Inspector;

[Tool]
public partial class DelegationInspector: EditorInspectorPlugin
{
	/// <inheritdoc cref="EditorInspectorPlugin"/>
	public override bool _CanHandle(GodotObject godotObject)
	{
		return true;
	}

	/// <inheritdoc cref="EditorInspectorPlugin"/>
	public override void _ParseBegin(GodotObject godotObject)
	{
		if (godotObject.HasMethod("_ParseBegin"))
			godotObject.Call("_ParseBegin", this);
	}

	/// <inheritdoc cref="EditorInspectorPlugin"/>
	public override void _ParseCategory(GodotObject godotObject, string category)
	{
		if (godotObject.HasMethod("_ParseCategory"))
			godotObject.Call("_ParseCategory", this, category);
	}

	/// <inheritdoc cref="EditorInspectorPlugin"/>
	public override bool _ParseProperty(GodotObject godotObject, Variant.Type type, string name, PropertyHint hintType, 
										string hintString, PropertyUsageFlags usageFlags, bool wide)
	{
		if (godotObject != null)
		{
			var propertyInfo = new PropertyInfo(type, name, hintType, hintString, usageFlags);

			return godotObject.HasMethod("_ParseProperty") && godotObject.Call("_ParseProperty", this, propertyInfo).AsBool();
		}

		return false;
	}

	/// <inheritdoc cref="EditorInspectorPlugin"/>
	public override void _ParseEnd(GodotObject godotObject)
	{
		if (godotObject is EditorInspectorPlugin editorPlugin)
			editorPlugin._ParseEnd(this);
	}
}
#endif
