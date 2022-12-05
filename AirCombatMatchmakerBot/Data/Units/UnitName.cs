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
public enum UnitName
{
    [EnumMember(Value = "F-5E-3")]
    F5E3,

    /*
    [EnumMember(Value = "A-10A")]
    A10A,*/

    /*
    [EnumMember(Value = "UH-1H")]
    UH1H,*/

    [EnumMember(Value = "MiG-21Bis")]
    MIG21BIS,

    /*
    [EnumMember(Value = "Su-25")]
    SU25,*/

    /*
    [EnumMember(Value = "Mi-8MT")]
    MI8MT,*/

    [EnumMember(Value = "AJS37")]
    AJS37,

    /*
    [EnumMember(Value = "Mi-24P")]
    MI24P,*/

    [EnumMember(Value = "Mirage-F1CE")]
    MIRAGEF1CE,

    /*
    [EnumMember(Value = "SA342L")]
    SA342L,

    [EnumMember(Value = "SA342M")]
    SA342M,

    [EnumMember(Value = "SA342Mistral")]
    SA342MISTRAL, */
}