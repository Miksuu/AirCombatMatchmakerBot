using System.Runtime.Serialization;

[DataContract]
public class F16C : PlayerAircraft
{
    public F16C()
    {
        unitName = UnitName.F16C;
    }
}