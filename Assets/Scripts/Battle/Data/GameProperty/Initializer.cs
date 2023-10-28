using System.Reflection;
using UnityEditor;

namespace Battle.Data.GameProperty
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