using System.Collections.Generic;

namespace Battle.Data
{
    public static class GameScopes
    {
        public const string Global = nameof(Global);
        
        public const string Heroes = Global + "/" + nameof(Heroes);

        public const string Warrior = Heroes + "/" + nameof(Warrior);
        public const string Ogre = Warrior + "/" + nameof(Ogre);
        
        public const string Mage = Heroes + "/" + nameof(Mage);
        public const string Dumbledore = Mage + "/" + nameof(Dumbledore);
        
        public const string Healer = Heroes + "/" + nameof(Healer);
        public const string Fairy = Healer + "/" + nameof(Fairy);
        
        public const string Engineer = Heroes + "/" + nameof(Engineer);

        public const string Raccoon = Engineer + "/" + nameof(Raccoon);

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
        };
        
        public static IEnumerable<string> EntityScopes => new string[]
        {
            Ogre,
            Dumbledore,
            Fairy,
            Raccoon,
        };
    };
}