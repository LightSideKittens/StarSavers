using System;
using System.Collections.Generic;
using Battle.Data;
using UnityEditor;

[CustomEditor(typeof(LevelConfig))]
public class LevelConfigEditor : BaseWalletSerializationHelper<LevelConfig>
{
    protected override void OnInit()
    {
        if (config.Price == null)
        {
            config.Price = new List<BaseWallet>();
        }
    }

    protected override void OnButtonClicked(Type type)
    {
        config.Price.Add((BaseWallet)Activator.CreateInstance(type));
    }
}