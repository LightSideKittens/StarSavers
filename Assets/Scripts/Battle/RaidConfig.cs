using System;
using System.Collections.Generic;
using LSCore;
using LSCore.AnimationsModule;
using LSCore.AnimationsModule.Animations.Text;
using LSCore.Attributes;
using LSCore.BattleModule;
using LSCore.LevelSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Battle.Data
{
    [Serializable]
    public class RaidConfigRef : AssetRef<RaidConfig> { }
    
    public class RaidConfig : ScriptableObject
    {
        [Serializable]
        [HideReferenceObjectPicker]
        [TypeFrom]
        public abstract class WaveAction
        {
            public abstract void OnStart();
            public virtual void OnComplete(){}
        }
        
        [Serializable]
        public class AnimCamera : WaveAction
        {
            public AnimSequencer animation;
            public override void OnStart()
            {
                var from = animation.GetAnim<CameraSizeAnim>();
                from.target = BattleWorld.Camera;
                animation.Animate();
            }
        }
        
        [Serializable]
        public class Wave
        {
            [SerializeReference] public List<WaveAction> actions;
        }
        
        [Serializable]
        public class BossData
        {
            [MinMaxSlider(0, "Max")] public Vector2Int fromTo;
            [HideInInspector] public Id[] stageIds;
            
#if UNITY_EDITOR
            private int Max
            {
                get
                {
                    stageIds = currentInspected.bossStages;
                    return stageIds.Length;
                }
            }
#endif
        }
        
        [Serializable]
        public class SingleSpawner : Spawner
        {
            [ValueDropdown("Ids")] public Id id;
        
            public override void Spawn()
            {
                OpponentWorld.Spawn(id);
            }
            
#if UNITY_EDITOR
            private IEnumerable<Id> Ids => currentInspected.levelsManager.Ids;
#endif
        }
        
        [Serializable]
        public class GroupSpawner : Spawner
        {
            public GroupUnitSpawner spawner;
        
            public override void Spawn() => spawner.Spawn();
        }

        [SerializeField] private LevelsManager levelsManager;
        [SerializeField] private Location location;
        [SerializeField] [ValueDropdown("Ids")] private Id[] bossStages;
        [SerializeField] private Wave[] waves;
        [SerializeField] private bool isInfinity;

#if UNITY_EDITOR
        private IEnumerable<Id> Ids => currentInspected.levelsManager.Ids;
#endif
        
        public bool MoveNextWave()
        {
            currentWave++;
            if (currentWave >= waves.Length)
            {
                currentWave = waves.Length - 1;
                return isInfinity;
            }

            return true;
        }
        
        private int currentWave;
        
        public void Setup(int enemiesLevel)
        {
            levelsManager.Init();
            OpponentWorld.Enemies = levelsManager;
            levelsManager.SetLevelForAll(enemiesLevel);
            InstantiateLocation();
            currentWave = 0;
        }

        public Wave GetWave()
        {
            return waves[currentWave];
        }
        
        private void InstantiateLocation()
        {
            location.Generate();
        }

        public void Dispose() => location.Dispose();
        
#if UNITY_EDITOR
        private static RaidConfig currentInspected;
        [OnInspectorInit] private void Editor_Init() => currentInspected = this;
#endif
    }
}