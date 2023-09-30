using System;
using System.Collections;
using System.Collections.Generic;
using Battle.Data;
using GameCore.Battle.Data;
using LSCore.Extensions.Unity;
using Sirenix.OdinInspector;
using UnityEngine;
using static Battle.BattleWorld;

namespace Battle
{
    public class OpponentWorld : BasePlayerWorld<OpponentWorld>
    {
        private static IEnumerable<string> EnemyNames => GameScopes.EntitiesNames;
        [SerializeField, ValueDropdown(nameof(EnemyNames))] private string enemyName;
        [SerializeField] private Transform spawnPoint;
        
        private CardDecks decks;

        protected override void Awake()
        {
            base.Awake();
            UserId = "Opponent";
        }

        private void OnEnable()
        {
            StartCoroutine(SpawnOpponents());
        }

        private IEnumerator SpawnOpponents()
        {
            while (enabled)
            {
                Spawn();
                yield return new WaitForSeconds(1);
            }
        }

        private void Spawn()
        {
            Internal_Spawn(Units.ByName[enemyName], spawnPoint.position);
        }

        protected override void OnStop()
        {
            
        }
    }
}