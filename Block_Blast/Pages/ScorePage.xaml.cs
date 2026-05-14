using Block_Blast.Resources.Localization;
using Block_Blast.Services;

namespace Block_Blast.Pages;

public partial class ScorePage : ContentPage
{
    // ══════════════════════════════════════════════════════════
    // Сервисы
    // ══════════════════════════════════════════════════════════

    private readonly ThemeService _themeService;
    
    private readonly ScoreService _scoreService;

    // ══════════════════════════════════════════════════════════
    // UI элементы
    // ══════════════════════════════════════════════════════════

    private Label LblTitle;

    private Grid GridHeader;
    private Label LblRankHeader;
    private Label LblNameHeader;
    private Label LblScoreHeader;

    private ScrollView ScoreScroll;
    private VerticalStackLayout ScoreList;

    private Label LblNoScores;

    private Button BtnBack;

    // ══════════════════════════════════════════════════════════
    // Состояние
    // ══════════════════════════════════════════════════════════

    private bool _isNavigating = false;

    // ══════════════════════════════════════════════════════════
    // Конструктор
    // ══════════════════════════════════════════════════════════

    public ScorePage(
        ThemeService themeService,
        
        ScoreService scoreService)
    {
        _themeService = themeService;
        
        _scoreService = scoreService;

        BuildUI();

        PrepareForEntrance();
    }

    // ══════════════════════════════════════════════════════════
    // UI
    // ══════════════════════════════════════════════════════════

    private void BuildUI()
    {
        BackgroundColor = Colors.Black;

        LblTitle = new Label
        {
            Text = AppResources.scores,
            FontSize = 30,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.White,
            HorizontalOptions = LayoutOptions.Center
        };

        LblRankHeader = new Label
        {
            Text = AppResources.rank,
            FontSize = 14,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.Gold,
            HorizontalOptions = LayoutOptions.Start
        };

        LblNameHeader = new Label
        {
            Text = AppResources.name,
            FontSize = 14,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.Gold,
            HorizontalOptions = LayoutOptions.Start
        };

        LblScoreHeader = new Label
        {
            Text = AppResources.score,
            FontSize = 14,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.Gold,
            HorizontalOptions = LayoutOptions.End
        };

        GridHeader = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = 60 },
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = 100 }
            },
            Padding = new Thickness(10, 8),
            BackgroundColor = Color.FromArgb("#222222")
        };

        GridHeader.Add(LblRankHeader, 0, 0);
        GridHeader.Add(LblNameHeader, 1, 0);
        GridHeader.Add(LblScoreHeader, 2, 0);

        ScoreList = new VerticalStackLayout
        {
            Spacing = 0
        };

        ScoreScroll = new ScrollView
        {
            HeightRequest = 450,
            Content = ScoreList
        };

        LblNoScores = new Label
        {
            Text = AppResources.no_scores_yet,
            FontSize = 18,
            TextColor = Colors.Gray,
            HorizontalOptions = LayoutOptions.Center,
            IsVisible = false
        };

        BtnBack = new Button
        {
            Text = AppResources.back,
            FontSize = 20,
            CornerRadius = 16,
            HeightRequest = 55,
            BackgroundColor = Colors.DodgerBlue,
            TextColor = Colors.White
        };

        BtnBack.Clicked += OnBackClicked;

        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Padding = new Thickness(25, 60, 25, 30),
                Spacing = 20,
                Children =
                {
                    LblTitle,
                    GridHeader,
                    LblNoScores,
                    ScoreScroll,
                    BtnBack
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
        ApplyLocalization();
        LoadScores();

        await PlayEntranceAnimation();
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        LanguageService.LanguageChanged -= ApplyLocalization; // важно — отписаться!
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
        LblTitle.Text = AppResources.scores;
        LblRankHeader.Text = AppResources.rank;
        LblNameHeader.Text = AppResources.name;
        LblScoreHeader.Text = AppResources.score;
        LblNoScores.Text = AppResources.no_scores_yet;
        BtnBack.Text = AppResources.back;
    }

    // ══════════════════════════════════════════════════════════
    // Анимации
    // ══════════════════════════════════════════════════════════

    private void PrepareForEntrance()
    {
        LblTitle.Opacity = 0;
        LblTitle.TranslationY = -20;

        GridHeader.Opacity = 0;
        ScoreScroll.Opacity = 0;
        LblNoScores.Opacity = 0;

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
            GridHeader.FadeToAsync(1, 250, Easing.CubicOut),
            ScoreScroll.FadeToAsync(1, 250, Easing.CubicOut),
            LblNoScores.FadeToAsync(1, 250, Easing.CubicOut)
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
    // Загрузка результатов
    // ══════════════════════════════════════════════════════════

    private void LoadScores()
    {
        ScoreList.Children.Clear();

        var scores = _scoreService.GetTopScores();

        if (scores.Count == 0)
        {
            LblNoScores.IsVisible = true;
            GridHeader.IsVisible = false;
            ScoreScroll.IsVisible = false;
            return;
        }

        LblNoScores.IsVisible = false;
        GridHeader.IsVisible = true;
        ScoreScroll.IsVisible = true;

        var theme = _themeService.Current;

        for (int i = 0; i < scores.Count; i++)
        {
            var entry = scores[i];
            int rank = i + 1;

            Color rowBg = rank switch
            {
                1 => Color.FromArgb("#FFD70033"),
                2 => Color.FromArgb("#C0C0C033"),
                3 => Color.FromArgb("#CD7F3233"),
                _ => Colors.Transparent
            };

            var row = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = 60 },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = 100 }
                },
                BackgroundColor = rowBg,
                Padding = new Thickness(10, 8)
            };

            var lblRank = new Label
            {
                Text = $"#{rank}",
                FontSize = 14,
                TextColor = theme.TextColor
            };

            var lblName = new Label
            {
                Text = entry.Name,
                FontSize = 14,
                TextColor = theme.TextColor
            };

            var lblScore = new Label
            {
                Text = entry.Score.ToString("N0"),
                FontSize = 14,
                FontAttributes = FontAttributes.Bold,
                TextColor = theme.AccentColor,
                HorizontalOptions = LayoutOptions.End
            };

            row.Add(lblRank, 0, 0);
            row.Add(lblName, 1, 0);
            row.Add(lblScore, 2, 0);

            ScoreList.Children.Add(row);

            ScoreList.Children.Add(new BoxView
            {
                HeightRequest = 1,
                BackgroundColor = theme.CellBorderColor,
                Opacity = 0.3
            });
        }
    }

    // ══════════════════════════════════════════════════════════
    // Назад
    // ══════════════════════════════════════════════════════════

    private async void OnBackClicked(object sender, EventArgs e)
    {
        if (_isNavigating)
            return;

        _isNavigating = true;

        await AnimateButtonPress(BtnBack);

        await Navigation.PopAsync();
    }
}