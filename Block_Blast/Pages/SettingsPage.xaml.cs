using Block_Blast.Resources.Localization;
using Block_Blast.Services;
using System.Globalization;

namespace Block_Blast.Pages;

public partial class SettingsPage : ContentPage
{

   
    private readonly ThemeService _themeService;

    // ══════════════════════════════════════════════════════════
    //  UI-элементы (объявляем здесь, создаём в BuildUI)
    // ══════════════════════════════════════════════════════════

    private Label LblTitle;
    private Label LblTheme;
    private Label LblLanguage;
    private FlexLayout ThemeLayout;
    private FlexLayout LangLayout;
    private Button BtnBack;

    // ══════════════════════════════════════════════════════════
    //  Состояние
    // ══════════════════════════════════════════════════════════

    private bool _isNavigating = false;

    // ══════════════════════════════════════════════════════════
    //  Конструктор
    // ══════════════════════════════════════════════════════════

    public SettingsPage(ThemeService themeService)
    {
        _themeService = themeService;
        


        BuildUI();
        PrepareForEntrance();
    }

    // ══════════════════════════════════════════════════════════
    //  Построение всего UI
    // ══════════════════════════════════════════════════════════

    private void BuildUI()
    {
        BackgroundColor = Color.FromArgb("#0D0D1A");
        Shell.SetNavBarIsVisible(this, false);

        
        var decoTopLeft = new BoxView
        {
            Color = Color.FromArgb("#FF3CAC"),
            Opacity = 0.13,
            WidthRequest = 110,
            HeightRequest = 110,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start
        };

        var decoTopRight = new BoxView
        {
            Color = Color.FromArgb("#FFE500"),
            Opacity = 0.10,
            WidthRequest = 80,
            HeightRequest = 80,
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(0, 50, 0, 0)
        };

        var decoBottomRight = new BoxView
        {
            Color = Color.FromArgb("#00F5FF"),
            Opacity = 0.10,
            WidthRequest = 90,
            HeightRequest = 90,
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.End
        };

        var decoBottomLeft = new BoxView
        {
            Color = Color.FromArgb("#39FF14"),
            Opacity = 0.09,
            WidthRequest = 70,
            HeightRequest = 70,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.End,
            Margin = new Thickness(0, 0, 0, 30)
        };

        // ── Заголовок страницы ────────────────────────────────
        LblTitle = new Label
        {
            Text = AppResources.settings,
            FontFamily = "PressStart2P",
            FontSize = 26,
            TextColor = Color.FromArgb("#00F5FF"),
            HorizontalOptions = LayoutOptions.Start,
            CharacterSpacing = 3,
            Margin = new Thickness(0, 0, 0, 6),
            Shadow = new Shadow
            {
                Brush = new SolidColorBrush(Color.FromArgb("#00F5FF")),
                Offset = new Point(3, 3),
                Radius = 0,
                Opacity = 0.7f
            }
        };

        // Декоративная линия под заголовком
        var titleLine = new BoxView
        {
            Color = Color.FromArgb("#00F5FF"),
            HeightRequest = 2,
            Opacity = 0.4,
            Margin = new Thickness(0, 0, 0, 32)
        };

        // ── Секция: Тема ──────────────────────────────────────
        LblTheme = new Label
        {
            Text = AppResources.theme,
            FontFamily = "PressStart2P",
            FontSize = 9,
            TextColor = Color.FromArgb("#FFE500"),
            CharacterSpacing = 3,
            Margin = new Thickness(0, 0, 0, 10)
        };

        ThemeLayout = new FlexLayout
        {
            Wrap = Microsoft.Maui.Layouts.FlexWrap.Wrap,
            Direction = Microsoft.Maui.Layouts.FlexDirection.Row,
            JustifyContent = Microsoft.Maui.Layouts.FlexJustify.Start,
            AlignItems = Microsoft.Maui.Layouts.FlexAlignItems.Start
        };

        var themeCard = new Border
        {
            BackgroundColor = Color.FromArgb("#1A1A2E"),
            StrokeThickness = 2,
            Stroke = new SolidColorBrush(Color.FromArgb("#FFE500")),
            Padding = new Thickness(16, 14),
            Margin = new Thickness(0, 0, 0, 28),
            StrokeShape = new Microsoft.Maui.Controls.Shapes.Rectangle(),
            Content = ThemeLayout
        };

        // ── Секция: Язык ──────────────────────────────────────
        LblLanguage = new Label
        {
            Text = AppResources.language,
            FontFamily = "PressStart2P",
            FontSize = 9,
            TextColor = Color.FromArgb("#FFE500"),
            CharacterSpacing = 3,
            Margin = new Thickness(0, 0, 0, 10)
        };

        LangLayout = new FlexLayout
        {
            Wrap = Microsoft.Maui.Layouts.FlexWrap.Wrap,
            Direction = Microsoft.Maui.Layouts.FlexDirection.Row,
            JustifyContent = Microsoft.Maui.Layouts.FlexJustify.Start,
            AlignItems = Microsoft.Maui.Layouts.FlexAlignItems.Start
        };

        var langCard = new Border
        {
            BackgroundColor = Color.FromArgb("#1A1A2E"),
            StrokeThickness = 2,
            Stroke = new SolidColorBrush(Color.FromArgb("#39FF14")),
            Padding = new Thickness(16, 14),
            Margin = new Thickness(0, 0, 0, 36),
            StrokeShape = new Microsoft.Maui.Controls.Shapes.Rectangle(),
            Content = LangLayout
        };

        // ── Кнопка назад ─────────────────────────────────────
        BtnBack = new Button
        {
            Text = AppResources.back,
            FontFamily = "PressStart2P",
            FontSize = 10,
            TextColor = Color.FromArgb("#FF3CAC"),
            BackgroundColor = Colors.Transparent,
            BorderColor = Color.FromArgb("#FF3CAC"),
            BorderWidth = 2,
            CornerRadius = 0,
            HeightRequest = 48
        };



        BtnBack.Clicked += OnBackClicked;

        // ── Сборка ScrollView → VerticalStackLayout ───────────
        var stack = new VerticalStackLayout
        {
            Padding = new Thickness(28, 56, 28, 40),
            Spacing = 0,
            Children =
            {
                LblTitle,
                titleLine,
                LblTheme,
                themeCard,
                LblLanguage,
                langCard,
                BtnBack
            }
        };

        var scroll = new ScrollView { Content = stack };

        // ── Корневой Grid (декор + контент) ───────────────────
        var root = new Grid();
        root.Children.Add(decoTopLeft);
        root.Children.Add(decoTopRight);
        root.Children.Add(decoBottomRight);
        root.Children.Add(decoBottomLeft);
        root.Children.Add(scroll);

        Content = root;
    }

