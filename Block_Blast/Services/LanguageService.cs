using Block_Blast.Resources.Localization;
using System.Globalization;

public static class LanguageService
{
    public static event Action? LanguageChanged;


    public static void ChangeLanguage(string languageCode)
    {
        var culture = new CultureInfo(languageCode);
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;

        AppResources.Culture = culture;
        LanguageChanged?.Invoke();
    }
}