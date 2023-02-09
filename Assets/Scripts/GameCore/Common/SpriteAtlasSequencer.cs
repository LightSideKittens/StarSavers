using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

namespace BeatRoyale
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteAtlasSequencer : MonoBehaviour
    {
        [Serializable]
        private struct AnimationData
        {
            private SpriteRenderer spriteRenderer;
            [ReadOnly] public SpriteAtlas spriteAtlas;
            private Sprite[] sprites;
            private int currentSprite;
            private float timePerFrame;
            private float time;

            public void Init(SpriteRenderer spriteRenderer, int framePerSecond)
            {
                this.spriteRenderer = spriteRenderer;
                sprites = new Sprite[spriteAtlas.spriteCount];
                for (int i = 0; i < spriteAtlas.spriteCount; i++)
                {
                    var name = (i+1).ToString("0000");
                    sprites[i] = spriteAtlas.GetSprite(name);
                }

                timePerFrame = 1f / framePerSecond;
            }
            
            public void Update()
            {
                time += Time.deltaTime;
                
                if (time > timePerFrame)
                {
                    spriteRenderer.sprite = sprites[currentSprite++ % sprites.Length];
                    time = 0;
                }
            }
        }
        
        [SerializeField] private string animName;
        [SerializeField] private int framePerSecond = 24;
        [SerializeField] private AnimationData forward;
        [SerializeField] private AnimationData forwardLeft;
        [SerializeField] private AnimationData forwardRight;
        [SerializeField] private AnimationData back;
        [SerializeField] private AnimationData backLeft;
        [SerializeField] private AnimationData backRight;
        [SerializeField] private AnimationData left;
        [SerializeField] private AnimationData right;

        [Button("Configure")] 
        void Configure()
        {
            forward.spriteAtlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>($"Assets/{animName}/F/{animName}_F.spriteatlas");
            forwardLeft.spriteAtlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>($"Assets/{animName}/FL/{animName}_FL.spriteatlas");
            forwardRight.spriteAtlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>($"Assets/{animName}/FR/{animName}_FR.spriteatlas");
            back.spriteAtlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>($"Assets/{animName}/B/{animName}_B.spriteatlas");
            backLeft.spriteAtlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>($"Assets/{animName}/BL/{animName}_BL.spriteatlas");
            backRight.spriteAtlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>($"Assets/{animName}/BR/{animName}_BR.spriteatlas");
            left.spriteAtlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>($"Assets/{animName}/L/{animName}_L.spriteatlas");
            right.spriteAtlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>($"Assets/{animName}/R/{animName}_R.spriteatlas");
        }
        
        private void Awake()
        {
            var spriteRenderer = GetComponent<SpriteRenderer>();
            forward.Init(spriteRenderer, framePerSecond);
            forwardLeft.Init(spriteRenderer, framePerSecond);
            forwardRight.Init(spriteRenderer, framePerSecond);
            back.Init(spriteRenderer, framePerSecond);
            backLeft.Init(spriteRenderer, framePerSecond);
            backRight.Init(spriteRenderer, framePerSecond);
            left.Init(spriteRenderer, framePerSecond);
            right.Init(spriteRenderer, framePerSecond);
            WASDEvents.Forward += forward.Update;
            WASDEvents.ForwardLeft += forwardLeft.Update;
            WASDEvents.ForwardRight += forwardRight.Update;
            WASDEvents.Back += back.Update;
            WASDEvents.BackLeft += backLeft.Update;
            WASDEvents.BackRight += backRight.Update;
            WASDEvents.Left += left.Update;
            WASDEvents.Right += right.Update;
        }
    }
}
