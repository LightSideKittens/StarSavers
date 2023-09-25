using System;
using UnityEngine;

namespace LGCore.AddressablesModule.AssetReferences
{
    [Serializable]
    public class AudioClipRef : AssetRef<AudioClip>
    {
        public AudioClipRef(string guid) : base(guid) { }
    }
}