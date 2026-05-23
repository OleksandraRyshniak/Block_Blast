using Block_Blast.Resources.Localization;
using Block_Blast.Services;
using Microsoft.Maui.Controls.Shapes;
using System.Globalization;

namespace Block_Blast.Pages;

public partial class SettingsPage : ContentPage
{
    // ── Сервисы ───────────────────────────────────────────────
    private readonly ThemeService _themeService;
    private readonly AccountService _accountService;
    private readonly StartPage _startPage;

    // ── UI ────────────────────────────────────────────────────
    private Label LblTitle;
    private Label LblTheme;
    private Label LblLanguage;
    private Label LblAccount;
    private FlexLayout ThemeLayout;
    private FlexLayout LangLayout;
    private Button BtnLogout;
    private Button BtnBack;

    private bool _isNavigating = false;

    // ── Конструктор ───────────────────────────────────────────
    public SettingsPage(
        ThemeService themeService,
        AccountService accountService,
        StartPage startPage)
    {
        _themeService = themeService;
        _accountService = accountService;
        _startPage = startPage;

        BuildUI();
        PrepareForEntrance();
    }

    // ── UI ────────────────────────────────────────────────────
    private void BuildUI()
    {
        BackgroundColor = Color.FromArgb("#0D0D1A");
        Shell.SetNavBarIsVisible(this, false);

        // Декор
        var decoTopLeft = MakeDecoBox("#FF3CAC", 0.13, 110, 110,
            LayoutOptions.Start, LayoutOptions.Start, Thickness.Zero);
        var decoTopRight = MakeDecoBox("#FFE500", 0.10, 80, 80,
            LayoutOptions.End, LayoutOptions.Start, new Thickness(0, 50, 0, 0));
        var decoBottomRight = MakeDecoBox("#00F5FF", 0.10, 90, 90,
            LayoutOptions.End, LayoutOptions.End, Thickness.Zero);
        var decoBottomLeft = MakeDecoBox("#39FF14", 0.09, 70, 70,
            LayoutOptions.Start, LayoutOptions.End, new Thickness(0, 0, 0, 30));

        // Заголовок
        LblTitle = MakePixelLabel(AppResources.settings, 26, "#00F5FF");
        LblTitle.Shadow = new Shadow
        {
            Brush = new SolidColorBrush(Color.FromArgb("#00F5FF")),
            Offset = new Point(3, 3),
            Radius = 0,
            Opacity = 0.7f
        };

        var titleLine = new BoxView
        {
            Color = Color.FromArgb("#00F5FF"),
            HeightRequest = 2,
            Opacity = 0.4,
            Margin = new Thickness(0, 0, 0, 32)
        };

        // Тема
        LblTheme = MakePixelLabel(AppResources.theme, 9, "#FFE500");
        LblTheme.Margin = new Thickness(0, 0, 0, 10);

        ThemeLayout = new FlexLayout
        {
            Wrap = Microsoft.Maui.Layouts.FlexWrap.Wrap,
            Direction = Microsoft.Maui.Layouts.FlexDirection.Row,
            JustifyContent = Microsoft.Maui.Layouts.FlexJustify.Start,
            AlignItems = Microsoft.Maui.Layouts.FlexAlignItems.Start
        };

        var themeCard = MakeCard("#FFE500", ThemeLayout, new Thickness(0, 0, 0, 28));

        // Язык
        LblLanguage = MakePixelLabel(AppResources.language, 9, "#FFE500");
        LblLanguage.Margin = new Thickness(0, 0, 0, 10);

        LangLayout = new FlexLayout
        {
            Wrap = Microsoft.Maui.Layouts.FlexWrap.Wrap,
            Direction = Microsoft.Maui.Layouts.FlexDirection.Row,
            JustifyContent = Microsoft.Maui.Layouts.FlexJustify.Start,
            AlignItems = Microsoft.Maui.Layouts.FlexAlignItems.Start
        };

        var langCard = MakeCard("#39FF14", LangLayout, new Thickness(0, 0, 0, 28));

        // Аккаунт
        LblAccount = MakePixelLabel("ACCOUNT", 9, "#FF3CAC");
        LblAccount.Margin = new Thickness(0, 0, 0, 10);

        BtnLogout = new Button
        {
            FontFamily = "PressStart2P",
            FontSize = 9,
            TextColor = Color.FromArgb("#FF3CAC"),
            BackgroundColor = Colors.Transparent,
            BorderColor = Color.FromArgb("#FF3CAC"),
            BorderWidth = 2,
            CornerRadius = 0,
            HeightRequest = 44,
            Margin = new Thickness(0, 0, 0, 28)
        };
        BtnLogout.Clicked += OnLogoutClicked;

        // Кнопка назад
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

        var stack = new VerticalStackLayout
        {
            Padding = new Thickness(28, 56, 28, 40),
            Spacing = 0,
            Children =
            {
                LblTitle, titleLine,
                LblTheme, themeCard,
                LblLanguage, langCard,
                LblAccount, BtnLogout,
                BtnBack
            }
        };

        var scroll = new ScrollView { Content = stack };

        var root = new Grid();
        root.Children.Add(decoTopLeft);
        root.Children.Add(decoTopRight);
        root.Children.Add(decoBottomRight);
        root.Children.Add(decoBottomLeft);
        root.Children.Add(scroll);

        Content = root;
    }

