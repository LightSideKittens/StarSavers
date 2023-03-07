using UnityEngine;

namespace FastSpriteMask
{
    public class MultiMaskedSpriteShape : MultiMaskedSprite
    {
        public Sprite targetSprite;

        public Texture2D TargetTexture
        {
            get
            {
                if (targetSprite == null)
                    return null;
                return targetSprite.texture;
            }
        }

        protected override void SpriteRegistration()
        {
            MultiMaskHandler.SpriteRegistration(this);
        }

        protected override void SpriteCancellation()
        {
            MultiMaskHandler.SpriteCancellation(this);
        }

        protected override void ApplyMaterial(Material material)
        {
            Renderer.sharedMaterials = new []{ material, material };
        }

#if UNITY_EDITOR
        [SerializeField, HideInInspector]
        private Sprite _oldTargetSprite;
        protected override void OnValidate()
        {
            base.OnValidate();
            if (_oldTargetSprite != targetSprite)
            {
                MultiMaskHandler.SpriteCancellation(this);
                _oldTargetSprite = targetSprite;
                MultiMaskHandler.SpriteRegistration(this);
            }
        }
#endif
    }
}