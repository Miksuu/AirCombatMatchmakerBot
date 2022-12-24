using System.Runtime.Serialization;

public static class EnumExtensions
{
    public static object GetInstance(string _string)
    {
        Log.WriteLine("GetInstance string: " + _string, LogLevel.VERBOSE);

        Type? t = Type.GetType(_string.ToUpper());

        Log.WriteLine("Generated type: " + t, LogLevel.VERBOSE);
        if (t == null)
        {
            Log.WriteLine("type was null for: " + _string.ToUpper(), LogLevel.CRITICAL);
            return new object();
        }

        var instance = Activator.CreateInstance(t);

        Log.WriteLine("Generated instance: " + instance, LogLevel.VERBOSE);

        if (instance == null)
        {
            Log.WriteLine("instance was null for: " + _string.ToUpper(), LogLevel.CRITICAL);
            return t;
        }

        return instance;
    }

    public static string? GetEnumMemberAttrValue(object enumVal)
    {
        string? enumValString = enumVal.ToString();
        if (enumValString == null)
        {
            return "null";
        }

        var memInfo = enumVal.GetType().GetMember(enumValString);
        var attr = memInfo[0].GetCustomAttributes(false).OfType<EnumMemberAttribute>().FirstOrDefault();
        if (attr == null)
        {
            Log.WriteLine(nameof(attr) + " was null!", LogLevel.CRITICAL);
            return null;
        }
        return attr.Value;
    }
}