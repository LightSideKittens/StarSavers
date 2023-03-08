using System.Collections.Generic;
using GameCore.Battle.Data;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Battle.Windows
{
    public class DeckWindow : BaseWindow<DeckWindow>, IPointerDownHandler, IPointerUpHandler, IDragHandler
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
        private int currentIndex;
        private float manaFillSpeed = 1;
        private float mana;
        private int IntMana => (int)mana;

        protected override void Init()
        {
            base.Init();
            var cardsPrefabs = CardDecks.Config.Attack;
            
            for (int i = 0; i < cardsPrefabs.Count; i++)
            {
                var entityName = cardsPrefabs[i];
                var card = Instantiate(Cards.ByEntitiesNames[entityName], deckPanel);
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
                    disabledCards[i-4] = cardGameObject;
                }
            }
        }

        private void Update()
        {
            if (mana >= 10)
            {
                mana = 10;
            }
            else
            {
                mana += manaFillSpeed * Time.deltaTime;
                manaSlider.value = mana;
                manaCountText.text = $"{IntMana}";
            }
        }

        private void ToDeck(GameObject card, Transform parent)
        {
            card.transform.SetParent(parent, false);
            card.transform.SetSiblingIndex(0);
            
            var price = Units.ByEntitiesNames.TryGetValue(namesByCard[card], out var unit) ? unit.Price : 0;
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
            selected = eventData.pointerEnter;
            selected.transform.SetParent(transform);
            selected.transform.position = eventData.position;
            BattleBootstrap.SpawnArea.gameObject.SetActive(true);
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            selected.transform.position = eventData.position;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            var unit = Units.ByEntitiesNames[namesByCard[selected]];
            BattleBootstrap.SpawnArea.gameObject.SetActive(false);
            
            if (IntMana >= unit.Price)
            {
                var bounds = BattleBootstrap.SpawnArea.bounds;
                var size = bounds.size;
                size.z = 0;
                bounds.size = size;
                Vector2 position = Camera.main.ScreenToWorldPoint(eventData.position);
                var min = bounds.min;
                var max = bounds.max;

                if (position.x > min.x && position.y > min.y 
                    && position.x < max.x && position.y < max.y)
                {
                    mana -= unit.Price; 
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
                    PlayerWorld.Spawn(unit, position);
                }
                else
                {
                    Reset();
                }
            }
            else
            {
                Reset();
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