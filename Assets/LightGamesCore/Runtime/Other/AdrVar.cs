using UnityEngine;
using UnityEngine.Scripting;

public static class AdrVar
{
    [Preserve]
    public static string ServerName => Application.productName.ToLower().Replace(" ", "-")
#if DEBUG
    + "-dev"
#endif
    ;
    
}