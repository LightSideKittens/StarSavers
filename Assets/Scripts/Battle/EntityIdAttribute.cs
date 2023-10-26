using System;
using System.Diagnostics;
using Sirenix.OdinInspector;

[AttributeUsage(AttributeTargets.All)]
[Conditional("UNITY_EDITOR")]
public class EntityIdAttribute : ValueDropdownAttribute
{
    public string GroupName;

    public EntityIdAttribute(string name = "") : base(name)
    {
        GroupName = name;
        IsUniqueList = true;
    }
}
