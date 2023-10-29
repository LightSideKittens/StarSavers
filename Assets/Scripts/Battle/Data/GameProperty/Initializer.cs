using System.Reflection;
using UnityEditor;

namespace LSCore.LevelSystem
{
    [InitializeOnLoad]
    public class Initializer
    {
        static Initializer()
        {
            BaseGameProperty.AddAllTypesFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}