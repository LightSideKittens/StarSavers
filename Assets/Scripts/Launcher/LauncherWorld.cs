using System.Diagnostics;
using Animatable;
using Battle.Data;
using StarSavers.Interfaces;
using StarSavers.Windows;
using LSCore;
using LSCore.Attributes;
using Sirenix.OdinInspector;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

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

        public Image spriteRenderer;
        public float x;
        
        public MoveItCurve moveItCurve;
        public AnimationCurve animationCurve;
        
        [Button]
        public void Test()
        {
            var go = gameObject;
            var tr = transform;
            var type = typeof(LauncherWorld);
            
            var sw = new Stopwatch();
            
            sw.Start();
            for (int i = 0; i < 100_000; i++)
            {
                transform.localPosition = new Vector3(0, 0, 0);
            }
            sw.Stop();
            Debug.Log(sw.ElapsedMilliseconds);
            sw.Reset();

            var clip = CreateClip();

            
            sw.Start();
            for (int i = 0; i < 100_000; i++)
            {
                
            }
            
            sw.Stop();
            Debug.Log(sw.ElapsedMilliseconds);

        }
        
        private static AnimationClip CreateClip()
        {
            var clip = new AnimationClip { legacy = true };
            clip.hideFlags = HideFlags.HideAndDontSave;
            return clip;
        }

        private void Init()
        {
            MainWindow.AsHome();
            MainWindow.Show();
            PlayerData.Config.SelectedHero.SubOnChangedAndCall(Create);
            AnimatableCanvas.SortingOrder = 30000;
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

