using System;
using UnityEngine;

namespace LGCore
{
    public static class EntryPoint
    {
        public static event Action Inited;
        private static bool isInited;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void BeforeSceneLoad()
        {
            Inited?.Invoke();
            isInited = true;
        }

        public static void TryCallAndListen(Action action)
        {
            if (isInited) action();

            Inited += action;
        }

        public static bool OnInit(Action action)
        {
            if (isInited)
            {
                action();
                return false;
            }

            Inited += action;
            return true;
        }
    }
}