    // ── Жизненный цикл ────────────────────────────────────────
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        LanguageService.LanguageChanged += ApplyLocalization;
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
        LanguageService.LanguageChanged -= ApplyLocalization;
    }

    // ── Тема и локализация ────────────────────────────────────
    private void ApplyTheme() => _themeService.Current.Apply(this);

    private void ApplyLocalization()
    {
        LblTitle.Text = AppResources.settings;
        LblTheme.Text = AppResources.theme;
        LblLanguage.Text = AppResources.language;
        BtnBack.Text = AppResources.back;

        // Текст кнопки выхода с именем текущего игрока
        var name = _accountService.GetCurrentName();
        BtnLogout.Text = name != null
            ? $"EXIT: {name.ToUpper()}"
            : "LOGOUT";
    }

    // ── Анимации ──────────────────────────────────────────────
    private void PrepareForEntrance()
    {
        LblTitle.Opacity = 0; LblTitle.TranslationY = -20;
        LblTheme.Opacity = 0; ThemeLayout.Opacity = 0;
        LblLanguage.Opacity = 0; LangLayout.Opacity = 0;
        LblAccount.Opacity = 0; BtnLogout.Opacity = 0;
        BtnBack.Opacity = 0; BtnBack.TranslationY = 20;
    }

    private async Task PlayEntranceAnimation()
    {
        await Task.WhenAll(
            LblTitle.FadeToAsync(1, 300, Easing.CubicOut),
            LblTitle.TranslateToAsync(0, 0, 300, Easing.CubicOut));
        await Task.Delay(60);
        await Task.WhenAll(
            LblTheme.FadeToAsync(1, 220, Easing.CubicOut),
            ThemeLayout.FadeToAsync(1, 220, Easing.CubicOut));
        await Task.Delay(60);
        await Task.WhenAll(
            LblLanguage.FadeToAsync(1, 220, Easing.CubicOut),
            LangLayout.FadeToAsync(1, 220, Easing.CubicOut));
        await Task.Delay(60);
        await Task.WhenAll(
            LblAccount.FadeToAsync(1, 200, Easing.CubicOut),
            BtnLogout.FadeToAsync(1, 200, Easing.CubicOut));
        await Task.Delay(60);
        await Task.WhenAll(
            BtnBack.FadeToAsync(1, 200, Easing.CubicOut),
            BtnBack.TranslateToAsync(0, 0, 200, Easing.CubicOut));
    }

    private static async Task AnimateButtonPress(View btn)
    {
        await btn.ScaleToAsync(0.93, 80, Easing.CubicIn);
        await btn.ScaleToAsync(1.0, 100, Easing.CubicOut);
    }

    // ── Кнопки тем / языков ───────────────────────────────────
    private void BuildThemeButtons()
    {
        ThemeLayout.Children.Clear();
        foreach (var theme in _themeService.GetThemes())
        {
            bool isActive = theme.Name == _themeService.Current.Name;
            var btn = new Button
            {
                Text = theme.Name,
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
                BackgroundColor = isActive ? _themeService.Current.AccentColor : Colors.Transparent,
                TextColor = isActive ? _themeService.Current.TextColor : _themeService.Current.AccentColor,
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

    // ── Выход из аккаунта ─────────────────────────────────────
    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert(
            "ACCOUNT",
            "Exit account and switch player?",
            "Yes", "No");

        if (!confirm) return;

        await AnimateButtonPress(BtnLogout);
        _accountService.Logout();

        // Возвращаемся на StartPage, затем открываем LoginPage
        // canCancel=true — можно вернуться (аккаунт уже был)
        await Navigation.PopAsync();
        await _startPage.GoToLogin(canCancel: true);
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        if (_isNavigating) return;
        _isNavigating = true;
        await AnimateButtonPress(BtnBack);
        await Navigation.PopAsync();
    }

    // ── Фабричные методы ──────────────────────────────────────
    private static Label MakePixelLabel(string text, double size, string hex) => new Label
    {
        Text = text,
        FontFamily = "PressStart2P",
        FontSize = size,
        TextColor = Color.FromArgb(hex),
        CharacterSpacing = 3
    };

    private static BoxView MakeDecoBox(string hex, double opacity,
        double w, double h,
        LayoutOptions hOpt, LayoutOptions vOpt, Thickness margin) => new BoxView
        {
            Color = Color.FromArgb(hex),
            Opacity = opacity,
            WidthRequest = w,
            HeightRequest = h,
            HorizontalOptions = hOpt,
            VerticalOptions = vOpt,
            Margin = margin
        };

    private static Border MakeCard(string strokeHex, View content, Thickness margin) => new Border
    {
        BackgroundColor = Color.FromArgb("#1A1A2E"),
        StrokeThickness = 2,
        Stroke = new SolidColorBrush(Color.FromArgb(strokeHex)),
        Padding = new Thickness(16, 14),
        Margin = margin,
        StrokeShape = new Microsoft.Maui.Controls.Shapes.Rectangle(),
        Content = content
    };
}