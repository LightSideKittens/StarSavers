using System.Runtime.CompilerServices;

namespace FastSpriteMask
{
    public readonly struct MaskDataHandler
    {
        public readonly IMultiMaskData MaskData;

        public bool IsEmpty => MaskData.IsEmpty;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MaskDataHandler(IMultiMask mask)
        {
            MaskData = new MultiMaskData(mask);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Update()
        {
            MaskData.UpdateState();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddMask(IMultiMask mask)
        {
            MaskData.AddMask(mask);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool RemoveMask(IMultiMask mask, IMultiMaskData maskData)
        {
            maskData.RemoveMask(mask);

            if (maskData.IsEmpty)
            {
                MultiMaskHandler.MaterialCancellation(maskData.Type, maskData.Material, maskData.Texture);
                maskData.Destroy();
                return true;
            }

            return false;
        }
    }
}