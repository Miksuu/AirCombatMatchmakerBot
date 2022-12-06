using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

[DataContract]
public class FA18C : PlayerAircraft
{
    public FA18C()
    {
        unitName = UnitName.FA18C;
    }
}