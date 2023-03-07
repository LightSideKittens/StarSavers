using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace FastSpriteMask
{
    public static class MultiMaskHandler
    {
        private static readonly Dictionary<Texture2D, MaskDataHandler>[] MaskClusters =
        {
            new Dictionary<Texture2D, MaskDataHandler>(64),
            new Dictionary<Texture2D, MaskDataHandler>(64),
            new Dictionary<Texture2D, MaskDataHandler>(64),
            new Dictionary<Texture2D, MaskDataHandler>(64),

            new Dictionary<Texture2D, MaskDataHandler>(64),
            new Dictionary<Texture2D, MaskDataHandler>(64),
            new Dictionary<Texture2D, MaskDataHandler>(64),
            new Dictionary<Texture2D, MaskDataHandler>(64),
        };

        private static readonly RendererCluster[] RendererClusters =
        {
            new RendererCluster(new List<MultiMaskedSprite>(), new List<MultiMaskedSpriteShape>(), Array.Empty<Material>()),
            new RendererCluster(new List<MultiMaskedSprite>(), new List<MultiMaskedSpriteShape>(), Array.Empty<Material>()),
            new RendererCluster(new List<MultiMaskedSprite>(), new List<MultiMaskedSpriteShape>(), Array.Empty<Material>()),
            new RendererCluster(new List<MultiMaskedSprite>(), new List<MultiMaskedSpriteShape>(), Array.Empty<Material>()),

            new RendererCluster(new List<MultiMaskedSprite>(), new List<MultiMaskedSpriteShape>(), Array.Empty<Material>()),
            new RendererCluster(new List<MultiMaskedSprite>(), new List<MultiMaskedSpriteShape>(), Array.Empty<Material>()),
            new RendererCluster(new List<MultiMaskedSprite>(), new List<MultiMaskedSpriteShape>(), Array.Empty<Material>()),
            new RendererCluster(new List<MultiMaskedSprite>(), new List<MultiMaskedSpriteShape>(), Array.Empty<Material>()),
        };

        private static readonly RendererCluster[] SpriteShapeRendererClusters =
        {
            new RendererCluster(new List<MultiMaskedSprite>(), new List<MultiMaskedSpriteShape>(), Array.Empty<Material>()),
            new RendererCluster(new List<MultiMaskedSprite>(), new List<MultiMaskedSpriteShape>(), Array.Empty<Material>()),
            new RendererCluster(new List<MultiMaskedSprite>(), new List<MultiMaskedSpriteShape>(), Array.Empty<Material>()),
            new RendererCluster(new List<MultiMaskedSprite>(), new List<MultiMaskedSpriteShape>(), Array.Empty<Material>()),

            new RendererCluster(new List<MultiMaskedSprite>(), new List<MultiMaskedSpriteShape>(), Array.Empty<Material>()),
            new RendererCluster(new List<MultiMaskedSprite>(), new List<MultiMaskedSpriteShape>(), Array.Empty<Material>()),
            new RendererCluster(new List<MultiMaskedSprite>(), new List<MultiMaskedSpriteShape>(), Array.Empty<Material>()),
            new RendererCluster(new List<MultiMaskedSprite>(), new List<MultiMaskedSpriteShape>(), Array.Empty<Material>()),
        };

        internal static void UpdateMask()
        {
            foreach (var cluster in MaskClusters)
            {
                foreach (var mask in cluster)
                {
                    mask.Value.Update();
                }
            }
        }

        public static void MaskRegistration(IMultiMask mask)
        {
            if(mask.Sprite == null) return;
            #if UNITY_EDITOR
            if (!Application.isPlaying) MultiMaskUpdater.Init();
            #endif

            var masks = MaskClusters[(int)mask.Type];

            if (!masks.TryGetValue(mask.Sprite.texture, out var maskHandler))
            {
                maskHandler = new MaskDataHandler(mask);
                masks.Add(mask.Sprite.texture, maskHandler);
            }

            maskHandler.AddMask(mask);
        }

        public static void MaskCancellation(IMultiMask mask)
        {
            var maskData = mask.MultiMaskData;
            if (maskData == null) return;
            mask.MultiMaskData = null;

            var masks = MaskClusters[(int)maskData.Type];

            if (masks.TryGetValue(maskData.Texture, out var handler))
            {
                if (handler.RemoveMask(mask, maskData))
                {
                    masks.Remove(maskData.Texture);
                }
            }
        }

        internal static void MaterialRegistration(MaskType type, Material material, Texture2D texture)
        {
            var index = (int)type;
            RendererClusters[index].AddMaterial(material);

            var materials = RendererClusters[index].Materials;

            foreach (var sprite in RendererClusters[index].MaskedSprites)
            {
                sprite.Renderer.sharedMaterials = materials;
            }

            var spriteShapeMaterials = new []
            {
                material,
                material,
            };

            foreach (var sprite in RendererClusters[index].MaskedSpriteShapes)
            {
                if (sprite.TargetTexture == texture)
                {
                    sprite.Renderer.sharedMaterials = spriteShapeMaterials;
                }
            }
        }

        internal static void MaterialCancellation(MaskType type, Material material, Texture2D texture)
        {
            var index = (int)type;
            RendererClusters[index].RemoveMaterial(material);

            var materials = GetMaterials(type);

            foreach (var sprite in RendererClusters[index].MaskedSprites)
            {
                sprite.Renderer.sharedMaterials = materials;
            }

            var emptyMaterial = MaskMaterialHandler.DefaultMultiMaskMaterials[(int)type];
            var spriteShapeMaterials = new []
            {
                emptyMaterial,
                emptyMaterial,
            };

            foreach (var sprite in RendererClusters[index].MaskedSpriteShapes)
            {
                if (sprite.TargetTexture == texture)
                {
                    sprite.Renderer.sharedMaterials = spriteShapeMaterials;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SpriteRegistration(MultiMaskedSprite sprite)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) MultiMaskUpdater.Init();
#endif
            RendererClusters[(int)sprite.MaskType].MaskedSprites.Add(sprite);
            sprite.Renderer.sharedMaterials = GetMaterials(sprite.MaskType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SpriteRegistration(MultiMaskedSpriteShape sprite)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) MultiMaskUpdater.Init();
#endif
            RendererClusters[(int)sprite.MaskType].MaskedSpriteShapes.Add(sprite);
            var material = GetTargetMaterial(sprite.MaskType, sprite.TargetTexture);
            sprite.Renderer.sharedMaterials = new []
            {
                material,
                material,
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SpriteCancellation(MultiMaskedSpriteShape sprite)
        {
            RendererClusters[(int)sprite.MaskType].MaskedSpriteShapes.Remove(sprite);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SpriteCancellation(MultiMaskedSprite sprite)
        {
            RendererClusters[(int)sprite.MaskType].MaskedSprites.Remove(sprite);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Material[] GetMaterials(MaskType type)
        {
            var index = (int)type;
            if (RendererClusters[index].Materials.Length == 0)
            {
                return new [] { MaskMaterialHandler.DefaultMultiMaskMaterials[(int)type] };
            }
            else return RendererClusters[index].Materials;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Material GetTargetMaterial(MaskType type, Texture2D texture)
        {
            var index = (int)type;
            if (texture != null && MaskClusters[index].TryGetValue(texture, out var maskDataHandler))
            {
                return maskDataHandler.MaskData.Material;
            }
            return MaskMaterialHandler.DefaultMultiMaskMaterials[index];
        }

        private struct RendererCluster
        {
            public readonly List<MultiMaskedSprite> MaskedSprites;
            public readonly List<MultiMaskedSpriteShape> MaskedSpriteShapes;
            public Material[] Materials;

            public RendererCluster(List<MultiMaskedSprite> maskedSprites, List<MultiMaskedSpriteShape> maskedSpriteShapes, Material[] materials)
            {
                MaskedSprites = maskedSprites;
                MaskedSpriteShapes = maskedSpriteShapes;
                Materials = materials;
            }

            public void AddMaterial(Material material)
            {
                var lenght = Materials.Length;
                var newMaterials = new Material[lenght + 1];
                Array.Copy(Materials, newMaterials, lenght);
                newMaterials[lenght] = material;

                Materials = newMaterials;
            }

            public void RemoveMaterial(Material material)
            {
                var newLenght = Materials.Length - 1;
                var index = Array.IndexOf(Materials, material);
                Materials[index] = Materials[newLenght];

                var newMaterials = new Material[newLenght];
                Array.Copy(Materials, newMaterials, newLenght);

                Materials = newMaterials;
            }
        }
    }
}