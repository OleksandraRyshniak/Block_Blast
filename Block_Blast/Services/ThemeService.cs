using Block_Blast.Models;
using Block_Blast.Resources.Localization;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Storage;


namespace Block_Blast.Services
{
    public class ThemeService
    {
        
        private const string ThemeKey = "selected_theme";
        private List<Theme> _themes;

        public Theme Current { get; private set; }

       
        public ThemeService()
        {
            _themes = CreateThemes();

            string savedName = Preferences.Get(ThemeKey, AppResources.light);
            Current = _themes.FirstOrDefault(t => t.Name == savedName) ?? _themes[0];
        }

        public List<Theme> GetThemes() => _themes;

        public void SetTheme(string name)
        {
            var theme = _themes.FirstOrDefault(t => t.Name == name);
            if (theme != null)
            {
                Current = theme;
                Preferences.Set(ThemeKey, name);
            }
        }

        public void SetTheme(Theme theme)
        {
            Current = theme;
            Preferences.Set(ThemeKey, theme.Name);
        }

        private List<Theme> CreateThemes()
        {
            return new List<Theme>
    {

   new Theme(
    name:            AppResources.light,
    backgroundColor: Color.FromArgb("#F0F0F8"),   
    textColor:       Color.FromArgb("#0D0D1A"),   
    accentColor:     Color.FromArgb("#0099CC"),  
    cellEmptyColor:  Color.FromArgb("#DCDCF0"),   
    cellBorderColor: Color.FromArgb("#9999BB")    
),
        
        new Theme(
            name:             AppResources.dark,
            backgroundColor: Color.FromArgb("#1A1A2E"),
            textColor:       Color.FromArgb("#FFFFFF"),   
            accentColor:     Color.FromArgb("#E94560"),
            cellEmptyColor:  Color.FromArgb("#16213E"),
            cellBorderColor: Color.FromArgb("#0F3460")
        ),

        
        new Theme(
            name:             AppResources.colorful,
            backgroundColor: Color.FromArgb("#2D1B69"),
            textColor:       Color.FromArgb("#FFFFFF"),
            accentColor:     Color.FromArgb("#FF6B6B"),
            cellEmptyColor:  Color.FromArgb("#3D2B7D"),
            cellBorderColor: Color.FromArgb("#6C4FBF")
        )
    };
        }
    
    }

}
