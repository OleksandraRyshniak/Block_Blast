using Block_Blast.Models;
using Block_Blast.Services;
using Block_Blast.Resources.Localization;

namespace Block_Blast.Pages;

public partial class StartPage : ContentPage
{
    // ── Сервисы ───────────────────────────────────────────────
    private readonly ThemeService _themeService;
    private readonly ScoreService _scoreService;
    private readonly AccountService _accountService;

    // ── UI ────────────────────────────────────────────────────
    private Label LblTitle;
    private Label LblWelcome;       // "Привет, Имя!"
    private Label LblBest;          // "Рекорд: 1230"

    private Button BtnPlayEasy;
    private Button BtnPlayHard;
    private Button BtnScores;
    private Button BtnSettings;

    // ── Состояние ─────────────────────────────────────────────
    private bool _isNavigating = false;

    // ── Конструктор ───────────────────────────────────────────
    public StartPage(
        ThemeService themeService,
        ScoreService scoreService,
        AccountService accountService)
    {
        _themeService = themeService;
        _scoreService = scoreService;
        _accountService = accountService;

        BuildUI();
        PrepareForEntrance();
    }

    // ── Построение UI ─────────────────────────────────────────
    private void BuildUI()
    {
        BackgroundColor = Colors.Black;

        LblTitle = new Label
        {
            Text = "BLOCK BLAST",
            FontSize = 34,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.White,
            HorizontalOptions = LayoutOptions.Center
        };

        LblWelcome = new Label
        {
            FontSize = 20,
            TextColor = Colors.LightGray,
            HorizontalOptions = LayoutOptions.Center
        };

        LblBest = new Label
        {
            FontSize = 16,
            TextColor = Colors.Gold,
            HorizontalOptions = LayoutOptions.Center
        };

        // ── Две кнопки режима ─────────────────────────────────
        BtnPlayEasy = new Button
        {
            FontSize = 20,
            CornerRadius = 18,
            BackgroundColor = Colors.LimeGreen,
            TextColor = Colors.White,
            HeightRequest = 60
        };
        BtnPlayEasy.Clicked += (s, e) => OnPlayClicked(GameMode.Easy);

        BtnPlayHard = new Button
        {
            FontSize = 20,
            CornerRadius = 18,
            BackgroundColor = Color.FromArgb("#E53935"),
            TextColor = Colors.White,
            HeightRequest = 60
        };
        BtnPlayHard.Clicked += (s, e) => OnPlayClicked(GameMode.Hard);

        BtnScores = new Button
        {
            FontSize = 18,
            CornerRadius = 14,
            BackgroundColor = Colors.DodgerBlue,
            TextColor = Colors.White,
            HeightRequest = 55
        };
        BtnScores.Clicked += OnScoresClicked;

        BtnSettings = new Button
        {
            FontSize = 18,
            CornerRadius = 14,
            BackgroundColor = Colors.DarkOrange,
            TextColor = Colors.White,
            HeightRequest = 55
        };
        BtnSettings.Clicked += OnSettingsClicked;

        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Padding = new Thickness(30, 80, 30, 40),
                Spacing = 18,
                VerticalOptions = LayoutOptions.Center,
                Children =
                {
                    LblTitle,
                    new BoxView { HeightRequest = 10, Opacity = 0 },
                    LblWelcome,
                    LblBest,
                    new BoxView { HeightRequest = 10, Opacity = 0 },
                    BtnPlayEasy,
                    BtnPlayHard,
                    BtnScores,
                    BtnSettings
                }
            }
        };
    }

    // ── Жизненный цикл ────────────────────────────────────────
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        LanguageService.LanguageChanged += ApplyLocalization;

        _isNavigating = false;
        ApplyTheme();
        ApplyLocalization();

        // Имя показываем если аккаунт есть, иначе прячем (покажем при нажатии Play)
        RefreshPlayerInfo();

        await PlayEntranceAnimation();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        LanguageService.LanguageChanged -= ApplyLocalization;
    }

    // ── Аккаунт ───────────────────────────────────────────────

    /// <summary>
    /// Показывает диалог ввода имени.
    /// canCancel = false при первом запуске (нельзя закрыть без имени).
    /// </summary>
    public async Task ShowLoginDialog(bool canCancel = true)
    {
        while (true)
        {
            string prompt = canCancel
                ? AppResources.enter_your_name
                : AppResources.enter_your_name; // можно сделать отдельный ключ

            string result = await DisplayPromptAsync(
                title: "Block Blast",
                message: AppResources.player,
                accept: "OK",
                cancel: canCancel ? AppResources.back : null,
                placeholder: AppResources.enter_your_name,
                maxLength: 16,
                keyboard: Keyboard.Text);

            // Отменил (только если разрешено)
            if (result == null && canCancel) return;

            string cleaned = CleanName(result ?? "");

            if (string.IsNullOrEmpty(cleaned))
            {
                await DisplayAlert(
                    AppResources.no_name,
                    AppResources.name_start,
                    "OK");
                continue;
            }

            _accountService.SetCurrentName(cleaned);
            RefreshPlayerInfo();
            return;
        }
    }

    private void RefreshPlayerInfo()
    {
        var name = _accountService.GetCurrentName();

        if (name == null)
        {
            LblWelcome.Text = "👤 ...";
            LblBest.Text = "";
            return;
        }

        LblWelcome.Text = $"👤 {name}";

        int best = _scoreService.GetBestScore(name);
        LblBest.Text = best > 0
            ? $"🏆 {AppResources.best}: {best:N0}"
            : $"🏆 {AppResources.best}: —";
    }

    // ── Тема и локализация ────────────────────────────────────
    private void ApplyTheme() => _themeService.Current.Apply(this);

    private void ApplyLocalization()
    {
        BtnPlayEasy.Text = "▶  " + AppResources.play + " (Easy)";
        BtnPlayHard.Text = "🔥  " + AppResources.play + " (Hard)";
        BtnScores.Text = "🏆  " + AppResources.scores;
        BtnSettings.Text = "⚙  " + AppResources.settings;
    }

    // ── Анимации ──────────────────────────────────────────────
    private void PrepareForEntrance()
    {
        LblTitle.Opacity = 0;
        LblTitle.TranslationY = -30;
        LblWelcome.Opacity = 0;
        LblBest.Opacity = 0;
        BtnPlayEasy.Opacity = 0;
        BtnPlayEasy.TranslationY = 20;
        BtnPlayHard.Opacity = 0;
        BtnPlayHard.TranslationY = 20;
        BtnScores.Opacity = 0;
        BtnSettings.Opacity = 0;
    }

    private async Task PlayEntranceAnimation()
    {
        await Task.WhenAll(
            LblTitle.FadeTo(1, 350, Easing.CubicOut),
            LblTitle.TranslateTo(0, 0, 350, Easing.CubicOut));

        await Task.Delay(60);

        await Task.WhenAll(
            LblWelcome.FadeTo(1, 250, Easing.CubicOut),
            LblBest.FadeTo(1, 250, Easing.CubicOut));

        await Task.Delay(60);

        await Task.WhenAll(
            BtnPlayEasy.FadeTo(1, 220, Easing.CubicOut),
            BtnPlayEasy.TranslateTo(0, 0, 220, Easing.CubicOut),
            BtnPlayHard.FadeTo(1, 220, Easing.CubicOut),
            BtnPlayHard.TranslateTo(0, 0, 220, Easing.CubicOut));

        await Task.Delay(60);

        await Task.WhenAll(
            BtnScores.FadeTo(1, 200, Easing.CubicOut),
            BtnSettings.FadeTo(1, 200, Easing.CubicOut));
    }

    private static async Task AnimateButtonPress(View btn)
    {
        await btn.ScaleTo(0.93, 80, Easing.CubicIn);
        await btn.ScaleTo(1.0, 100, Easing.CubicOut);
    }

    private async Task AnimateShake(View view)
    {
        for (int i = 0; i < 3; i++)
        {
            await view.TranslateTo(-8, 0, 55);
            await view.TranslateTo(8, 0, 55);
        }
        await view.TranslateTo(0, 0, 55);
    }

    // ── Кнопки ────────────────────────────────────────────────
    private async void OnPlayClicked(GameMode mode)
    {
        if (_isNavigating) return;

        var btn = mode == GameMode.Easy ? BtnPlayEasy : BtnPlayHard;
        await AnimateButtonPress(btn);

        var name = _accountService.GetCurrentName();
        if (name == null)
        {
            await ShowLoginDialog(canCancel: false);
            name = _accountService.GetCurrentName();
            if (name == null) return;
        }

        _isNavigating = true;

        try
        {
            int best = _scoreService.GetBestScore(name);
            var player = new Player(name, best);

            await Navigation.PushAsync(
                new GamePage(player, mode, _themeService, _scoreService));
        }
        catch (Exception ex)
        {
            _isNavigating = false;
            await DisplayAlert(AppResources.error, ex.Message, "OK");
        }
    }

    private async void OnScoresClicked(object sender, EventArgs e)
    {
        if (_isNavigating) return;
        await AnimateButtonPress(BtnScores);
        _isNavigating = true;
        await Navigation.PushAsync(new ScorePage(_themeService, _scoreService));
    }

    private async void OnSettingsClicked(object sender, EventArgs e)
    {
        if (_isNavigating) return;
        await AnimateButtonPress(BtnSettings);
        _isNavigating = true;
        await Navigation.PushAsync(
            new SettingsPage(_themeService, _accountService, this));
    }

    // ── Утилиты ───────────────────────────────────────────────
    private static string CleanName(string raw)
    {
        string filtered = new string(
            raw.Where(c => char.IsLetterOrDigit(c) || c == ' ' || c == '-' || c == '_')
               .ToArray());

        while (filtered.Contains("  "))
            filtered = filtered.Replace("  ", " ");

        filtered = filtered.Trim();

        return filtered.Length > 16 ? filtered[..16] : filtered;
    }
}