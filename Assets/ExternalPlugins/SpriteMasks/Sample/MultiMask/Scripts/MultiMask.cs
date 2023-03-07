using UnityEngine;

namespace FastSpriteMask
{
    [ExecuteInEditMode]
    public class MultiMask : MonoBehaviour, IMultiMask
    {
        public Sprite maskSprite;
        public MaskType maskType;
        [Range(0, 1)]
        public float alphaCutoff = 0.2f;
        
        public Transform Transform { get; private set; }
        public Sprite Sprite => maskSprite;
        public MaskType Type => maskType;
        public float AlphaCutoff => alphaCutoff;
        public int DataIndex { get; set; }
        public IMultiMaskData MultiMaskData { get; set; }

        private void Awake()
        {
            Transform = transform;
        }

        private void OnEnable()
        {
#if UNITY_EDITOR
            _oldSprite = Sprite;
            _oldType = Type;
#endif
            MultiMaskHandler.MaskRegistration(this);
        }

        private void OnDisable()
        {
            MultiMaskHandler.MaskCancellation(this);
        }

#if UNITY_EDITOR
        [SerializeField, HideInInspector]
        private Sprite _oldSprite;
        [SerializeField, HideInInspector]
        private MaskType _oldType;

        private void OnValidate()
        {
            var isSpriteChanged = _oldSprite != Sprite;
            var isTypeChanged = _oldType != Type;
            var currentType = Type;

            if (isSpriteChanged)
            {
                _oldSprite = Sprite;
            }
            if (isTypeChanged)
            {
                maskType = _oldType;
                _oldType = currentType;
            }
            if ((isTypeChanged || isSpriteChanged) && enabled)
            {
                MultiMaskHandler.MaskCancellation(this);
                maskType = currentType;
                MultiMaskHandler.MaskRegistration(this);
            }
        }
#endif
    }
}
