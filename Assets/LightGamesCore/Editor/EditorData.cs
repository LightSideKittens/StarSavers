#if UNITY_EDITOR
using System.Collections.Generic;
using Core.ConfigModule;

namespace Fishcoin
{
    public class EditorData : JsonBaseConfigData<EditorData>
    {
        protected override string FolderName => FolderNames.EditorConfigs;
        public Dictionary<string, string> textureGuidByAssetGuid { get; } = new();
    }
}
#endif