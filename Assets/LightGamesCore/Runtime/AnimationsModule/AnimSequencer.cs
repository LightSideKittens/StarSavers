using System;
using System.Collections.Generic;
using DG.Tweening;
using LGCore.AnimationsModule.Animations;
using LGCore.AnimationsModule.Animations.Options;
using LGCore;
using LGCore.Runtime;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace LGCore.AnimationsModule
{
    [Serializable]
    public class AnimSequencer : ISerializationCallbackReceiver
    {
        [Serializable]
        private struct AnimData
        {
            [TableColumnWidth(60, false)]
            public float timeOffset;
            
            [TableColumnWidth(60, false)]
            [NonSerialized]
            [ShowInInspector]
            [ReadOnly]
            public float time;

            [HideInInspector]
            public string guid;

            [TableColumnWidth(400)]
            [SerializeReference] public BaseAnim anim;
        }

        [NonSerialized]
        [ShowInInspector]
        [ReadOnly]
        public float totalTime;
        
        [SerializeReference] private IOptions[] options;
        
        [TableList]
        [SerializeField] private AnimData[] animsData;

        public Dictionary<string, BaseAnim> AnimsByGuid { get; } = new();

        public BaseAnim GetAnim(string guid) => AnimsByGuid[guid];

#if UNITY_EDITOR
        [OnInspectorGUI]
        private void OnGUI()
        {
            if (animsData is {Length: > 0})
            {
                var currentTime = 0f;
                
                for (int i = 0; i < animsData.Length; i++)
                {
                    var data = animsData[i];

                    if (string.IsNullOrEmpty(data.guid))
                    {
                        data.guid = GUID.Generate().ToString();
                    }
                    
                    currentTime += data.timeOffset;
                    data.time = currentTime;
                    animsData[i] = data;
                }

                totalTime = currentTime;
            }
        }
#endif

        public void InitAllAnims()
        {
            for (int i = 0; i < animsData.Length; i++)
            {
                animsData[i].anim.TryInit();
            }
        }

        [Button("Animate")]
        public void Editor_Animate()
        {
            if (Application.isPlaying)
            {
                Animate();
            }
        }
        
        public Sequence Animate()
        {
            var currentTime = 0f;
            var sequence = DOTween.Sequence().SetId(this);

            for (int i = 0; i < animsData.Length; i++)
            {
                var data = animsData[i];
                var anim = data.anim;
                anim.TryInit();
                
                if (!anim.IsDurationZero)
                {
                    currentTime += data.timeOffset;
                    sequence.Insert(currentTime, anim.Animate());
                }
            }

            if (options != null)
            {
                for (int i = 0; i < options.Length; i++)
                {
                    options[i].ApplyTo(sequence);
                }
            }

            return sequence;
        }

        public void Kill()
        {
            DOTween.Kill(this);
        }
        
        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize()
        {
            if (animsData != null && World.IsPlaying)
            {
                for (int i = 0; i < animsData.Length; i++)
                {
                    var data = animsData[i];
                    AnimsByGuid.Add(data.guid, data.anim);
                }
            }
        }
    }
}