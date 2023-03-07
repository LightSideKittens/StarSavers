using System;

[Serializable]
public struct ValuePercent
{
    public decimal value;
    public int percent;

    public float Value => (float)(value + value * (percent / 100m));
}
