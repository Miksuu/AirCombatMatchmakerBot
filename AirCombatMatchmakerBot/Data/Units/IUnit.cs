
using Newtonsoft.Json;

[JsonObjectAttribute]
public interface IUnit
{
    public UnitName UnitName { get; set; } // Name of the plane
    //public ConcurrentBag<Era> UnitEras { get; set; }
}