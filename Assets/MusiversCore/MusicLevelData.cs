using System;
using LSCore.AddressablesModule.AssetReferences;
using MusicEventSystem.Configs;
using Sirenix.OdinInspector;

[Serializable]
public class MusicLevelData
{
    public AudioClipRef audioClip;
    public string configName;
    public float startTime;
    [MinValue(1f)]
    public float endTime = MusicData.DefaultEndTime;
}