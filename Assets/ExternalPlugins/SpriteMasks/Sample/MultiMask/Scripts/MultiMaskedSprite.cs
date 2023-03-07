using UnityEngine;

namespace FastSpriteMask
{
    [RequireComponent(typeof(Renderer)), ExecuteInEditMode]
    public class MultiMaskedSprite : MonoBehaviour
    {
        [Tooltip("Material that applied if mask is disabled")]
        public Material UnmaskedMaterial;

        public MaskType MaskType;

        internal Renderer Renderer;

        private void OnEnable()
        {
#if UNITY_EDITOR
            _oldMaskType = MaskType;
#endif
            Renderer = GetComponent<Renderer>();
            SpriteRegistration();
        }

        private void OnDisable()
        {
            SpriteCancellation();

            var material = UnmaskedMaterial != null ?
                UnmaskedMaterial :
                (MaskType.IsUnlit() ?
                    MaskMaterialHandler.DefaultUnlit :
                    MaskMaterialHandler.DefaultLit);
            ApplyMaterial(material);
        }

        protected virtual void SpriteRegistration()
        {
            MultiMaskHandler.SpriteRegistration(this);
        }

        protected virtual void SpriteCancellation()
        {
            MultiMaskHandler.SpriteCancellation(this);
        }

        protected virtual void ApplyMaterial(Material material)
        {
            Renderer.sharedMaterial = material;
        }

#if UNITY_EDITOR
        [SerializeField, HideInInspector]
        private MaskType _oldMaskType;
        protected virtual void OnValidate()
        {
            if (_oldMaskType != MaskType)
            {
                MultiMaskHandler.SpriteCancellation(this);
                _oldMaskType = MaskType;
                MultiMaskHandler.SpriteRegistration(this);
            }
        }
#endif
    }
}