﻿
using Newtonsoft.Json;

[JsonObjectAttribute]
public interface InterfaceUnit
{
    public UnitName UnitName { get; set; } // Name of the plane
    //public ConcurrentBag<Era> UnitEras { get; set; }
}