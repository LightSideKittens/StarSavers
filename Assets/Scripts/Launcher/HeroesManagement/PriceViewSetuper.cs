using System;
using LSCore;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Launcher.HeroesManagement
{
    [Serializable]
    public abstract class PriceViewSetuper
    {
        public abstract void Setup(Funds funds);
    }

    [Serializable]
    public class OnOffById : PriceViewSetuper
    {
        public FundText[] texts;
        
        public override void Setup(Funds funds)
        {
            foreach (var text in texts)
            {
                text.gameObject.SetActive(false);
            }
            
            foreach (var text in texts)
            {
                var fund = funds.GetById(text.Id);
                text.gameObject.SetActive(true);
                text.text = fund.Value.ToString();
            }
        }
    }
    
    [Serializable]
    public class CreateById : PriceViewSetuper
    {
        public FundText[] textsPrefabs;
        public Transform parent;
        
        public override void Setup(Funds funds)
        {
            foreach (var prefab in textsPrefabs)
            {
                var fund = funds.GetById(prefab.Id);
                var text = Object.Instantiate(prefab, parent);
                text.text = fund.Value.ToString();
            }
        }
    }
}