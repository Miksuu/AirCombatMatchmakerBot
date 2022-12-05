
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[JsonObjectAttribute]
public interface IUnit
{
    public UnitName UnitName { get; set; } // Name of the plane
    public List<Era> UnitEras { get; set; }
}