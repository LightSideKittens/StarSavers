using System;
using LSCore.AddressablesModule.AssetReferences;

namespace GameCore.Battle.Data
{
    [Serializable]
    public class LocationRef : AssetRef<LocationData>
    {
        protected LocationRef(string guid) : base(guid) { }
    }
}