using System.Collections.Generic;

namespace Battle.Data
{
    public static class GameScopes
    {
        #region Global

        public const string Global = nameof(Global);
        
        #region Heroes
        public const string Heroes = Global + "/" + nameof(Heroes);
        
        #region Warriors
        public const string Warrior = Heroes + "/" + nameof(Warrior);
        public const string Ogre = Warrior + "/" + nameof(Ogre);
        #endregion
        
        #region Mages
        public const string Mage = Heroes + "/" + nameof(Mage);
        public const string Dumbledore = Mage + "/" + nameof(Dumbledore);
        public const string Witch = Mage + "/" + nameof(Witch);
        public const string Gerald = Mage + "/" + nameof(Gerald);
        public const string Prophet = Mage + "/" + nameof(Prophet);
        #endregion
        
        #region Healers
        public const string Healer = Heroes + "/" + nameof(Healer);
        public const string Fairy = Healer + "/" + nameof(Fairy);
        #endregion
        
        #region Engineers
        public const string Engineer = Heroes + "/" + nameof(Engineer);
        public const string Raccoon = Engineer + "/" + nameof(Raccoon);
        #endregion
        
        #endregion
        #endregion
        
        public static IEnumerable<string> Scopes => new string[]
        {
            Global,
            Heroes,
            Warrior,
            Ogre,
            Mage,
            Dumbledore,
            Healer,
            Fairy,
            Engineer,
            Raccoon,
            Witch,
            Gerald,
            Prophet,
        };
        
        public static IEnumerable<string> EntityScopes => new string[]
        {
            Ogre,
            Dumbledore,
            Fairy,
            Raccoon,
            Witch,
            Gerald,
            Prophet,
        };
    };
}