using System;
using UnityEngine;

namespace Core.Server
{
    public static class DelegateExtensions
    {
        public static void SafeInvoke(this Action action)
        {
            try { action?.Invoke(); }
            catch (Exception e) { Debug.LogException(e); }
        }
        
        public static void SafeInvoke<T>(this Action<T> action, T data)
        {
            try { action?.Invoke(data); }
            catch (Exception e) { Debug.LogException(e); }
        }
        
        public static void SafeInvoke<T, T1>(this Action<T, T1> action, T data, T1 data1)
        {
            try { action?.Invoke(data, data1); }
            catch (Exception e) { Debug.LogException(e); }
        }
    }
}