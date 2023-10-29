namespace Battle.Data
{
    public class Heroes : ObjectsById<Unit>
    {
        protected override IdGroup IdGroup { get; }
    }
}