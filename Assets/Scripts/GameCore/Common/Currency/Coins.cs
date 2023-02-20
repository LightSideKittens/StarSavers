public class Coins : BaseCurrency<Coins>
{
    protected override int DefaultValue { get; } = 100;
}

public class Crystal : BaseCurrency<Crystal>
{
    protected override int DefaultValue { get; } = 100;
}

public class DarkElexir : BaseCurrency<DarkElexir>
{
    protected override int DefaultValue { get; } = 100;
}


public class Metal : BaseCurrency<Metal>
{
    protected override int DefaultValue { get; } = 100;
}
