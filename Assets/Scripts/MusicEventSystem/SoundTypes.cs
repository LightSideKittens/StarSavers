public static class SoundTypes
{
    public const string Kick = nameof(Kick);
    public const string PluckBassChord = nameof(PluckBassChord);

    public const string MajorMelody = nameof(MajorMelody);
    public const string MinorMelody = nameof(MinorMelody);
    
    public const string Bass = nameof(Bass);
    public const string NoiseLeadPad = nameof(NoiseLeadPad);
    
    public const string ClapSnare = nameof(ClapSnare);
    public const string HatPercussion = nameof(HatPercussion);

    public static string[] Types { get; } =
    {
        Kick,
        PluckBassChord,
        
        MajorMelody,
        MinorMelody,
        
        Bass,
        NoiseLeadPad,
        
        ClapSnare,
        HatPercussion,
    };
}
