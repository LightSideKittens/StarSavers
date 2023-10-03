using System.Collections.Generic;
using GameCore.Battle.Data;
using UnityEngine;
using UnityEngine.UI;

namespace BeatRoyale.Windows
{
    public class HeroView : MonoBehaviour
    {
        private Toggle toggle;

        private string id;
        
        private void Awake()
        {
            toggle = GetComponent<Toggle>();
            
        }

        public void Init(string name, Image image)
        {
            id = name;
            toggle.targetGraphic.GetComponent<Image>().sprite = image.sprite;
            toggle.onValueChanged.AddListener(OnSelectedHeroChanged);
        }

        public void SetSelected(bool value)
        {
            toggle.isOn = value;
        }
        
        private void OnSelectedHeroChanged(bool value)
        {
            if (value)
            {
                PlayerData.Config.SelectedHero = id;
            }
        }
    }
}