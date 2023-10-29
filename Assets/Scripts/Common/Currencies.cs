using System;
using LSCore;
using UnityEngine;
using static Paths;

internal abstract class Paths
{
    public const string Root = "Assets/Art/Sprites/UI/Icons/";
}

[Icon(Root + "coin.png")]
public class Coins : BaseCurrency<Coins> { }
[Icon(Root + "key.png")] public class Keys : BaseCurrency<Keys> { }
[Icon(Root + "gem-red.png")] public class RedGems : BaseCurrency<RedGems> { }
[Icon(Root + "trophy.png")] public class Rank : BaseCurrency<Rank> { }
[Icon(Root + "star.png")] public class Exp : BaseCurrency<Exp> { }
