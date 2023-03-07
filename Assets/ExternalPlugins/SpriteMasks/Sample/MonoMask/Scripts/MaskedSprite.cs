using UnityEngine;

namespace FastSpriteMask
{
    [RequireComponent(typeof(Renderer)), ExecuteInEditMode]
    public class MaskedSprite : MonoBehaviour
    {
        [Tooltip("Material that applied if mask is disabled")]
        public Material UnmaskedMaterial;
        [SerializeField]
        private Mask Mask;

        internal Renderer Renderer;

        private void OnEnable()
        {
            Renderer = GetComponent<Renderer>();
            ApplyMask();
        }

        private void OnDisable()
        {
            if (Renderer == null)
                return;

            var material = UnmaskedMaterial != null ?
                UnmaskedMaterial :
                (Mask == null || Mask.Type.IsUnlit() ?
                    MaskMaterialHandler.DefaultUnlit :
                    MaskMaterialHandler.DefaultLit);

            ApplyMaterial(material);

#if UNITY_EDITOR
            if(Mask != null) Mask.OnMaterialChanged -= ApplyMask;
#endif
        }

        private void ApplyMask()
        {
            if (Renderer == null || Mask == null)
                return;

            ApplyMaterial(Mask.GetMaterial());

#if UNITY_EDITOR
            Mask.OnMaterialChanged -= ApplyMask;
            Mask.OnMaterialChanged += ApplyMask;
#endif
        }

        protected virtual void ApplyMaterial(Material material)
        {
            Renderer.sharedMaterial = material;
        }

#if UNITY_EDITOR

        [SerializeField, HideInInspector]
        private Mask OldMask;
        private void OnValidate()
        {
            if (enabled) ApplyMask();

            if (OldMask != Mask)
            {
                if(OldMask != null) OldMask.OnMaterialChanged -= ApplyMask;
                OldMask = Mask;
            }
        }

#endif
    }
}