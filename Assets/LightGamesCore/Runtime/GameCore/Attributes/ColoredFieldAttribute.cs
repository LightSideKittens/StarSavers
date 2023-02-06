using System.Diagnostics;
using Sirenix.OdinInspector;
using UnityEditor;
using static EditorUtils;

namespace GameCore.Attributes
{
    [Conditional("UNITY_EDITOR")]
    public class ColoredFieldAttribute : PropertyGroupAttribute
    {
        public static int index;
        public int myIndex;
        public bool isModified;
        public ColorData colorData;

        private void GetGUID(ref string guid)
        {
            #if UNITY_EDITOR
            guid = GUID.Generate().ToString();
            #endif
        }

        public ColoredFieldAttribute() : base("d")
        {
            var guid = string.Empty;
            GetGUID(ref guid);
            GroupID = guid;
            index %= Colors.Length;
            colorData = Colors[index];
            myIndex = index;
            index++;
        }
    }
}