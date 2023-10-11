namespace Battle.Data.GameProperty
{
    public struct PropBuilder
    {
        private string value;

        public static implicit operator string(PropBuilder a)
        {
            return a.value;
        }
        
        public static PropBuilder Create()
        {
            return new PropBuilder();
        }

        public PropBuilder Float(string name, float floatValue)
        {
            value += $"{name}:{floatValue} ";
            
            return this;
        }
    }
}