    // ══════════════════════════════════════════════════════════
    //  Жизненный цикл
    // ══════════════════════════════════════════════════════════
    private void ApplyLocalization()
    {
        LblTitle.Text = AppResources.settings;
        LblTheme.Text = AppResources.theme;
        LblLanguage.Text = AppResources.language;
        BtnBack.Text =  AppResources.back;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        LanguageService.LanguageChanged += ApplyLocalization;
        ApplyLocalization();

        _isNavigating = false;

        ApplyTheme();
        ApplyLocalization();
        BuildThemeButtons();
        BuildLanguageButtons();

        await PlayEntranceAnimation();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        LanguageService.LanguageChanged -= ApplyLocalization; // важно — отписаться!
    }

    // ══════════════════════════════════════════════════════════
    //  Тема и локализация
    // ══════════════════════════════════════════════════════════

    private void ApplyTheme()
    {
        _themeService.Current.Apply(this);
    }

    // ══════════════════════════════════════════════════════════
    //  Анимации
    // ══════════════════════════════════════════════════════════

    private void PrepareForEntrance()
    {
        LblTitle.Opacity = 0;
        LblTitle.TranslationY = -20;

        LblTheme.Opacity = 0;
        ThemeLayout.Opacity = 0;

        LblLanguage.Opacity = 0;
        LangLayout.Opacity = 0;

        BtnBack.Opacity = 0;
        BtnBack.TranslationY = 20;
    }

