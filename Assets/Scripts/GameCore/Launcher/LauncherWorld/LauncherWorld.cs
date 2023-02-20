using System;
using System.Collections.Generic;
using Battle.Data;
using BeatRoyale.Windows;
using Core.SingleService;
using GameCore.Attributes;
using UnityEngine;

namespace BeatRoyale.Launcher
{
    public partial class LauncherWorld : ServiceManager
    {
        [ColoredField, SerializeField] private BackgroundAnimationData backgroundData;
        [SerializeField] private LevelConfig[] dumbledoreLevels;
        [SerializeField] private LevelConfig[] raccoonLevels;
        private Dictionary<string, (LevelConfig[], Dictionary<Type, float>)> levels = new Dictionary<string, (LevelConfig[], Dictionary<Type, float>)>();

        protected override void Awake()
        {
            base.Awake();
            Application.targetFrameRate = 60;
            ControlPanel.Show();
            levels.Add(GameScopes.Dumbledore, (dumbledoreLevels, new Dictionary<Type, float>()));
            levels.Add(GameScopes.Raccoon, (raccoonLevels, new Dictionary<Type, float>()));
        }

        private void Start()
        {
            LevelConfig.Properties.Clear();
            InitProperties(GameScopes.Dumbledore);
            InitProperties(GameScopes.Raccoon);
            ComputeProperties(GameScopes.Dumbledore);
            ComputeProperties(GameScopes.Raccoon);

            foreach (var level in levels)
            {
                foreach (var prop in level.Value.Item2)
                {
                    Debug.Log($"[Malvis] Scope: {level.Key}, Property Type: {prop.Key}, Value: {prop.Value}");
                }
            }
        }

        private void InitProperties(string entityScope)
        {
            for (int i = 0; i < UnlockedLevels.Config.Levels[entityScope]; i++)
            {
                var level = levels[entityScope].Item1[i];
                level.InitProperties();
            }
        }

        private void ComputeProperties(string entityScope)
        {
            var propByType = levels[entityScope].Item2;
            
            foreach (var property in LevelConfig.Properties)
            {
                if (!propByType.TryGetValue(property.Key, out var value))
                {
                    propByType.Add(property.Key, GetValue(value));
                }
                else
                {
                    propByType[property.Key] = GetValue(value);
                }

                float GetValue(float oldValue)
                {
                    for (int i = 0; i < property.Value.Count; i++)
                    {
                        var prop = property.Value[i];

                        if (entityScope.Contains(prop.scope))
                        {
                            oldValue += prop.Fixed;
                            oldValue *= prop.Multiply;
                            oldValue += oldValue / 100f * prop.Percent;
                        }
                    }

                    return oldValue;
                }
            }
        }

        private void Update()
        {
            backgroundData.Update();
        }
    }
}

