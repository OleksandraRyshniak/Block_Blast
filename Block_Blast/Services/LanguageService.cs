using Block_Blast.Resources.Localization;
using System.Globalization;

public static class LanguageService
{
    private const string LangKey = "selected_language";
    public static event Action? LanguageChanged;

    public static void ChangeLanguage(string languageCode)
    {
        var culture = new CultureInfo(languageCode);
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
        AppResources.Culture = culture;

        // Заменяем объект в словаре → DynamicResource обновится сам
        if (Application.Current?.Resources != null)
            Application.Current.Resources["AppStrings"] = new AppResources();

        Preferences.Set(LangKey, languageCode);
        LanguageChanged?.Invoke();
    }

    public static void LoadSaved()
    {
        string code = Preferences.Get(LangKey, "en");
        ChangeLanguage(code);
    }
}