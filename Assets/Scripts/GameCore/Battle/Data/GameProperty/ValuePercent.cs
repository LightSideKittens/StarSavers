using System;
using Newtonsoft.Json;

[Serializable]
public struct ValuePercent
{
    public float value;
    public int percent;

    [JsonIgnore] public float Value => value + value * (percent / 100f);
}
