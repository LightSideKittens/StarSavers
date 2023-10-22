using System;
using Battle.Windows;
using BeatHeroes.Interfaces;
using GameCore.Battle.Data;
using LSCore;
using LSCore.AddressablesModule.AssetReferences;
using LSCore.Extensions;
using UnityEngine;

namespace Battle
{
    public class BattleWorld : ServiceManager
    {
        [SerializeField] private Effectors effectors;
        [SerializeField] private Camera camera;
        [SerializeField] private Locations locations;
        private Location location;

        private void Start()
        {
            BaseInitializer.Initialize(OnInitialize);
        }

        private void InstatiateLocation()
        {
            var locationIndex = IListExtensions.ClosestBinarySearch(
                index => locations[index].maxLevel,
                locations.Length,
                PlayerData.Config.Level);
            var locationData = locations[locationIndex];
            location = locationData.locationRef.Load();
            location.Generate();
        }
        
        private void OnInitialize()
        {
            Init();
            enabled = true;
        }

        private void Init()
        {
            effectors.Init();
            InstatiateLocation();
            PlayerWorld.Begin();
            OpponentWorld.Begin();
            MatchResultWindow.Showing += Unsubscribe;
            BattleWindow.Show();
        }

        private void Unsubscribe()
        {
            MatchResultWindow.Showing -= Unsubscribe;
        }

        private void OnApplicationQuit()
        {
            Unsubscribe();
        }
    }
}