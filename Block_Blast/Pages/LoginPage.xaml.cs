using Block_Blast.Resources.Localization;
using Block_Blast.Services;

namespace Block_Blast.Pages;

/// <summary>
/// Экран входа / создания аккаунта.
/// Показывается при первом запуске и после logout.
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
    private Label LblHint;       // живая подсказка рекорда / ошибки
    private Button BtnConfirm;
    private Button BtnCancel;

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
        PrepareEntrance();
    }

    // ── Построение UI ─────────────────────────────────────────
    private void BuildUI()
    {
        BackgroundColor = Color.FromArgb("#0A0A14");

        var decoTL = Deco("#00F5FF", 0.12, 120, 120, LayoutOptions.Start, LayoutOptions.Start);
        var decoBR = Deco("#FF3CAC", 0.12, 100, 100, LayoutOptions.End, LayoutOptions.End);
        var decoTR = Deco("#FFE500", 0.08, 70, 70, LayoutOptions.End, LayoutOptions.Start);
        var decoBL = Deco("#39FF14", 0.08, 60, 60, LayoutOptions.Start, LayoutOptions.End);

        LblTitle = new Label
        {
            Text = "BLOCK\nBLAST",
            FontFamily = "PressStart2P",
            FontSize = 28,
            TextColor = Color.FromArgb("#00F5FF"),
            HorizontalOptions = LayoutOptions.Center,
            HorizontalTextAlignment = TextAlignment.Center,
            LineHeight = 1.4,
            Shadow = new Shadow
            {
                Brush = new SolidColorBrush(Color.FromArgb("#00F5FF")),
                Offset = new Point(4, 4),
                Radius = 0,
                Opacity = 0.6f
            }
        };

        LblSubtitle = new Label
        {
            Text = "ENTER YOUR NAME",
            FontFamily = "PressStart2P",
            FontSize = 9,
            TextColor = Color.FromArgb("#888888"),
            HorizontalOptions = LayoutOptions.Center,
            CharacterSpacing = 2
        };

        EntryName = new Entry
        {
            Placeholder = "Your name...",
            MaxLength = 16,
            FontSize = 18,
            HorizontalTextAlignment = TextAlignment.Center,
            BackgroundColor = Color.FromArgb("#1A1A2E"),
            TextColor = Colors.White,
            PlaceholderColor = Color.FromArgb("#555555"),
        };
        EntryName.Completed += (s, e) => OnConfirm();
        EntryName.TextChanged += OnTextChanged;

        // Живая подсказка: зелёная = нашли рекорд, красная = ошибка
        LblHint = new Label
        {
            FontSize = 11,
            HorizontalOptions = LayoutOptions.Center,
            HorizontalTextAlignment = TextAlignment.Center,
            IsVisible = false
        };

        var scoreHint = new Label
        {
            Text = "Returning player? Use your old name\nto load your record.",
            FontSize = 11,
            TextColor = Color.FromArgb("#444466"),
            HorizontalOptions = LayoutOptions.Center,
            HorizontalTextAlignment = TextAlignment.Center,
        };

        BtnConfirm = new Button
        {
            Text = "▶  START",
            FontFamily = "PressStart2P",
            FontSize = 12,
            CornerRadius = 0,
            HeightRequest = 54,
            BackgroundColor = Color.FromArgb("#00F5FF"),
            TextColor = Color.FromArgb("#0A0A14"),
            BorderWidth = 0
        };
        BtnConfirm.Clicked += (s, e) => OnConfirm();

        BtnCancel = new Button
        {
            Text = AppResources.back,
            FontFamily = "PressStart2P",
            FontSize = 10,
            CornerRadius = 0,
            HeightRequest = 44,
            BackgroundColor = Colors.Transparent,
            TextColor = Color.FromArgb("#888888"),
            BorderColor = Color.FromArgb("#333333"),
            BorderWidth = 1,
            IsVisible = _canCancel
        };
        BtnCancel.Clicked += (s, e) => OnCancel();

        var card = new Border
        {
            BackgroundColor = Color.FromArgb("#12122A"),
            StrokeThickness = 2,
            Stroke = new SolidColorBrush(Color.FromArgb("#00F5FF")),
            StrokeShape = new Microsoft.Maui.Controls.Shapes.Rectangle(),
            Padding = new Thickness(24, 28),
            Content = new VerticalStackLayout
            {
                Spacing = 16,
                Children = { LblSubtitle, EntryName, LblHint, scoreHint, BtnConfirm, BtnCancel }
            }
        };

        var centerStack = new VerticalStackLayout
        {
            Spacing = 32,
            VerticalOptions = LayoutOptions.Center,
            Padding = new Thickness(28, 0),
            Children = { LblTitle, card }
        };

        var root = new Grid();
        root.Children.Add(decoTL);
        root.Children.Add(decoBR);
        root.Children.Add(decoTR);
        root.Children.Add(decoBL);
        root.Children.Add(new ScrollView { Content = centerStack });
        Content = root;
    }

    // ── Lifecycle ─────────────────────────────────────────────
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await PlayEntrance();
        EntryName.Focus();
    }

    // Физическая кнопка «назад» — блокируем если нет аккаунта
    protected override bool OnBackButtonPressed()
    {
        if (_canCancel) OnCancel();
        return true;
    }

    // ── Логика ────────────────────────────────────────────────
    private void OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        LblHint.IsVisible = false;

        // Фильтрация символов
        string raw = e.NewTextValue ?? "";
        string filtered = new string(
            raw.Where(c => char.IsLetterOrDigit(c) || c == ' ' || c == '-' || c == '_')
               .ToArray());
        while (filtered.Contains("  "))
            filtered = filtered.Replace("  ", " ");

        if (filtered != raw)
            EntryName.Text = filtered;

        // Живая подсказка рекорда
        string cleaned = filtered.Trim();
        if (cleaned.Length >= 2)
        {
            int best = _scoreService.GetBestScore(cleaned);
            if (best > 0)
            {
                LblHint.Text = $"🏆 Welcome back! Record: {best:N0}";
                LblHint.TextColor = Color.FromArgb("#39FF14");
                LblHint.IsVisible = true;
            }
        }
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

    /// <summary>Ждёт пока пользователь не подтвердит или не отменит.</summary>
    public Task<string?> WaitForResult() => _tcs.Task;

    // ── Анимации ──────────────────────────────────────────────
    private void PrepareEntrance()
    {
        LblTitle.Opacity = 0;
        LblTitle.TranslationY = -30;
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
}