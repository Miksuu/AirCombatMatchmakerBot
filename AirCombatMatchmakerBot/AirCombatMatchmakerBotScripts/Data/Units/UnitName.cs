using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Runtime.Serialization;

[JsonConverter(typeof(StringEnumConverter))]
public enum UnitName
{
    [EnumMember(Value = "Plane not selected")]
    NOTSELECTED,

    [EnumMember(Value = "F-5E-3")]
    F5E3,

    [EnumMember(Value = "MiG-21Bis")]
    MIG21BIS,

    [EnumMember(Value = "AJS37")]
    AJS37,

    [EnumMember(Value = "Mirage-F1CE")]
    MIRAGEF1CE,

    [EnumMember(Value = "F/A-18C")]
    FA18C,

    [EnumMember(Value = "F-16C")]
    F16C,

    [EnumMember(Value = "Mirage 2000C")]
    M2000C,

    [EnumMember(Value = "JF-17")]
    JF17,

    [EnumMember(Value = "Su-27")]
    SU27,

    [EnumMember(Value = "F-14B")]
    F14B,

    [EnumMember(Value = "F-15C")]
    F15C,

    [EnumMember(Value = "F-15E")]
    F15E,

    /*
    [EnumMember(Value = "SA342L")]
    SA342L,

    [EnumMember(Value = "SA342M")]
    SA342M,

    [EnumMember(Value = "SA342Mistral")]
    SA342MISTRAL, */
    /*
    [EnumMember(Value = "A-10A")]
    A10A,*/

    /*
    [EnumMember(Value = "UH-1H")]
    UH1H,*/

    /*
    [EnumMember(Value = "Su-25")]
    SU25,*/

    /*
    [EnumMember(Value = "Mi-8MT")]
    MI8MT,*/

    /*
    [EnumMember(Value = "Mi-24P")]
    MI24P,*/
}