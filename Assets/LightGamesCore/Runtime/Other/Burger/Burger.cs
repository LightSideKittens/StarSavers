using System.Collections.Generic;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public static class Burger
{
    private static readonly HashSet<string> tags = new HashSet<string>();
    
    [Conditional("DEBUG")]
    public static void RegisterTag()
    {
        UniTrace.Create(1).ClassName(out var className);
        tags.Add(className);
    }
    
    [Conditional("DEBUG")]
    public static void Log(object log)
    {
        Debug.Log(TryModifyLog(log));
    }

    [Conditional("DEBUG")]
    public static void Error(object log)
    {
        Debug.LogError(TryModifyLog(log));
    }

    [Conditional("DEBUG")]
    public static void Warning(object log)
    {
        Debug.LogWarning(TryModifyLog(log));
    }

    private static string TryModifyLog(object log)
    {
        UniTrace.Create(2).ClassName(out var className);
        if (tags.Contains(className))
        {
            return $"[{className}] {log}";
        }

        return $"{log}";
    }
}