    private async Task PlayEntranceAnimation()
    {
        await Task.WhenAll(
            LblTitle.FadeToAsync(1, 300, Easing.CubicOut),
            LblTitle.TranslateToAsync(0, 0, 300, Easing.CubicOut)
        );

        await Task.Delay(60);

        await Task.WhenAll(
            LblTheme.FadeToAsync(1, 220, Easing.CubicOut),
            ThemeLayout.FadeToAsync(1, 220, Easing.CubicOut)
        );

        await Task.Delay(60);

        await Task.WhenAll(
            LblLanguage.FadeToAsync(1, 220, Easing.CubicOut),
            LangLayout.FadeToAsync(1, 220, Easing.CubicOut)
        );

        await Task.Delay(60);

        await Task.WhenAll(
            BtnBack.FadeToAsync(1, 200, Easing.CubicOut),
            BtnBack.TranslateToAsync(0, 0, 200, Easing.CubicOut)
        );
    }

    private static async Task AnimateButtonPress(View button)
    {
        await button.ScaleToAsync(0.93, 80, Easing.CubicIn);
        await button.ScaleToAsync(1.0, 100, Easing.CubicOut);
    }

    // ══════════════════════════════════════════════════════════
    //  Построение кнопок тем
    // ══════════════════════════════════════════════════════════

    private void BuildThemeButtons()
    {
        ThemeLayout.Children.Clear();

        foreach (var theme in _themeService.GetThemes())
        {
            bool isActive = theme.Name == _themeService.Current.Name;

            var btn = new Button
            {
                Text = AppResources.name,
                FontFamily = "PressStart2P",
                FontSize = 10,
                CornerRadius = 0,
                Margin = new Thickness(0, 0, 10, 10),
                HeightRequest = 44,
                BackgroundColor = isActive ? theme.AccentColor : Colors.Transparent,
                TextColor = isActive ? theme.TextColor : theme.AccentColor,
                BorderColor = theme.AccentColor,
                BorderWidth = 2
            };

            var themeName = theme.Name;
            btn.Clicked += async (s, e) =>
            {
                await AnimateButtonPress(btn);
                _themeService.SetTheme(themeName);
                ApplyTheme();
                BuildThemeButtons();
                BuildLanguageButtons();
            };

            ThemeLayout.Children.Add(btn);
        }
    }

    // ══════════════════════════════════════════════════════════
    //  Построение кнопок языков
    // ══════════════════════════════════════════════════════════

    private void BuildLanguageButtons()
    {
        LangLayout.Children.Clear();

        var languages = new List<(string Code, string Name)>
    {
        ("et", "Eesti"),
        ("en", "English"),
    };

        foreach (var lang in languages)
        {
            bool isActive = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == lang.Code;

            var btn = new Button
            {
                Text = lang.Name,
                FontFamily = "PressStart2P",
                FontSize = 10,
                CornerRadius = 0,
                Margin = new Thickness(0, 0, 10, 10),
                HeightRequest = 44,
                BackgroundColor = isActive
                    ? _themeService.Current.AccentColor
                    : Colors.Transparent,
                TextColor = isActive
                    ? _themeService.Current.TextColor
                    : _themeService.Current.AccentColor,
                BorderColor = _themeService.Current.AccentColor,
                BorderWidth = 2
            };

            var code = lang.Code;

            btn.Clicked += async (s, e) =>
            {
                await AnimateButtonPress(btn);
                LanguageService.ChangeLanguage(code);
                BuildLanguageButtons();
            };

            LangLayout.Children.Add(btn);
        }
    }

    // ══════════════════════════════════════════════════════════
    //  Кнопка назад
    // ══════════════════════════════════════════════════════════

    private async void OnBackClicked(object sender, EventArgs e)
    {
        if (_isNavigating) return;

        _isNavigating = true;

        await AnimateButtonPress(BtnBack);
        await Navigation.PopAsync();
    }
}