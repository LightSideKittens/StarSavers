using System;
using UnityEngine;

namespace LGCore.AddressablesModule.AssetReferences
{
    [Serializable]
    public class SpriteRef : AssetRef<Sprite>
    {
        public SpriteRef(string guid) : base(guid) { }
    }
}