using UnityEngine;

namespace FastSpriteMask
{
    public class MaskedSpriteShape : MaskedSprite
    {
        protected override void ApplyMaterial(Material material)
        {
            Renderer.sharedMaterials = new []{ material, material };
        }
    }
}