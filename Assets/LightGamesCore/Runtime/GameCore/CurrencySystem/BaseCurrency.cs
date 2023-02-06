using System;
using Core.ConfigModule;
using Newtonsoft.Json;

public abstract class BaseCurrency<T> : JsonBaseConfigData<T> where T : BaseCurrency<T>, new()
{
    protected override string FolderName => "Currencies";
    public static int defaultValue;
    [JsonProperty] private int value;
    public static event Action Changing;
    public static event Action Changed;

    public static int Value
    {
        get => Config.value;
        private set
        {
            if (Config.value != value)
            {
                Changing?.Invoke();
                Config.value = value;
                Changed?.Invoke();
            }
        }
    }

    protected override void SetDefault()
    {
        base.SetDefault();
        FileName = GetType().Name.ToLower();
        value = defaultValue;
    }

    public static void Earn(int value)
    {
        Value += value;
    }

    public static bool TrySpend(int value)
    {
        var canSpend = value <= Value;

        if (canSpend)
        {
            Value -= value;
        }

        return canSpend;
    }

    public static bool TryConvertTo<T1>(int fromUnitCount, int toUnitCount) where T1 : BaseCurrency<T1>, new()
    {
        var isSpent = TrySpend(fromUnitCount);

        if (isSpent)
        {
            BaseCurrency<T1>.Earn(toUnitCount);
        }

        return isSpent;
    }
}
