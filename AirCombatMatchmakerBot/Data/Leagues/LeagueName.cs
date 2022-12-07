using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.Serialization;

[JsonConverter(typeof(StringEnumConverter))]
public enum LeagueName
{
    [EnumMember(Value = "1v1 Modern BFM Guns")]
    ONEMODERNBFMGUNS,

    [EnumMember(Value = "1v1 Modern BFM Fox2")]
    ONEMODERNBFMFOXTWO,

    /*
    [EnumMember(Value = "2v2 Modern BVR")]
    TWOMODERNBVR,*/
}