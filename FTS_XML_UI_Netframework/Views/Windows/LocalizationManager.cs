using System.Globalization;

namespace SelfScale.L10n;

public static class LocalizationManager
{
    /// <summary>
    /// Указывает ресурсам, что будет использован язык из параметра.
    /// </summary>
    /// <param name="culture">
    /// en-US, ru-RU, uz-Latn
    /// </param>
    public static void SetCulture(string culture)
    {
        if (CultureInfo.GetCultureInfoByIetfLanguageTag(culture) != null)
        {
            Localization.Culture = new CultureInfo(culture, true);
            App.SetSetting("lang", Localization.Culture.Name);
        }
    }

    /// <summary>
    /// Возвращает выбранную культуру из ресурсов
    /// </summary>
    public static CultureInfo GetCulture()
    {
        return Localization.Culture;
    }
    
    /// <summary>
    /// Возвращает перевод из параметра
    /// </summary>
    public static string GetString(string property)
    {
        if (!string.IsNullOrWhiteSpace(property))
        {
            return Localization.ResourceManager.GetString(property, Localization.Culture) ?? "";
        }

        return "";
    }
}
