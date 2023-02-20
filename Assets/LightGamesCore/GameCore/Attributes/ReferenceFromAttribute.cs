using System.Diagnostics;
using UnityEngine;
using static EditorUtils;

[Conditional("UNITY_EDITOR")]
public class ReferenceFromAttribute : PropertyAttribute
{
    public string PrefabName { get; }
    public static int Index { get; private set; }
    public int MyIndex { get; }
    public ColorData ColorData { get; }

    public ReferenceFromAttribute(string prefabName)
    {
        PrefabName = prefabName;
        Index %= Colors.Length;
        ColorData = Colors[Index];
        MyIndex = Index;
        Index++;
    }
}
