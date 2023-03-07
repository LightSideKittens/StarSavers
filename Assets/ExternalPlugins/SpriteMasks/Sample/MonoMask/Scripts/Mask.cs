using System;
using UnityEngine;

namespace FastSpriteMask
{
    [ExecuteInEditMode]
    public class Mask : MonoBehaviour, IMask
    {
        [SerializeField, HideInInspector]
        private Transform _transform;
        private Material _material;
        
        public Sprite maskSprite;
        public MaskType maskType;
        [Range(0, 1)]
        public float alphaCutoff = 0.2f;

        public Transform Transform => _transform;
        public Sprite Sprite => maskSprite;
        public MaskType Type => maskType;
        public float AlphaCutoff => alphaCutoff;
        
        private void OnEnable()
        {
            _transform = transform;
#if UNITY_EDITOR
            _oldType = Type;
#endif
        }
        
        private void Update()
        {
            if (_material != null && maskSprite != null)
            {
                MaskUtility.SetTransform(_material, this);
                if ((int)Type < 4) MaskUtility.SetAlphaCutoff(_material, alphaCutoff);
            }
        }
        
        private void OnDisable()
        {
            if (_material != null) MaskUtility.SetScale(_material, Vector2.zero);
        }

        public Material GetMaterial()
        {
            if (_material == null)
            {
                _material = MaskMaterialHandler.GetMaskInstance(maskType);
                if (maskSprite != null) MaskUtility.SetSprite(_material, maskSprite);
            }
            return _material;
        }

#if UNITY_EDITOR
        [SerializeField, HideInInspector]
        public Action OnMaterialChanged;

        [SerializeField, HideInInspector]
        private Sprite _oldSprite;
        [SerializeField, HideInInspector]
        private MaskType _oldType;
        
        private void OnValidate()
        {
            if (maskSprite != _oldSprite)
            {
                _oldSprite = maskSprite;
                if (_material != null) MaskUtility.SetSprite(_material, maskSprite);
            }

            if (_oldType != Type)
            {
                _oldType = Type;
                
                _material = MaskMaterialHandler.GetMaskInstance(maskType);
                if (maskSprite != null) MaskUtility.SetSprite(_material, maskSprite);
                
                OnMaterialChanged?.Invoke();
            }
        }
#endif
    }
}
