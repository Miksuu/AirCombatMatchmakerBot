using Newtonsoft.Json;

[JsonObjectAttribute]
public interface InterfaceLoggingClass
{
    public abstract string GetLoggingClassParameters<TKey, TValue>();
}