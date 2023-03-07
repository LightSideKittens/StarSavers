using UnityEngine;

namespace FastSpriteMask
{
    public interface IMultiMaskData
    {
        Material Material { get; }
        MaskType Type { get; }
        Texture2D Texture { get; }
        bool IsEmpty { get; }
        
        void AddMask(IMultiMask mask);
        
        void RemoveMask(IMultiMask mask);
        
        void UpdateState();

        void Destroy();
    }
}
