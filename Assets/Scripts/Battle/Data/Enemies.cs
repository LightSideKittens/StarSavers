namespace Battle.Data
{
    public class Enemies : ObjectsById<Unit>
    {
        protected override IdGroup IdGroup { get; }
    }
}