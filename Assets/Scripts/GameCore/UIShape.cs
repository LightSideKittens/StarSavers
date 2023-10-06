using UnityEngine;
using UnityEngine.UI;

namespace LGCore.UIModule
{
    [RequireComponent(typeof(CanvasRenderer), typeof(RectTransform), typeof(MeshFilter)),  DisallowMultipleComponent]
    public class UIShape : MaskableGraphic
    {
        public MeshFilter Shape { get; set; }
        private Mesh mesh;

        protected override void OnEnable()
        {
            base.OnEnable();
            Shape = GetComponent<MeshFilter>();
            mesh = new Mesh();
        }

        public override void Rebuild(CanvasUpdate update)
        {
            base.Rebuild(update);
            if (update == CanvasUpdate.PreRender)
            {
                mesh = Shape.sharedMesh;
                canvasRenderer.SetMesh(mesh);
                var texture = material.mainTexture;
                canvasRenderer.SetColor(color);

                if (texture == null)
                {
                    texture = Texture2D.whiteTexture;
                }
                
                canvasRenderer.SetTexture(texture);
            }
        }
    }
}