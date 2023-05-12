using Newtonsoft.Json;

[JsonObjectAttribute]
public interface InterfaceLoggableClass
{
    public abstract List<string> GetClassParameters();
}