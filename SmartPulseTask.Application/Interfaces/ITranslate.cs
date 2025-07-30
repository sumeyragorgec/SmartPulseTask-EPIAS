
using Application.Models;
using SmartPulseTask.Application; 
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Application.Services;

public interface ITranslate
{
    void InitializeLang(string langKey, string langName);
    void AddItem(string lang, TranslateItem item);
    void RemoveByKey(string lang, string key);
    void AddRange(string lang, IEnumerable<TranslateItem> items)
    {
        foreach (var item in items)
        {
            AddItem(lang, item);
        }
    }
    void RemoveItem(string lang, TranslateItem item)
    {
        RemoveByKey(lang, item.Key);
    }
    string GetByKey(string key, params string[] @params);
    string GetByKey(string key)
    {
        return GetByKey(key, []);
    }
    string GetLanguageCode();

    void SelectLanguage(string lang);
    IList<LanguageInfo> GetLanguages();
}

internal class  Translate : ITranslate
{
    private readonly IDictionary<string, LanguageItem> LanguageLibrary = new Dictionary<string, LanguageItem>();
    private string SelectedLanguage { get; set; } = "tr";
    public  Translate()
    {
        LoadDirectory();
    }

    public void InitializeLang(string langKey, string langName)
    {
        if (string.IsNullOrEmpty(langKey))
        {
            throw new ArgumentNullException(nameof(langKey));
        }
        if (string.IsNullOrEmpty(langName))
        {
            throw new ArgumentNullException(nameof(langName));
        }
        if (LanguageLibrary.ContainsKey(langKey)) return;
        LanguageLibrary.Add(langKey, new(langKey, langName));
    }
    public void AddItem(string lang, TranslateItem item)
    {
        if (string.IsNullOrEmpty(lang))
        {
            throw new ArgumentNullException(nameof(lang));
        }
        if (!LanguageLibrary.ContainsKey(lang))
        {
            throw new ArgumentOutOfRangeException(nameof(lang));
        }
        if (string.IsNullOrEmpty(item.Key))
        {
            throw new ArgumentNullException(nameof(item.Key));
        }
        var dict = LanguageLibrary[lang].Dictionary;
        if (!dict.ContainsKey(item.Key))
        {
            dict.Add(item.Key, item);
            return;
        }
        dict[item.Key] = item;
    }
    public void RemoveByKey(string lang, string key)
    {
        if (string.IsNullOrEmpty(lang))
        {
            throw new ArgumentNullException(nameof(lang));
        }
        if (!LanguageLibrary.ContainsKey(lang))
        {
            throw new ArgumentOutOfRangeException(nameof(lang));
        }
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentNullException(nameof(key));
        }
        var dict = LanguageLibrary[lang].Dictionary;
        if (dict.ContainsKey(key))
        {
            dict.Remove(key);
            return;
        }
    }
    public string GetByKey(string key, params string[] @params)
    {
        if (!LanguageLibrary.ContainsKey(SelectedLanguage))
        {
            return "-!-";
        }
        if (LanguageLibrary[SelectedLanguage].Dictionary.ContainsKey(key))
        {
            for (int i = 0; i < @params.Length; i++)
            {
                @params[i] = GetByKey(@params[i]);
            }
            return string.Format(LanguageLibrary[SelectedLanguage].Dictionary[key].Value, @params);
        }
        return key;
    }
    public void SelectLanguage(string lang)
    {
        if (!LanguageLibrary.ContainsKey(lang))
        {
            throw new ArgumentOutOfRangeException(nameof(lang));
        }
        SelectedLanguage = lang;
    }
    private void LoadDirectory()
    {
        string appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        string[] jsonFiles = Directory.GetFiles(Path.Combine(appPath, "Assets", "Lang"), "*.json");
        List<string> keyList = new();
        foreach (var item in typeof(LanguageConstants).GetFields())
        {
            keyList.Add(item.GetRawConstantValue()?.ToString() ?? "");
        }
        var nestedTypes = typeof(LanguageConstants).GetNestedTypes(BindingFlags.Public);
        foreach (var nestedType in nestedTypes)
        {
            foreach (var item in nestedType.GetFields())
            {
                keyList.Add(item.GetRawConstantValue()?.ToString() ?? "");
            }
        }
        keyList = keyList.Where(c => c.Length > 0).ToList();
        for (int i = 0; i < jsonFiles.Length; i++)
        {
            var file = jsonFiles[i];
            string lang = Path.GetFileName(file).Split(".")[0];
            string content = File.ReadAllText(file);
            var values = JsonSerializer.Deserialize<IDictionary<string, string>>(content);
            if (!values.ContainsKey("_name")) continue;
            InitializeLang(lang, values["_name"]);
            foreach (var key in values.Keys)
            {
                AddItem(lang, new(key, values[key]));
            }
            var list = keyList.Where(x => !values.Keys.Contains(x));
            if (!list.Any()) continue;
            foreach (var item in list)
            {
                values.Add(item, item);
            }
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, 
                WriteIndented = true 
            };

            string json = JsonSerializer.Serialize(values, options);
            File.WriteAllText(file, json);

        }

    }

    public IList<LanguageInfo> GetLanguages()
    {
        return LanguageLibrary.Keys.Select(key => new LanguageInfo()
        {
            Key = key,
            Language = LanguageLibrary[key].LangName
        }).ToList();
    }

    public string GetLanguageCode() => SelectedLanguage;
}
