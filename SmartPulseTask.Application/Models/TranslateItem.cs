namespace Application.Models;

public class TranslateItem
{
    public string Key { get; } = "";
    public string Value { get; } = "";
    public string[] Params { get; set; } = [];
    public TranslateItem(string key, string value, params string[] @params)
    {
        Key = key;
        Value = value;
        Params = @params;
    }
    public TranslateItem(string key, string value)
    {
        Key = key;
        Value = value;
    }
}
