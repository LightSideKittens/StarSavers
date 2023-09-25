using System.Diagnostics;

namespace LGCore.ConfigModule.Editor
{
    public static partial class ConfigUtils
    {
        [Conditional("UNITY_EDITOR")]
        public static void Save<T>() where T : BaseConfig<T>, new()
        {
            BaseConfig<T>.Save();
        }
    }
}