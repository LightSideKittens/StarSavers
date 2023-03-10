using System;
using Newtonsoft.Json;

[Serializable]
public struct ValuePercent
{
    public decimal value;
    public int percent;

    [JsonIgnore] public float Value => (float)(value + value * (percent / 100m));
}
