using Godot.Collections;
using Godot;

namespace NorthEdge.addons.EntityGridAddon.Inspector;

[Tool]
public partial class PropertyInfo(Variant.Type type, string name, PropertyHint hint, 
                                  string hintString, PropertyUsageFlags usage)
    : RefCounted
{
    public string Name { get; } = name;

    public PropertyHint Hint { get; } = hint;

    public Variant.Type Type { get; } = type;

    public string HintString { get; } = hintString;

    public PropertyUsageFlags Usage { get; } = usage;
    
    
    public static implicit operator Dictionary(PropertyInfo propertyInfo)
    {
        return new Dictionary {
            { nameof(name),        propertyInfo.Name       },
            { nameof(type),        (int)propertyInfo.Type  },
            { nameof(hint),        (int)propertyInfo.Hint  },
            { nameof(hintString),  propertyInfo.HintString },
            { nameof(usage),       (int)propertyInfo.Usage },
        };
    }

    public PropertyInfo() : this(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.None) {}
}