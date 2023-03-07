using UnityEngine;

namespace FastSpriteMask
{
    public static class MaskMaterialHandler
    {
        private static readonly Shader[] MaskShaders =
        {
            Shader.Find("SpriteMasks/Mono/Sprite-Mask-Hard-Inside-Lit"),
            Shader.Find("SpriteMasks/Mono/Sprite-Mask-Hard-Inside-Unlit"),
            Shader.Find("SpriteMasks/Mono/Sprite-Mask-Hard-Outside-Lit"),
            Shader.Find("SpriteMasks/Mono/Sprite-Mask-Hard-Outside-Unlit"),

            Shader.Find("SpriteMasks/Mono/Sprite-Mask-Soft-Inside-Lit"),
            Shader.Find("SpriteMasks/Mono/Sprite-Mask-Soft-Inside-Unlit"),
            Shader.Find("SpriteMasks/Mono/Sprite-Mask-Soft-Outside-Lit"),
            Shader.Find("SpriteMasks/Mono/Sprite-Mask-Soft-Outside-Unlit"),
        };

        private static readonly Shader[] MultiMaskShaders =
        {
            Shader.Find("SpriteMasks/Multi/Sprite-MultiMask-Hard-Inside-Lit"),
            Shader.Find("SpriteMasks/Multi/Sprite-MultiMask-Hard-Inside-Unlit"),
            Shader.Find("SpriteMasks/Multi/Sprite-MultiMask-Hard-Outside-Lit"),
            Shader.Find("SpriteMasks/Multi/Sprite-MultiMask-Hard-Outside-Unlit"),

            Shader.Find("SpriteMasks/Multi/Sprite-MultiMask-Soft-Inside-Lit"),
            Shader.Find("SpriteMasks/Multi/Sprite-MultiMask-Soft-Inside-Unlit"),
            Shader.Find("SpriteMasks/Multi/Sprite-MultiMask-Soft-Outside-Lit"),
            Shader.Find("SpriteMasks/Multi/Sprite-MultiMask-Soft-Outside-Unlit"),
        };

        public static readonly Material[] DefaultMaskMaterials =
        {
            new Material(Shader.Find("SpriteMasks/Mono/Sprite-Mask-Hard-Inside-Lit"))    { name = "Mask-Hard-Inside-Lit_Default", hideFlags = HideFlags.DontSave },
            new Material(Shader.Find("SpriteMasks/Mono/Sprite-Mask-Hard-Inside-Unlit"))  { name = "Mask-Hard-Inside-Unlit_Default", hideFlags = HideFlags.DontSave },
            new Material(Shader.Find("SpriteMasks/Mono/Sprite-Mask-Hard-Outside-Lit"))   { name = "Mask-Hard-Outside-Lit_Default", hideFlags = HideFlags.DontSave },
            new Material(Shader.Find("SpriteMasks/Mono/Sprite-Mask-Hard-Outside-Unlit")) { name = "Mask-Hard-Outside-Unlit_Default", hideFlags = HideFlags.DontSave },

            new Material(Shader.Find("SpriteMasks/Mono/Sprite-Mask-Soft-Inside-Lit"))    { name = "Mask-Soft-Inside-Lit_Default", hideFlags = HideFlags.DontSave },
            new Material(Shader.Find("SpriteMasks/Mono/Sprite-Mask-Soft-Inside-Unlit"))  { name = "Mask-Soft-Inside-Unlit_Default", hideFlags = HideFlags.DontSave },
            new Material(Shader.Find("SpriteMasks/Mono/Sprite-Mask-Soft-Outside-Lit"))   { name = "Mask-Soft-Outside-Lit_Default", hideFlags = HideFlags.DontSave },
            new Material(Shader.Find("SpriteMasks/Mono/Sprite-Mask-Soft-Outside-Unlit")) { name = "Mask-Soft-Outside-Unlit_Default", hideFlags = HideFlags.DontSave }
        };

        public static readonly Material[] DefaultMultiMaskMaterials =
        {
            new Material(Shader.Find("SpriteMasks/Multi/Sprite-MultiMask-Hard-Inside-Lit"))    { name = "MultiMask-Hard-Inside-Lit_Default", hideFlags = HideFlags.DontSave },
            new Material(Shader.Find("SpriteMasks/Multi/Sprite-MultiMask-Hard-Inside-Unlit"))  { name = "MultiMask-Hard-Inside-Unlit_Default", hideFlags = HideFlags.DontSave },
            new Material(Shader.Find("SpriteMasks/Multi/Sprite-MultiMask-Hard-Outside-Lit"))   { name = "MultiMask-Hard-Outside-Lit_Default", hideFlags = HideFlags.DontSave },
            new Material(Shader.Find("SpriteMasks/Multi/Sprite-MultiMask-Hard-Outside-Unlit")) { name = "MultiMask-Hard-Outside-Unlit_Default", hideFlags = HideFlags.DontSave },

            new Material(Shader.Find("SpriteMasks/Multi/Sprite-MultiMask-Soft-Inside-Lit"))    { name = "MultiMask-Soft-Inside-Lit_Default", hideFlags = HideFlags.DontSave },
            new Material(Shader.Find("SpriteMasks/Multi/Sprite-MultiMask-Soft-Inside-Unlit"))  { name = "MultiMask-Soft-Inside-Unlit_Default", hideFlags = HideFlags.DontSave },
            new Material(Shader.Find("SpriteMasks/Multi/Sprite-MultiMask-Soft-Outside-Lit"))   { name = "MultiMask-Soft-Outside-Lit_Default", hideFlags = HideFlags.DontSave },
            new Material(Shader.Find("SpriteMasks/Multi/Sprite-MultiMask-Soft-Outside-Unlit")) { name = "MultiMask-Soft-Outside-Unlit_Default", hideFlags = HideFlags.DontSave },
        };

        public static readonly Material DefaultLit   = new Material(Shader.Find("Universal Render Pipeline/2D/Sprite-Lit-Default")) { name = "DefaultLit", hideFlags = HideFlags.DontSave };
        public static readonly Material DefaultUnlit = new Material(Shader.Find("Sprites/Default")) { name = "DefaultUnlit", hideFlags = HideFlags.DontSave };

        public static Material GetMultiMaskInstance(MaskType type)
        {
            var shader = MultiMaskShaders[(int)type];
            return new Material(shader) { name = shader.name, hideFlags = HideFlags.DontSave };
        }

        public static Material GetMaskInstance(MaskType type)
        {
            var shader = MaskShaders[(int) type];
            return new Material(shader) { name = shader.name, hideFlags = HideFlags.DontSave };
        }

        public static bool IsUnlit(this MaskType type)
        {
            return (int)type % 2 == 1;
        }
    }

    public enum MaskType
    {
        Hard_Inside_Lit,
        Hard_Inside_Unlit,
        Hard_Outside_Lit,
        Hard_Outside_Unlit,
        Soft_Inside_Lit,
        Soft_Inside_Unlit,
        Soft_Outside_Lit,
        Soft_Outside_Unlit,
    }
}