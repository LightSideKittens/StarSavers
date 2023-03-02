using System;
using System.Collections.Generic;
using Battle.Data;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelConfig))]
public class LevelConfigEditor : BaseWalletSerializationHelper<LevelConfig>
{
    protected override void OnInit()
    {
        if (config.Prices == null)
        {
            config.Prices = new List<BaseWallet>();
        }
        else
        {
            for (int i = 0; i < config.Prices.Count; i++)
            {
                var price = config.Prices[i];
                config.AddedPrices.TryAdd(price.GetType(), price);
            }
        }
    }

    protected override void OnButtonClicked(Type type)
    {
        if (!config.AddedPrices.TryGetValue(type, out var price))
        {
            price = (BaseWallet) Activator.CreateInstance(type);
            config.Prices.Add(price);
            config.AddedPrices.TryAdd(price.GetType(), price);
        }
        else
        {
            config.Prices.Remove(price);
            config.AddedPrices.Remove(price.GetType());
        }
        
        config.OnValidate();
        EditorUtility.SetDirty(config);
    }

    protected override void OnDrawButton(Type type)
    {
        if (config.AddedPrices.TryGetValue(type, out var price))
        {
            var newValue = EditorGUILayout.IntField(price.value, GUILayout.MaxWidth(100));

            if (newValue != price.value)
            {
                price.value = newValue;
                config.OnValidate();
                EditorUtility.SetDirty(config);
            }
        }
    }
}