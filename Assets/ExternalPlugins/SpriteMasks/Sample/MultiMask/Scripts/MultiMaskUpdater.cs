using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Unity.Jobs;
using UnityEngine.Jobs;
using UnityEditor;

namespace FastSpriteMask
{
    [ExecuteInEditMode]
    public class MultiMaskUpdater : MonoBehaviour
    {
        private static MultiMaskUpdater Instance;
        public static MultiMaskUpdater Get
        {
            get
            {
                Init();
                return Instance;
            }
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad), MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Init()
        {
            if (Instance != null) return;
            
            var updaters = FindObjectsOfType<MultiMaskUpdater>();
            if (updaters.Length > 0)
            {
                Instance = updaters[0];

                for (var i = 1; i < updaters.Length; i++)
                {
                    DestroyImmediate(updaters[i]);
                }
                return;
            }
            
            Instance = new GameObject("MultiMaskUpdater") { hideFlags = HideFlags.HideInHierarchy }.AddComponent<MultiMaskUpdater>();
        }
        
        private void Start()
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
#endif
                DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            MultiMaskHandler.UpdateMask();
        }
    }
}
