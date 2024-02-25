using System;
using System.Collections.Generic;
using System.Linq;
using LSCore;
using LSCore.LevelSystem;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Battle.Data
{
    public class RaidConfig : ScriptableObject
    {
        [Serializable]
        private struct EnemyData
        {
            [ValueDropdown("Ids")] public Id id;
            [BoxGroup] public InBattleFund reward;
            public int level;
            [CustomValueDrawer("Editor_Draw")] public AnimationCurve spawnPossibility;
            
#if UNITY_EDITOR
            private AnimationCurve Editor_Draw(AnimationCurve value, GUIContent content) => DrawCurve(value, content);
            private IEnumerable<Id> Ids => currentInspected.levelsManager.Group;
#endif
        }
        
        [field: SerializeField] public int Time { get; private set; } = 300;
        [SerializeField] private LocationRef locationRef;
        [SerializeField] private LevelsManager levelsManager;
        [SerializeField] private EnemyData[] enemyData;
        
        [CustomValueDrawer("Editor_Draw")] 
        [SerializeField] private AnimationCurve spawnFrequency;
        
        [field: SerializeField] public int BreakDuration { get; private set; } = 15;
        [SerializeField] private int[] waveDurations;

        public IEnumerable<Id> EnemyIds => enemyData.Select(x => x.id); 
        
        public int CurrentWave
        {
            get => currentWave;
            set
            {
                if (value >= waveDurations.Length)
                {
                    currentWave = waveDurations.Length - 1;
                    return;
                }
                
                currentWave = value;
            }
        }
        
        private int currentWave;
        private readonly List<float> possibilities = new();
        private readonly Dictionary<Id, EnemyData> enemyById = new();
        
        public void Setup()
        {
            InstantiateLocation();
            possibilities.Clear();
            enemyById.Clear();
            currentWave = 0;
            
            for (int i = 0; i < enemyData.Length; i++)
            {
                var data = enemyData[i];
                possibilities.Add(default);
                enemyById.Add(data.id, data);
                levelsManager.SetLevel(data.id, data.level);
                Funds.Clear(data.reward.Id);
            }
        }

        public float GetSpawnFrequency(int time)
        {
            var factor = (float)time / Time;
            return spawnFrequency.Evaluate(factor);
        }

        public int GetWaveDuration()
        {
            return waveDurations[currentWave];
        }

        public void OnEnemyKilled(Id id)
        {
            if (enemyById.TryGetValue(id, out var data))
            {
                data.reward.Earn();
            }
        }
        
        public Id GetEnemyId(int time)
        {
            var factor = (float)time / Time;
            var total = 0f;

            for (int i = 0; i < enemyData.Length; i++)
            {
                var value =  enemyData[i].spawnPossibility.Evaluate(factor);
                possibilities[i] = value;
                total += value;
            }
            
            for (int i = 0; i < enemyData.Length; i++)
            {
                possibilities[i] /= total;
            }
            
            var indexedList = possibilities.Select((value, index) => (value, index)).ToList();
            indexedList.Sort((pair1, pair2) => pair1.value.CompareTo(pair2.value));

            factor = Random.value;
            total = 0f;
            
            for (int i = 0; i < indexedList.Count; i++)
            {
                var pair = indexedList[i];
                total += pair.value;
                
                if (factor < total)
                {
                    return enemyData[pair.index].id;
                }
            }
            
            return enemyData[indexedList[^1].index].id;
        }

        private void InstantiateLocation()
        {
            var location = locationRef.Load();
            location.Generate();
        }
        
#if UNITY_EDITOR
        private AnimationCurve Editor_Draw(AnimationCurve value, GUIContent content) => DrawCurve(value, content);
        private static RaidConfig currentInspected;
        [OnInspectorInit] private void Editor_Init() => currentInspected = this;

        private static AnimationCurve DrawCurve(AnimationCurve value, GUIContent content)
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            EditorGUI.LabelField(EditorGUILayout.GetControlRect(GUILayout.MaxWidth(120)), content);
            var rect = EditorGUILayout.GetControlRect(GUILayout.Height(50));
            EditorGUI.BeginChangeCheck();
            value = EditorGUI.CurveField(rect, value);
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(currentInspected);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(10);
            return value;
        }
#endif
    }
}