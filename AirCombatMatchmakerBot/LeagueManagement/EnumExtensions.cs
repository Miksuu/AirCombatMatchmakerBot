using System.Runtime.Serialization;

public static class EnumExtensions
{
    public static object GetInstance(string _string)
    {
        Log.WriteLine("string: " + _string, LogLevel.VERBOSE);

        Type? t = Type.GetType(_string.ToUpper());
        return Activator.CreateInstance(t);
    }

    public static string? GetEnumMemberAttrValue(object enumVal)
    {
        var memInfo = enumVal.GetType().GetMember(enumVal.ToString());
        var attr = memInfo[0].GetCustomAttributes(false).OfType<EnumMemberAttribute>().FirstOrDefault();
        if (attr != null)
        {
            return attr.Value;
        }
        return null;
    }
}