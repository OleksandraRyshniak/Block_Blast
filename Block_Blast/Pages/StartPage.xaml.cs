using Block_Blast.Models;
using Block_Blast.Services;
using Block_Blast.ViewModel;
using Block_Blast.Resources.Localization;

namespace Block_Blast.Pages;

public partial class StartPage : ContentPage
{
    private readonly ThemeService _themeService;
   // private readonly LanguageService _localization;
    private readonly ScoreService _scoreService;

    // ══════════════════════════════════════════════════════════
    // UI элементы (создаются полностью в C# без XAML)
    // ══════════════════════════════════════════════════════════

    private Label LblTitle;
    private Label LblPlayerName;
    private Entry EntryName;

    private Button BtnPlay;
    private Button BtnScores;
    private Button BtnSettings;

    // ══════════════════════════════════════════════════════════
    // Состояние
    // ══════════════════════════════════════════════════════════

    private bool _isNavigating = false;

    // ══════════════════════════════════════════════════════════
    // Конструктор
    // ══════════════════════════════════════════════════════════

    public StartPage(
        ThemeService themeService,
        //LanguageService localization,
        ScoreService scoreService)
    {
        _themeService = themeService;
       // _localization = localization;
        _scoreService = scoreService;

        BuildUI();

        PrepareForEntrance();
    }

    // ══════════════════════════════════════════════════════════
    // Построение интерфейса
    // ══════════════════════════════════════════════════════════

