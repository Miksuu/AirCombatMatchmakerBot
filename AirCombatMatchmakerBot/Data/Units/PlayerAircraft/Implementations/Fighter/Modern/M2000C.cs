using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

[DataContract]
public class M2000C : PlayerAircraft
{
    public M2000C()
    {
        unitName = UnitName.M2000C;
    }
}