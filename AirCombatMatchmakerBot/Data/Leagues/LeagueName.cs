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
    [EnumMember(Value = "Modern BFM Guns")]
    MODERNBFMGUNS,

    [EnumMember(Value = "Modern BFM Fox2")]
    MODERNBFMFOXTWO,

    [EnumMember(Value = "Modern BVR")]
    MODERNBVR,
}