using UnityEditor;
using UnityEditor.U2D;
using UnityEngine.U2D;

namespace StarSavers
{
    public class SpriteAtlasGenerator
    {
        [MenuItem("Assets" + "/" + nameof(SpriteAtlasGenerator) + "/" + nameof(Generate))]
        private static void Generate()
        {
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            var folder = AssetDatabase.GetSubFolders(path);

            for (int i = 0; i < folder.Length; i++)
            {
                var spriteAtlas = new SpriteAtlas();
                var ends = folder[i].Split('/');
                var spriteAtlasPath = $"{folder[i]}/{Selection.activeObject.name}_{ends[^1]}.spriteatlas";
                AssetDatabase.CreateAsset(spriteAtlas, spriteAtlasPath);
                var folderObject = AssetDatabase.LoadMainAssetAtPath(folder[i]);
                spriteAtlas.Add(new []{folderObject});
            }
        }
        
        [MenuItem("Assets" + "/" + nameof(SpriteAtlasGenerator) + "/" + nameof(Generate), true)]
        private static bool ValidateGenerate()
        {
            return Selection.objects.Length == 1 && Selection.activeObject.name.StartsWith("Anim_");
        }
    }
}
