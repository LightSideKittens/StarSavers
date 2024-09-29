using Battle.Data;
using StarSavers.Interfaces;
using StarSavers.Windows;
using LSCore;
using LSCore.Attributes;
using UnityEngine;

namespace StarSavers.Launcher
{
    public class LauncherWorld : ServiceManager<LauncherWorld>
    {
        [DateTime]
        [SerializeField] public long dateTime4;
        
        [TimeSpan(1, 1, 1)]
        [SerializeField] public long timeSpan4 = 1;
        
        [CronEx] public string cron;
        
        [SelectEx] public string selectEx;
        [SelectEx("count")] public string selectEx2;

        public int count;
        
        [GenerateGuid] public string id;

        public HeroesRenderersById heroesRenderersById;
        private GameObject heroRenderer;

        protected override void Awake()
        {
            base.Awake();
            BaseInitializer.Initialize(Init);
        }
        
        private void Init()
        {
            MainWindow.AsHome();
            MainWindow.Show();
            PlayerData.Config.SelectedHero.SubOnChangedAndCall(Create);
        }
        
        private void Create(string heroId)
        {
            if (heroRenderer != null)
            {
                Destroy(heroRenderer);
            }
            
            heroRenderer = heroesRenderersById.ByKey[heroId];
            heroRenderer = Instantiate(heroRenderer);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            PlayerData.Config.SelectedHero.Changed -= Create;
        }

        /*[Button] private void Create(string clanName) => Clan.Create(clanName).OnComplete(task => Burger.Log($"[Clan] Create {clanName} {task.IsSuccess} {task.Exception}"));
        [Button] private void Join(string clanId) => Clan.Join(clanId).OnComplete(task => Burger.Log($"[Clan] Joined {clanId} {task.IsSuccess} {task.Exception}"));
        [Button] private void Delete() => Clan.Delete().OnComplete(task => Burger.Log($"[Clan] Delete {Clan.Id} {task.IsSuccess} {task.Exception}"));
        [Button] private void Leave() => Clan.Leave().OnComplete(task => Burger.Log($"[Clan] Leave {Clan.Id} {task.IsSuccess} {task.Exception}"));
        [Button] private void Kick(string userId) => Clan.Kick(userId).OnComplete(task => Burger.Log($"[Clan] Kick {Clan.Id} User: {userId} {task.IsSuccess} {task.Exception}"));*/
        
    }
}

