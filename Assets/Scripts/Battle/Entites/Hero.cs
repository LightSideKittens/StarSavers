namespace Battle.Data
{
    public class Hero : Unit
    {
#if UNITY_EDITOR
        protected override string GroupName => "Heroes";
#endif
    }
}