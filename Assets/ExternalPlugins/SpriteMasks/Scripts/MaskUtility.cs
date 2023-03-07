using System.Runtime.CompilerServices;
using UnityEngine;

namespace FastSpriteMask
{
    public static class MaskUtility
    {
        private static readonly int TexId     = Shader.PropertyToID("_MaskTex");
        private static readonly int RectMinId = Shader.PropertyToID("_RectMin");
        private static readonly int RectMaxId = Shader.PropertyToID("_RectMax");
        private static readonly int PosId     = Shader.PropertyToID("_MaskPosition");
        private static readonly int ScaleId   = Shader.PropertyToID("_MaskScale");
        private static readonly int RadId     = Shader.PropertyToID("_MaskAngle");
        private static readonly int AlphaCutoffId     = Shader.PropertyToID("_AlphaCutoff");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetTex(Material material, Texture2D texture)
        {
            material.SetTexture(TexId, texture);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetSprite(Material material, IMask mask)
        {
            SetSprite(material, mask.Sprite);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetSprite(Material material, Sprite sprite)
        {
            material.SetTexture(TexId, sprite.texture);
            material.SetVector(RectMinId, sprite.textureRect.position);
            material.SetVector(RectMaxId, sprite.textureRect.max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetTransform(Material material, IMask mask)
        {
            SetTransform(material, mask.Sprite, mask.Transform);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetPosition(Material material, IMask mask)
        {
            SetPosition(material, mask.Transform.position);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetScale(Material material, IMask mask)
        {
            SetScale(material, mask.Sprite, mask.Transform.lossyScale);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetRotation(Material material, IMask mask)
        {
            SetDeg(material, mask.Transform.rotation.eulerAngles.z);
        }

        public static void SetTransform(Material material, Sprite sprite, Transform tr)
        {
            var pos = tr.position;
            var scale = tr.lossyScale / sprite.pixelsPerUnit;
            var rad = tr.rotation.eulerAngles.z * Mathf.Deg2Rad;

            material.SetVector(PosId, pos);
            material.SetVector(ScaleId, scale);
            material.SetFloat(RadId, rad);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetPosition(Material material, Vector2 pos)
        {
            material.SetVector(PosId, pos);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetScale(Material material, Sprite sprite, Vector2 scale)
        {
            material.SetVector(ScaleId, scale / sprite.pixelsPerUnit);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetScale(Material material, Vector2 scale)
        {
            material.SetVector(ScaleId, scale);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetDeg(Material material, float deg)
        {
            SetRad(material, deg * Mathf.Deg2Rad);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetRad(Material material, float rad)
        {
            material.SetFloat(RadId, rad);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetAlphaCutoff(Material material, float alphaCutoff)
        {
            material.SetFloat(AlphaCutoffId, alphaCutoff);
        }
    }
}