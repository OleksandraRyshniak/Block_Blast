using Block_Blast.Models;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Storage;


namespace Block_Blast.Services
{
    public class ThemeService
    {
        // ── Поля ──────────────────────────────────────────────
        private const string ThemeKey = "selected_theme";
        private List<Theme> _themes;

        // ── Свойства ──────────────────────────────────────────
        public Theme Current { get; private set; }

        // ── Конструктор ───────────────────────────────────────
        public ThemeService()
        {
            _themes = CreateThemes();

            // Загружаем сохранённую тему (по умолчанию — светлая)
            string savedName = Preferences.Get(ThemeKey, "Light");
            Current = _themes.FirstOrDefault(t => t.Name == savedName) ?? _themes[0];
        }

        // ── Методы ────────────────────────────────────────────

        // Возвращает все доступные темы
        public List<Theme> GetThemes() => _themes;

        // Устанавливает тему по имени и сохраняет выбор
        public void SetTheme(string name)
        {
            var theme = _themes.FirstOrDefault(t => t.Name == name);
            if (theme != null)
            {
                Current = theme;
                Preferences.Set(ThemeKey, name);
            }
        }

        // Устанавливает тему напрямую
        public void SetTheme(Theme theme)
        {
            Current = theme;
            Preferences.Set(ThemeKey, theme.Name);
        }

        // ── Создание тем ──────────────────────────────────────
        private List<Theme> CreateThemes()
        {
            return new List<Theme>
    {
        // Light — синяя рамка как на скрине
   new Theme(
    name:            "Light",
    backgroundColor: Color.FromArgb("#F0F0F8"),   // чуть голубоватый белый
    textColor:       Color.FromArgb("#0D0D1A"),   // почти чёрный — хорошо виден
    accentColor:     Color.FromArgb("#0099CC"),   // насыщенный синий вместо бледного
    cellEmptyColor:  Color.FromArgb("#DCDCF0"),   // светло-серый с фиолетовым
    cellBorderColor: Color.FromArgb("#9999BB")    // средний серо-синий
),
        // Dark — красная рамка как на скрине, текст светлый
        new Theme(
            name:            "Dark",
            backgroundColor: Color.FromArgb("#1A1A2E"),
            textColor:       Color.FromArgb("#FFFFFF"),   // был #E0E0E0 — теперь белый, чётче
            accentColor:     Color.FromArgb("#E94560"),
            cellEmptyColor:  Color.FromArgb("#16213E"),
            cellBorderColor: Color.FromArgb("#0F3460")
        ),

        // Colorful — красная/розовая рамка, белый текст
        new Theme(
            name:            "Colorful",
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
