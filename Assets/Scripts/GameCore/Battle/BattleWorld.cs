using System;
using Battle.Data;
using Battle.Data.GameProperty;
using Battle.Windows;
using GameCore.Battle;
using GameCore.Battle.Data;
using LGCore;
using UnityEngine;
using Initializer = BeatRoyale.Interfaces.BaseInitializer<BeatRoyale.Interfaces.IInitializer>;

namespace Battle
{
    public partial class BattleWorld : ServiceManager
    {
        [SerializeField] private Units units;
        [SerializeField] private Cards cards;
        [SerializeField] private Effectors effectors;
        [SerializeField] private Unit hero;
        [SerializeField] private Camera camera;
        public static MeshRenderer SpawnArea { get; private set; }
        public static MeshRenderer OpponentSpawnArena { get; private set; }
        public static BoxCollider2D ArenaBox { get; private set; }

        private Rigidbody2D rbHero;
        private float heroSpeed;
        
        protected override void Awake()
        {
            base.Awake();
            hero = Instantiate(hero);
            hero.Destroyed += OnHeroDied;
            rbHero = hero.GetComponent<Rigidbody2D>();
            heroSpeed = EntiProps.ByName[hero.EntityName][nameof(MoveSpeedGP)].Value;
            hero.Init("Player");
            CameraMover.Init(camera, hero.transform);
        }

        private void OnHeroDied()
        {
            enabled = false;
        }

        private void Start()
        {
            Initializer.Initialize(OnInitialize);
        }

        private void OnInitialize()
        {
            Init();
        }

        private void Init()
        {
            units.Init();
            cards.Init();
            effectors.Init();
            MatchResultWindow.Showing += Unsubscribe;
            BattleWindow.Show();
        }

        private void Update()
        {
            hero.Run();
        }

        private void FixedUpdate()
        {
            var direction = BattleWindow.Joystick.Direction.normalized;
            if (direction.Equals(Vector2.zero))
            {
                return;
            }
            
            rbHero.position += direction * (heroSpeed * Time.fixedDeltaTime);
            var zAngle = Vector2.SignedAngle(Vector2.up, direction);
            rbHero.rotation = zAngle;
            hero.FixedRun();
            CameraMover.MoveCamera();
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