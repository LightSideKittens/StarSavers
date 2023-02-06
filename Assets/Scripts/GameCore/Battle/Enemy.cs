using System;
using Battle.ConfigsSO;
using Core.ReferenceFrom.Extensions.Unity;
using DG.Tweening;
using DG.Tweening.Plugins.Core.PathCore;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Battle
{
    public class Enemy
    {
        private GameObject gameObject;
        private TMP_Text slider;
        private float damage;
        private float health;
        private float willHealth;
        private float maxHealth;
        private static int sortingOrder;
        public event Action NotDestroyed;
        public event Action Destroyed;
        public event Action WillDestroyed;
        public virtual Vector3 EndPosition => Position - new Vector3(0, 3, 0);
        public Vector3 Position => gameObject.transform.position;
        public Transform Transform => gameObject.transform;
        public string SoundType { get; private set; }

        public void Init(string soundType)
        {
            SoundType = soundType;
            var enemy = GetEnemy();
            damage = enemy.damage;
            health = enemy.health;
            willHealth = health;
            maxHealth = health;
            gameObject = Object.Instantiate(enemy.prefab, MusicReactiveTest.PathPoints[0], Quaternion.identity);
            gameObject.GetComponent<SortingGroup>().sortingOrder += 1000 - sortingOrder % 100;
            slider = gameObject.Get(enemy.text);
            gameObject.name = "Enemy " + Random.Range(0, 999);
            sortingOrder++;
        }

        protected virtual EnemiesData.Data GetEnemy()
        {
            return EnemiesData.GetEnemy(SoundType);
        }

        public void StartMove(float duration)
        {
            gameObject.transform.DOPath(new Path(PathType.Linear, MusicReactiveTest.PathPoints, 1), duration)
                .OnComplete(OnMoveCompleted)
                .SetEase(Ease.Linear);
        }

        protected virtual void OnMoveCompleted()
        {
            ComputeDamage(health);
            TakeDamage(health);
        }

        public void ComputeDamage(float damage)
        {
            willHealth -= damage;
            if (willHealth <= 0)
            {
                WillDestroyed?.Invoke();
            }
        }

        public void TakeDamage(float damage)
        {
            health -= damage;
            UpdateHealthText();

            if (health <= 0)
            {
                Destroyed?.Invoke();
                Object.Destroy(gameObject);
            }
            else
            {
                NotDestroyed?.Invoke();
            }
        }

        private void UpdateHealthText()
        {
            slider.text = $"{(int)health}";
        }
    }
}