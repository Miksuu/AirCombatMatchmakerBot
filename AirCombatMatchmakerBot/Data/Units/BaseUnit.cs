using System.Runtime.Serialization;

[DataContract]
public abstract class BaseUnit : InterfaceUnit
{
    UnitName InterfaceUnit.UnitName
    {
        get => unitName;
        set => unitName = value;
    }

    public UnitName unitName;

    public BaseUnit()
    {
    }
}
