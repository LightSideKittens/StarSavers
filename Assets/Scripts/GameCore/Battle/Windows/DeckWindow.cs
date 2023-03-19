using System;
using System.Collections.Generic;
using Battle.Data;
using GameCore.Battle.Data;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Battle.BattleBootstrap;

namespace Battle.Windows
{
    public partial class DeckWindow : BaseWindow<DeckWindow>, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [SerializeField] private Transform[] cardPlaces;
        [SerializeField] private Transform deckPanel;
        [SerializeField] private Slider manaSlider;
        [SerializeField] private TMP_Text manaCountText;
        [SerializeField] private TMP_Text[] manaPriceTexts;
        private readonly GameObject[] disabledCards = new GameObject[4];
        private Dictionary<GameObject, int> enabledCards = new();
        private Dictionary<GameObject, string> namesByCard = new();
        private GameObject selected;
        private Vector2 lastWorldPosition;
        private int currentIndex;
        private float manaFillSpeed = 1;
        private float mana;
        private bool manaEnabled = true;
        public static bool IsInited { get; private set; }
        private BaseEffector currentEffector;
        private Camera cam;
        private int IntMana => (int) mana;

        protected override void Init()
        {
            base.Init();
            IsInited = true;
            var cardsPrefabs = CardDecks.Config.Attack;
            cam = Camera.main;

            for (int i = 0; i < cardsPrefabs.Count; i++)
            {
                var entityName = cardsPrefabs[i];

                if (GameScopes.IsEffector(entityName))
                {
                    Effectors.ByName[entityName].Init();
                }

                var card = Instantiate(Cards.ByName[entityName], deckPanel);
                var cardGameObject = card.gameObject;
                namesByCard.Add(cardGameObject, entityName);

                if (i < 4)
                {
                    enabledCards.Add(cardGameObject, i);
                    cardGameObject.SetActive(true);
                    ToDeck(cardGameObject, cardPlaces[i]);
                    currentIndex++;
                }
                else
                {
                    cardGameObject.SetActive(false);
                    disabledCards[i - 4] = cardGameObject;
                }
            }
        }

        private void Update()
        {
            UpdateMana();
        }

        private void UpdateMana()
        {
            if (manaEnabled)
            {
                if (mana >= 10)
                {
                    mana = 10;
                }
                else
                {
                    UpdateManaView();
                }
            }
        }

        private void UpdateManaView()
        {
            mana += manaFillSpeed * Time.deltaTime;
            manaSlider.value = mana;
            manaCountText.text = $"{IntMana}";
        }

        private void ToDeck(GameObject card, Transform parent)
        {
            card.transform.SetParent(parent, false);
            card.transform.SetSiblingIndex(0);
            card.transform.localScale = Vector3.one;

            var cardName = namesByCard[card];
            var price = 0;

            if (GameScopes.IsEffector(cardName))
            {
                price = Effectors.ByName[cardName].Price;
            }
            else
            {
                price = Units.ByName[cardName].Price;
            }

            var priceTextIndex = enabledCards[card];
            var priceText = manaPriceTexts[priceTextIndex];
            priceText.text = $"{price}";

            var center = new Vector2(0.5f, 0.5f);
            var rectTransform = ((RectTransform) card.transform);
            rectTransform.anchorMin = center;
            rectTransform.anchorMax = center;
            rectTransform.anchoredPosition = Vector2.zero;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            currentEffector = null;
            selected = eventData.pointerEnter;
            selected.transform.SetParent(transform);
            lastWorldPosition = eventData.position;
            selected.transform.position = lastWorldPosition;

            var cardName = namesByCard[selected];

            if (GameScopes.IsEffector(cardName))
            {
                currentEffector = Effectors.ByName[cardName];
            }
            else
            {
                SpawnArea.gameObject.SetActive(true);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            var position = eventData.position;
            selected.transform.position = position;
            lastWorldPosition = cam.ScreenToWorldPoint(position);
            currentEffector?.DrawRadius(lastWorldPosition, new Color(1f, 1f, 1f, 0.3f));
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            var cardName = namesByCard[selected];

            if (GameScopes.IsEffector(cardName))
            {
                var effector = Effectors.ByName[cardName];
                var price = effector.Price;

                if (CanSpawn(ArenaBox.bounds, price))
                {
                    Spawn(price);
                    effector.Apply();
                }
                else
                {
                    Reset();
                }
            }
            else
            {
                var unit = Units.ByName[cardName];
                var price = unit.Price;
                SpawnArea.gameObject.SetActive(false);

                if (CanSpawn(SpawnArea.bounds, price))
                {
                    Spawn(price);
                    PlayerWorld.Spawn(unit, lastWorldPosition);
                }
                else
                {
                    Reset();
                }
            }

            currentEffector?.EndDrawRadius();

            void Spawn(int price)
            {
                mana -= price;
                selected.SetActive(false);
                var disabledCardIndex = currentIndex % disabledCards.Length;
                var newCard = disabledCards[disabledCardIndex];
                disabledCards[disabledCardIndex] = selected;
                var placeIndex = enabledCards[selected];
                var cardPlace = cardPlaces[placeIndex];
                enabledCards.Remove(selected);
                newCard.SetActive(true);
                enabledCards[newCard] = placeIndex;
                ToDeck(newCard, cardPlace);
            }

            bool CanSpawn(Bounds bounds, int price)
            {
                var size = bounds.size;
                size.z = 0;
                bounds.size = size;
                var min = bounds.min;
                var max = bounds.max;

                return IntMana >= price &&
                    lastWorldPosition.x > min.x && lastWorldPosition.y > min.y
                    && lastWorldPosition.x < max.x && lastWorldPosition.y < max.y;
            }

            void Reset()
            {
                var placeIndex = enabledCards[selected];
                var cardPlace = cardPlaces[placeIndex];
                ToDeck(selected, cardPlace);
            }

            currentIndex++;
        }
    }
}