    private void BuildUI()
    {
        BackgroundColor = Colors.Black;

        // Заголовок
        LblTitle = new Label
        {
            Text = "BLOCK BLAST",
            FontSize = 34,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.White,
            HorizontalOptions = LayoutOptions.Center
        };

        // Подпись игрока
        LblPlayerName = new Label
        {
            Text = AppResources.player,
            FontSize = 18,
            TextColor = Colors.White,
            HorizontalOptions = LayoutOptions.Start
        };

        // Поле ввода
        EntryName = new Entry
        {
            Placeholder = AppResources.enter_your_name,
            MaxLength = 16,
            BackgroundColor = Colors.White,
            TextColor = Colors.Black,
            HorizontalOptions = LayoutOptions.Fill,
            ReturnType = ReturnType.Done
        };

        EntryName.Completed += OnEntryNameCompleted;
        EntryName.TextChanged += OnEntryNameTextChanged;

        // Кнопка PLAY
        BtnPlay = new Button
        {
            Text = AppResources.play,
            FontSize = 22,
            CornerRadius = 18,
            BackgroundColor = Colors.LimeGreen,
            TextColor = Colors.White,
            HeightRequest = 60
        };

        BtnPlay.Clicked += OnPlayClicked;

        // Кнопка SCORES
        BtnScores = new Button
        {
            Text = "🏆" + AppResources.scores,
            FontSize = 18,
            CornerRadius = 14,
            BackgroundColor = Colors.DodgerBlue,
            TextColor = Colors.White,
            HeightRequest = 55
        };

        BtnScores.Clicked += OnScoresClicked;

        // Кнопка SETTINGS
        BtnSettings = new Button
        {
            Text = "⚙" + AppResources.settings,
            FontSize = 18,
            CornerRadius = 14,
            BackgroundColor = Colors.DarkOrange,
            TextColor = Colors.White,
            HeightRequest = 55
        };

        BtnSettings.Clicked += OnSettingsClicked;

        // Основной контейнер
        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Padding = new Thickness(30, 80, 30, 40),
                Spacing = 22,
                VerticalOptions = LayoutOptions.Center,

                Children =
                {
                    LblTitle,

                    new BoxView
                    {
                        HeightRequest = 20,
                        Opacity = 0
                    },

                    LblPlayerName,
                    EntryName,

                    new BoxView
                    {
                        HeightRequest = 15,
                        Opacity = 0
                    },

                    BtnPlay,
                    BtnScores,
                    BtnSettings
                }
            }
        };
    }

    // ══════════════════════════════════════════════════════════
    // Жизненный цикл
    // ══════════════════════════════════════════════════════════

    protected override async void OnAppearing()
    {

        base.OnAppearing();
        LanguageService.LanguageChanged += ApplyLocalization;
        ApplyLocalization();

        _isNavigating = false;

        ApplyTheme();
        //ApplyLocalization();

        await PlayEntranceAnimation();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        LanguageService.LanguageChanged -= ApplyLocalization; // важно — отписаться!
        EntryName.Unfocus();
    }

    // ══════════════════════════════════════════════════════════
    // Тема и локализация
    // ══════════════════════════════════════════════════════════

    private void ApplyTheme()
    {
        _themeService.Current.Apply(this);
    }

    private void ApplyLocalization()
    {
        LblTitle.Text = "BLOCK BLAST";
        LblPlayerName.Text = AppResources.player;
        EntryName.Placeholder = AppResources.enter_your_name;
        BtnPlay.Text = "▶  " + AppResources.play;
        BtnScores.Text = "🏆  " +AppResources.scores;
        BtnSettings.Text = "⚙  " + AppResources.settings;
    }

    // ══════════════════════════════════════════════════════════
    // Анимации
    // ══════════════════════════════════════════════════════════

    private void PrepareForEntrance()
    {
        LblTitle.Opacity = 0;
        LblTitle.TranslationY = -30;

        LblPlayerName.Opacity = 0;
        EntryName.Opacity = 0;

        BtnPlay.Opacity = 0;
        BtnPlay.TranslationY = 20;

        BtnScores.Opacity = 0;
        BtnSettings.Opacity = 0;
    }

    private async Task PlayEntranceAnimation()
    {
        await Task.WhenAll(
            LblTitle.FadeTo(1, 350, Easing.CubicOut),
            LblTitle.TranslateTo(0, 0, 350, Easing.CubicOut)
        );

        await Task.Delay(80);

        await Task.WhenAll(
            LblPlayerName.FadeTo(1, 250, Easing.CubicOut),
            EntryName.FadeTo(1, 250, Easing.CubicOut)
        );

        await Task.Delay(60);

        await Task.WhenAll(
            BtnPlay.FadeTo(1, 250, Easing.CubicOut),
            BtnPlay.TranslateTo(0, 0, 250, Easing.CubicOut)
        );

        await Task.Delay(60);

        await Task.WhenAll(
            BtnScores.FadeTo(1, 200, Easing.CubicOut),
            BtnSettings.FadeTo(1, 200, Easing.CubicOut)
        );
    }

    private static async Task AnimateButtonPress(View button)
    {
        await button.ScaleTo(0.93, 80, Easing.CubicIn);
        await button.ScaleTo(1.0, 100, Easing.CubicOut);
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

    // ══════════════════════════════════════════════════════════
    // Валидация
    // ══════════════════════════════════════════════════════════

    private string? GetValidatedName()
    {
        string raw = EntryName.Text?.Trim() ?? string.Empty;

        while (raw.Contains("  "))
            raw = raw.Replace("  ", " ");

        return string.IsNullOrEmpty(raw) ? null : raw;
    }

    // ══════════════════════════════════════════════════════════
    // Кнопки
    // ══════════════════════════════════════════════════════════

    private async void OnPlayClicked(object sender, EventArgs e)
    {
        if (_isNavigating) return;

        await AnimateButtonPress(BtnPlay);

        string? name = GetValidatedName();

        if (name is null)
        {
            Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(80));
            await AnimateShake(EntryName);

            await DisplayAlert(AppResources.no_name, AppResources.name_start, "OK");
            return;
        }

        _isNavigating = true;

        try
        {
            int best = _scoreService.GetBestScore(name);
            var player = new Player(name, best);

            await Navigation.PushAsync(
                new GamePage(player, _themeService, //_localization,
                                                    _scoreService));
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

        await Navigation.PushAsync(
            new ScorePage(_themeService,  _scoreService));
    }

    private async void OnSettingsClicked(object sender, EventArgs e)
    {
        if (_isNavigating) return;

        await AnimateButtonPress(BtnSettings);

        _isNavigating = true;

        await Navigation.PushAsync(
            new SettingsPage(_themeService));
    }

    // ══════════════════════════════════════════════════════════
    // Ввод
    // ══════════════════════════════════════════════════════════

    private void OnEntryNameCompleted(object sender, EventArgs e)
    {
        OnPlayClicked(BtnPlay, EventArgs.Empty);
    }

    private void OnEntryNameTextChanged(object sender, TextChangedEventArgs e)
    {
        if (e.NewTextValue is null) return;

        string filtered = new string(
            e.NewTextValue
                .Where(c => char.IsLetterOrDigit(c) || c == ' ' || c == '-' || c == '_')
                .ToArray()
        );

        if (filtered.Length > 16)
            filtered = filtered[..16];

        if (filtered != e.NewTextValue)
            EntryName.Text = filtered;
    }
}