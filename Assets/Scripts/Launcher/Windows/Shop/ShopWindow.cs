using System;
using System.Collections.Generic;
using LSCore;
using UnityEngine;
using UnityEngine.UI;

namespace BeatHeroes.Windows
{
    public class ShopWindow : BaseWindow<ShopWindow>
    {
        [Serializable]
        public struct ShopCategory
        {
            public LSText button;
            public GameObject content;
        }
        
        [SerializeField] private List<ShopCategory> categories;
        [SerializeField] private Image selectedPointer;
        
        protected override void OnShowing()
        {
            MainWindow.Hide();
        }

        protected override void OnBackButton()
        {
            base.OnBackButton();
            MainWindow.Show();
        }
    }
}