using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FastSpriteMask
{
    public class MultiMaskData : IMultiMaskData
    {
        public Material Material { get; }
        public Texture2D Texture { get; }
        public MaskType Type { get; }

        private readonly bool _isHard;
        private readonly Texture2D _dataTexture;
        private readonly List<int> _rectCounter;
        private readonly List<IMultiMask> _masks;
        private float[] _data;

        public bool IsEmpty => _masks.Count == 0;

        public MultiMaskData(IMultiMask mask)
        {
            Type = mask.Type;
            Texture = mask.Sprite.texture;

            (_data, _rectCounter, _dataTexture, _masks) = MultiMaskUtility.CreateMaskData();

            Material = MaskMaterialHandler.GetMultiMaskInstance(Type);
            Material.SetMaskTexture(mask);
            MultiMaskHandler.MaterialRegistration(Type, Material, Texture);

            _isHard = Type.IsHard();
        }

        public void UpdateState()
        {
            _masks.UpdateState(_data, _isHard);
            Material.SetMaskData(_dataTexture, _data);
        }

        public void UpdatePosition()
        {
            _masks.UpdatePosition(_data);
            Material.SetMaskData(_dataTexture, _data);
        }

        public void UpdateScale()
        {
            _masks.UpdateScale(_data);
            Material.SetMaskData(_dataTexture, _data);
        }

        public void UpdateRotation()
        {
            _masks.UpdateRotation(_data);
            Material.SetMaskData(_dataTexture, _data);
        }

        public void UpdateAlphaCutoff()
        {
            _masks.UpdateAlphaCutoff(_data);
            Material.SetMaskData(_dataTexture, _data);
        }

        public void AddMask(IMultiMask mask)
        {
            while (!_masks.AddMask(_data, _rectCounter, mask))
            {
                MultiMaskUtility.Expand(ref _data, _dataTexture, _rectCounter.Count);
            }

            mask.MultiMaskData = this;
        }

        public void RemoveMask(IMultiMask mask)
        {
            _masks.RemoveMask(_data, _rectCounter, mask);

            mask.MultiMaskData = null;
        }

        public void Destroy()
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                Object.Destroy(_dataTexture);
                Object.Destroy(Material);
            }
            else
            {
                Object.DestroyImmediate(_dataTexture);
                Object.DestroyImmediate(Material);
            }
#else
            Object.Destroy(_dataTexture);
            Object.Destroy(Material);
#endif
        }
    }
}