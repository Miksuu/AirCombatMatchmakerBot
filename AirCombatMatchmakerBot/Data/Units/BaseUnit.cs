using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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
