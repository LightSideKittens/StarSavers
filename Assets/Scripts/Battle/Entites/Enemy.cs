namespace Battle.Data
{
    public class Enemy : Unit
    {
#if UNITY_EDITOR
        protected override string GroupName => "Enemies";
#endif
    }
}