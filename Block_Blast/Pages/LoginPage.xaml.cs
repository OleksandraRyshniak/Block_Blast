using Block_Blast.Resources.Localization;
using Block_Blast.Services;

namespace Block_Blast.Pages;

/// <summary>
/// Экран ввода имени.
/// Показывается ТОЛЬКО при первом запуске приложения (имя ещё не сохранено).
/// canCancel = true только если уже есть аккаунт (выход из Settings).
/// </summary>
public partial class LoginPage : ContentPage
{
    // ── Сервисы ───────────────────────────────────────────────
    private readonly AccountService _accountService;
    private readonly ScoreService _scoreService;
    private readonly ThemeService _themeService;
    private readonly bool _canCancel;

    // ── UI ────────────────────────────────────────────────────
    private Label LblTitle;
    private Label LblSubtitle;
    private Entry EntryName;
    private Label LblHint;
    private Button BtnConfirm;
    private Button BtnCancel;
   
    private Grid _rootGrid;

    // ── Результат ─────────────────────────────────────────────
    public string? ResultName { get; private set; }
    private readonly TaskCompletionSource<string?> _tcs = new();

    // ── Конструктор ───────────────────────────────────────────
    public LoginPage(
        AccountService accountService,
        ScoreService scoreService,
        ThemeService themeService,
        bool canCancel = false)
    {
        _accountService = accountService;
        _scoreService = scoreService;
        _themeService = themeService;
        _canCancel = canCancel;

        Shell.SetNavBarIsVisible(this, false);
        BuildUI();
        ApplyTheme();
        PrepareEntrance();
    }

    // ── Построение UI ─────────────────────────────────────────


    // Добавь в поля класса:
    private Border _card;
    private Label LblOneTimeHint;



