using System;
using DG.Tweening;
using GameCore.Attributes;
using LSCore.Extensions.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace BeatRoyale.Windows
{
    public class CardGalleryWindow : BaseLauncherWindow<CardGalleryWindow>
    {
        [Serializable]
        private class Tab
        {
            [SerializeField] private Toggle toggle;
            [SerializeField] private GameObject tab;

            public void Init()
            {
                toggle.AddListener(OnToggle);
                OnToggle(toggle.isOn);
            }

            private void OnToggle(bool active)
            {
                toggle.transform.SetSiblingIndex(active ? 1 : 0);
                tab.SetActive(active);
            }
        }
        
        [Serializable]
        private class CardDeck
        {
            [SerializeField] private Toggle switcher;
            [SerializeField] private Image mark;
            [SerializeField] private GameObject attackDeck;
            [SerializeField] private GameObject defenceDeck;

            public void Init()
            {
                switcher.AddListener(OnToggle);
                OnToggle(switcher.isOn);
            }

            private void OnToggle(bool active)
            {
                attackDeck.SetActive(active);
                defenceDeck.SetActive(!active);
                switcher.interactable = false;
                mark.rectTransform.DOAnchorPos(new Vector2(0, active ? 27 : -27), 0.2f).OnComplete(() => switcher.interactable = true);
            }
        }
        
        protected override int Internal_Index => 1;
        [ColoredField, SerializeField] private Tab cardsTab;
        [ColoredField, SerializeField] private Tab heroesTab;
        [ColoredField, SerializeField] private CardDeck deck;

        protected override void Init()
        {
            base.Init();
            cardsTab.Init();
            heroesTab.Init();
            deck.Init();
            transform.SetSiblingIndex(0);
        }
    }
}