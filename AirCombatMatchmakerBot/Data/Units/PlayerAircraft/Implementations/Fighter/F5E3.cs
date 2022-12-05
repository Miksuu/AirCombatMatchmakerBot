using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

[DataContract]
public class F5E3 : PlayerAircraft
{
    public F5E3()
    {
        unitName = UnitName.F5E3;
        unitEras = new List<Era> {
            Era.COLD_WAR,
        };
    }
}
