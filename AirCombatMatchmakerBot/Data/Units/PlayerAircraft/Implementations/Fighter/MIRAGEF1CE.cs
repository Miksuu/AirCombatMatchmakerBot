using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

[DataContract]
public class MIRAGEF1CE : PlayerAircraft
{
    public MIRAGEF1CE()
    {
        unitName = UnitName.MIRAGEF1CE;
        unitEras = new List<Era> {
            Era.COLD_WAR,
        };
    }
}