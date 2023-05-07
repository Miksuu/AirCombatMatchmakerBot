public class CustomUlong
{
    private ulong _value;

    public ulong Value
    {
        get
        {
            Log.WriteLine("Getting " + nameof(CustomUlong) + ": " + _value, LogLevel.GET_VERBOSE);
            return _value;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(CustomUlong) + ": " + _value, LogLevel.SET_VERBOSE);
            _value = value;
        }
    }
}