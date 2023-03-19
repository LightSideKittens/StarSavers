using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class Burger
{
    [Conditional("DEBUG")]
    public static void Log(string log)
    {
        Debug.Log(log);
    }

    [Conditional("DEBUG")]
    public static void Error(string log)
    {
        Debug.LogError(log);
    }

    [Conditional("DEBUG")]
    public static void Warning(string log)
    {
        Debug.LogWarning(log);
    }
}