namespace Application.Models;

internal class LanguageItem
{
    public string LangKey { get; } = "";
    public string LangName { get; } = "";
    public readonly IDictionary<string, TranslateItem> Dictionary = new Dictionary<string, TranslateItem>();
    public LanguageItem() { }
    public LanguageItem(string langKey, string langName)
    {
        LangKey = langKey;
        LangName = langName;
    }
}