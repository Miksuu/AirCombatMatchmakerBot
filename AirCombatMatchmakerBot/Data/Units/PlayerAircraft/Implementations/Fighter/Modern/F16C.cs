using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

[DataContract]
public class F16C : PlayerAircraft
{
    public F16C()
    {
        unitName = UnitName.F16C;
    }
}