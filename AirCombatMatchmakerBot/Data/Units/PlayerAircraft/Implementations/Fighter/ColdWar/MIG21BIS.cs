using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

[DataContract]
public class MIG21BIS : PlayerAircraft
{
    public MIG21BIS()
    {
        unitName = UnitName.MIG21BIS;
    }
}