    // ── Lifecycle ─────────────────────────────────────────────
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await PlayEntrance();
        EntryName.Focus();
    }

    protected override bool OnBackButtonPressed()
    {
        if (_canCancel) OnCancel();
        return true;
    }

    // ── Логика ────────────────────────────────────────────────
    private void OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        LblHint.IsVisible = false;

        string raw = e.NewTextValue ?? "";
        string filtered = new string(
            raw.Where(c => char.IsLetterOrDigit(c) || c == ' ' || c == '-' || c == '_')
               .ToArray());
        while (filtered.Contains("  "))
            filtered = filtered.Replace("  ", " ");

        if (filtered != raw)
            EntryName.Text = filtered;
    }

    private async void OnConfirm()
    {
        string cleaned = (EntryName.Text ?? "").Trim();
        while (cleaned.Contains("  "))
            cleaned = cleaned.Replace("  ", " ");

        if (cleaned.Length == 0)
        {
            await ShowError(AppResources.name_start);
            return;
        }

        if (cleaned.Length > 16)
            cleaned = cleaned[..16];

        await BtnConfirm.ScaleTo(0.93, 80, Easing.CubicIn);
        await BtnConfirm.ScaleTo(1.0, 100, Easing.CubicOut);

        _accountService.SetCurrentName(cleaned);
        ResultName = cleaned;
        _tcs.TrySetResult(cleaned);

        await Navigation.PopAsync();
    }

    private async void OnCancel()
    {
        ResultName = null;
        _tcs.TrySetResult(null);
        await Navigation.PopAsync();
    }

    private async Task ShowError(string message)
    {
        LblHint.Text = message;
        LblHint.TextColor = Color.FromArgb("#FF3CAC");
        LblHint.IsVisible = true;

        for (int i = 0; i < 3; i++)
        {
            await EntryName.TranslateTo(-8, 0, 50);
            await EntryName.TranslateTo(8, 0, 55);
        }
        await EntryName.TranslateTo(0, 0, 50);
    }

    public Task<string?> WaitForResult() => _tcs.Task;

    // ── Анимации ──────────────────────────────────────────────
    private void PrepareEntrance()
    {
        LblTitle.Opacity = 0;
        LblTitle.TranslationY = 0;
        LblTitle.Scale = 0.85;
    }

    private async Task PlayEntrance()
    {
        await Task.WhenAll(
            LblTitle.FadeTo(1, 400, Easing.CubicOut),
            LblTitle.TranslateTo(0, 0, 400, Easing.CubicOut),
            LblTitle.ScaleTo(1.0, 400, Easing.CubicOut));
    }

    // ── Утилиты ───────────────────────────────────────────────
    private static BoxView Deco(string hex, double opacity, double w, double h,
        LayoutOptions hOpt, LayoutOptions vOpt) => new BoxView
        {
            Color = Color.FromArgb(hex),
            Opacity = opacity,
            WidthRequest = w,
            HeightRequest = h,
            HorizontalOptions = hOpt,
            VerticalOptions = vOpt
        };
    private void BuildUI()
    {
        Shell.SetNavBarIsVisible(this, false);

        var decoTL = MakeDecoBox("#00F5FF", 0.12, 120, 120, LayoutOptions.Start, LayoutOptions.Start, Thickness.Zero);
        var decoBR = MakeDecoBox("#FF3CAC", 0.12, 100, 100, LayoutOptions.End, LayoutOptions.End, Thickness.Zero);
        var decoTR = MakeDecoBox("#FFE500", 0.08, 70, 70, LayoutOptions.End, LayoutOptions.Start, Thickness.Zero);
        var decoBL = MakeDecoBox("#39FF14", 0.08, 60, 60, LayoutOptions.Start, LayoutOptions.End, Thickness.Zero);

        LblTitle = new Label
        {
            Text = "BLOCK\nBLAST",
            FontFamily = "PressStart2P",
            FontSize = 28,
            HorizontalOptions = LayoutOptions.Center,
            HorizontalTextAlignment = TextAlignment.Center,
            LineHeight = 1.4
        };

        LblSubtitle = new Label
        {
            Text = "ENTER YOUR NAME",
            FontFamily = "PressStart2P",
            FontSize = 9,
            HorizontalOptions = LayoutOptions.Center,
            CharacterSpacing = 3
        };

        LblOneTimeHint = new Label
        {
            Text = "You only need to do this once.",
            FontFamily = "PressStart2P",
            FontSize = 12,
            HorizontalOptions = LayoutOptions.Center,
            HorizontalTextAlignment = TextAlignment.Center,
            CharacterSpacing = 1
        };

        EntryName = new Entry
        {
            Placeholder = "Your name...",
            MaxLength = 16,
            FontFamily = "PressStart2P",
            FontSize = 14,
            HorizontalTextAlignment = TextAlignment.Center
        };
        EntryName.Completed += (s, e) => OnConfirm();
        EntryName.TextChanged += OnTextChanged;

        LblHint = new Label
        {
            FontFamily = "PressStart2P",
            FontSize = 8,
            HorizontalOptions = LayoutOptions.Center,
            HorizontalTextAlignment = TextAlignment.Center,
            IsVisible = false
        };

        BtnConfirm = new Button
        {
            Text = "▶  START",
            FontFamily = "PressStart2P",
            FontSize = 11,
            CornerRadius = 0,
            HeightRequest = 52,
            BorderWidth = 0
        };
        BtnConfirm.Clicked += (s, e) => OnConfirm();

        BtnCancel = new Button
        {
            Text = AppResources.back,
            FontFamily = "PressStart2P",
            FontSize = 9,
            CornerRadius = 0,
            HeightRequest = 44,
            BackgroundColor = Colors.Transparent,
            BorderWidth = 2,
            IsVisible = _canCancel
        };
        BtnCancel.Clicked += (s, e) => OnCancel();

        _card = new Border
        {
            StrokeThickness = 2,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.Rectangle(),
            Padding = new Thickness(20, 24),
            Content = new VerticalStackLayout
            {
                Spacing = 14,
                Children = { LblSubtitle, LblOneTimeHint, EntryName, LblHint, BtnConfirm, BtnCancel }
            }
        };

        var centerStack = new VerticalStackLayout
        {
            Spacing = 32,
            VerticalOptions = LayoutOptions.Center,
            Padding = new Thickness(28, 0),
            Children = { LblTitle, _card }
        };

        _rootGrid = new Grid();
        _rootGrid.Children.Add(decoTL);
        _rootGrid.Children.Add(decoBR);
        _rootGrid.Children.Add(decoTR);
        _rootGrid.Children.Add(decoBL);
        _rootGrid.Children.Add(new ScrollView { Content = centerStack });
        Content = _rootGrid;
    }

    private void ApplyTheme()
    {
        var t = _themeService.Current;
        bool isLight = t.Name == "Light";

        // Фон страницы
        BackgroundColor = t.BackgroundColor;

        // Декор-боксы — скрываем на светлой теме
        foreach (var child in _rootGrid.Children)
        {
            if (child is BoxView deco)
                deco.Opacity = isLight ? 0.0 : 0.10;
        }

        // Заголовок BLOCK BLAST
        var titleColor = isLight
            ? Color.FromArgb("#005588")
            : Color.FromArgb("#00F5FF");

        LblTitle.TextColor = titleColor;
        LblTitle.Shadow = new Shadow
        {
            Brush = new SolidColorBrush(titleColor),
            Offset = new Point(4, 4),
            Radius = 0,
            Opacity = isLight ? 0.25f : 0.60f
        };

        // "ENTER YOUR NAME"
        LblSubtitle.TextColor = isLight
            ? Color.FromArgb("#1A1A2E")
            : Color.FromArgb("#FFE500");

        // "You only need to do this once."
        LblOneTimeHint.TextColor = isLight
            ? Color.FromArgb("#444466")
            : Color.FromArgb("#888899");

        // Поле ввода
        EntryName.BackgroundColor = isLight
            ? Color.FromArgb("#E0E0F0")
            : Color.FromArgb("#0D0D1A");
        EntryName.TextColor = isLight
            ? Color.FromArgb("#0D0D1A")
            : Colors.White;
        EntryName.PlaceholderColor = isLight
            ? Color.FromArgb("#9999BB")
            : Color.FromArgb("#444466");

        // Кнопка START
        var accentColor = isLight
            ? Color.FromArgb("#0077BB")
            : Color.FromArgb("#00F5FF");
        BtnConfirm.BackgroundColor = accentColor;
        BtnConfirm.TextColor = isLight
            ? Colors.White
            : Color.FromArgb("#0D0D1A");

        // Кнопка BACK
        var cancelColor = isLight
            ? Color.FromArgb("#CC0066")
            : Color.FromArgb("#FF3CAC");
        BtnCancel.TextColor = cancelColor;
        BtnCancel.BorderColor = cancelColor;

        // Карточка
        _card.BackgroundColor = isLight
            ? Color.FromArgb("#E8E8F8")
            : Color.FromArgb("#1A1A2E");
        _card.Stroke = new SolidColorBrush(isLight
            ? Color.FromArgb("#0077BB")
            : Color.FromArgb("#00F5FF"));
    }

    private static bool IsLightColor(Color c) =>
        (c.Red * 0.299 + c.Green * 0.587 + c.Blue * 0.114) > 0.6f;

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
}