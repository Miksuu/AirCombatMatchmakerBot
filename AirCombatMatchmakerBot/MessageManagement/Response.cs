public class Response
{
    public string responseString;
    public bool serialize;

    public Response(string _responseString, bool _serialize)
    {
        Log.WriteLine("Init " + nameof(responseString) +
            " with: " + _responseString + " | serialize: " + _serialize, LogLevel.VERBOSE);
        this.responseString = _responseString;
        this.serialize = _serialize;
    }
}