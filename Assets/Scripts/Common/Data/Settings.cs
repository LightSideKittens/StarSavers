using LSCore.ConfigModule;
using Newtonsoft.Json;
using UnityEngine.Localization.Settings;

namespace Common.Data
{
    public class Settings : BaseSingleConfig<Settings>
    {
        [JsonProperty] private string languageCode;
        
        public string LanguageCode
        {
            get => languageCode;
            set
            {
                if (value != languageCode)
                {
                    var locale = LocalizationSettings.AvailableLocales.GetLocale(value);
                    LocalizationSettings.SelectedLocale = locale;
                    languageCode = value;
                }
            }
        }
    }
}