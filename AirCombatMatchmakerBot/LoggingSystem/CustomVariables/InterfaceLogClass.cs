using Newtonsoft.Json;

[JsonObjectAttribute]
public interface InterfaceLogClass
{
    public abstract List<string> GetClassParameters();
}