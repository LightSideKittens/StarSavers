using System.IO;
using LSCore.Editor;
using UnityEditor.AssetImporters;
using UnityEngine;

[ScriptedImporter(1, "id")]
public class IdImporter : ScriptedImporter
{
    public override void OnImportAsset(AssetImportContext ctx)
    {
        var idAsset = ScriptableObject.CreateInstance<Id>();
        ctx.AddObjectToAsset("main", idAsset, LSIcons.Get("fire-icon"));
        ctx.SetMainObject(idAsset);
        // Create a nested text asset
        TextAsset textAsset = new TextAsset("This is a nested asset!");
        textAsset.name = Path.GetFileNameWithoutExtension(ctx.assetPath);
        // Add the nested asset with a specified name
        ctx.AddObjectToAsset("MyNestedTextAssetName", textAsset);
    }
}