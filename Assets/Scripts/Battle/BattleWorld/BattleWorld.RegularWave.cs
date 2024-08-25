using System;
using Battle;
using Battle.Data;
using Battle.Windows;
using DG.Tweening;
using LSCore.Async;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LSCore.BattleModule
{
    public partial class BattleWorld
    {
        [Serializable]
        public class RegularWave : RaidConfig.WaveAction
        {
            [Serializable]
            public struct Data
            {
                [SerializeReference] public Spawner spawner;
                [MinValue(1)] public int spawnCount;
                [MinValue(5)] public int timeRef;
                public AnimationCurve spawnFrequency;
                private int spawnedCount;
                private Action onComplete;
                private Func<float> timeGetter;

                public void StartSpawn(Func<float> time, Action onComplete)
                {
                    timeGetter = time;
                    this.onComplete = onComplete;
                    Spawn();
                }

                private void Spawn()
                {
                    var delay = spawnFrequency.Evaluate(timeGetter() / timeRef);

                    if (delay > 0.1f)
                    {
                        if (spawnedCount > 0)
                        {
                            spawner.Spawn();
                        }

                        if (spawnedCount >= spawnCount)
                        {
                            Debug.Log("Spawn Completed");
                            onComplete();
                            return;
                        }
                        
                        spawnedCount++;
                        Wait.Delay(delay, Spawn);
                    }
                    else
                    {
                        Wait.Frames(1, Spawn);
                    }
                }
            }

            public Data[] data;
            public bool waitAllUnitsAreKilled;
            private float currentTime;
            
            public override void OnStart()
            {
                currentWave++;
                BattleWindow.SplashText($"WAVE {currentWave}");
                Wait.TimerForward(int.MaxValue, x => currentTime = x).SetId(this);
                
                var allActions = Wait.AllActions();
                
                for (int i = 0; i < data.Length; i++)
                {
                    data[i].StartSpawn(GetTime, allActions.WaitAction());
                }
                
                allActions.OnComplete(() =>
                {
                    if (waitAllUnitsAreKilled && OpponentWorld.UnitCount > 0)
                    {
                        OpponentWorld.AllUnitsReleased += WaitForAllUnitsAreKilled;
                    }
                    else
                    {
                        OnWaveCompleted();
                    }
                });

                void WaitForAllUnitsAreKilled()
                {
                    OpponentWorld.AllUnitsReleased -= WaitForAllUnitsAreKilled;
                    OnWaveCompleted();
                }
            }

            private float GetTime() => currentTime;

            public override void OnComplete()
            {
                base.OnComplete();
                DOTween.Kill(this);
            }
        }
    }
}