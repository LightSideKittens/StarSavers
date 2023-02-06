using Battle.MusicEventSystem.Soundvent;

public static class SoundventTypes
{
    [Soundvent(true, "Sound")] public const string ShortIV = nameof(ShortIV);
    [Soundvent(true, "Sound")] public const string ShortIII = nameof(ShortIII);
    [Soundvent(true, "Sound")] public const string ShortII = nameof(ShortII);
    [Soundvent(true, "Sound")] public const string ShortI = nameof(ShortI);
    
    [Soundvent(false, "Sound")] public const string LongII = nameof(LongII);
    [Soundvent(false, "Sound")] public const string LongI = nameof(LongI);
    
    [Soundvent(true, "Enemy")] public const string EnemyIV = nameof(EnemyIV);
    [Soundvent(true, "Enemy")] public const string EnemyIII = nameof(EnemyIII);
    [Soundvent(true, "Enemy")] public const string EnemyII = nameof(EnemyII);
    [Soundvent(true, "Enemy")] public const string EnemyI = nameof(EnemyI);
    [Soundvent(true, "Boss")] public const string Boss = nameof(Boss);

    public static string[] Sounds { get; } =
    {
        ShortIV,
        ShortIII,
        ShortII,
        ShortI,
        
        LongII,
        LongI,
    };
    
    public static string[] Enemies { get; } =
    {
        EnemyIV,
        EnemyIII,
        EnemyII,
        EnemyI,
        Boss
    };
}
