using System.Runtime.Serialization;

[DataContract]
public abstract class BaseUnit : IUnit
{
    UnitName IUnit.UnitName
    {
        get => unitName;
        set => unitName = value;
    }

    public UnitName unitName;

    public BaseUnit()
    {
    }
}
