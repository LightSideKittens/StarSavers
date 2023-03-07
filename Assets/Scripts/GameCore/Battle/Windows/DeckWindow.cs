using System.Collections.Generic;
using GameCore.Battle.Data;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Battle.Windows
{
    public class DeckWindow : BaseWindow<DeckWindow>, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [SerializeField] private Transform[] cardPlaces;
        [SerializeField] private Transform deckPanel;
        private readonly GameObject[] disabledCards = new GameObject[4];
        private Dictionary<GameObject, int> enabledCards = new();
        private Dictionary<GameObject, string> namesByCard = new();
        private GameObject selected;
        private int currentIndex;

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
                    SetParent(cardGameObject, cardPlaces[i]);
                    currentIndex++;
                }
                else
                {
                    cardGameObject.SetActive(false);
                    disabledCards[i-4] = cardGameObject;
                }
            }
        }

        private void SetParent(GameObject card, Transform parent)
        {
            card.transform.SetParent(parent, false);
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
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            selected.transform.position = eventData.position;
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            selected.SetActive(false);
            var disabledCardIndex = currentIndex % disabledCards.Length;
            var newCard = disabledCards[disabledCardIndex];
            disabledCards[disabledCardIndex] = selected;
            var placeIndex = enabledCards[selected];
            var cardPlace = cardPlaces[placeIndex];
            enabledCards.Remove(selected);
            newCard.SetActive(true);
            enabledCards[newCard] = placeIndex;
            SetParent(newCard, cardPlace);
            var position = Camera.main.ScreenToWorldPoint(eventData.position);
            PlayerWorld.Spawn(Units.ByEntitiesNames[namesByCard[selected]], position);
            currentIndex++;
        }
    }
}