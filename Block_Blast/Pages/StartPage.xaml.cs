using Block_Blast.Models;
using Block_Blast.Services;
using Block_Blast.Resources.Localization;

namespace Block_Blast.Pages;

public partial class StartPage : ContentPage
{
  
    private readonly ThemeService _themeService;
    private readonly ScoreService _scoreService;
    private readonly AccountService _accountService;

    private Label LblTitle;
    private Label LblBest;       

    private Button BtnPlayEasy;
    private Button BtnPlayHard;

    private Button BtnSettings;

  
    private bool _isNavigating = false;
    private bool _loginChecked = false;

   
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

    private void BuildUI()
    {
        BackgroundColor = Color.FromArgb("#0D0D1A");
        Shell.SetNavBarIsVisible(this, false);

        // Декор — те же 4 угла
        var decoTL = MakeDecoBox("#39FF14", 0.10, 100, 100, LayoutOptions.Start, LayoutOptions.Start, Thickness.Zero);
        var decoBR = MakeDecoBox("#FF3CAC", 0.12, 110, 110, LayoutOptions.End, LayoutOptions.End, Thickness.Zero);
        var decoTR = MakeDecoBox("#FFE500", 0.09, 75, 75, LayoutOptions.End, LayoutOptions.Start, Thickness.Zero);
        var decoBL = MakeDecoBox("#00F5FF", 0.09, 65, 65, LayoutOptions.Start, LayoutOptions.End, Thickness.Zero);

        LblTitle = new Label
        {
            Text = "BLOCK\nBLAST",
            FontFamily = "PressStart2P",
            FontSize = 30,
            TextColor = Color.FromArgb("#00F5FF"),
            HorizontalOptions = LayoutOptions.Center,
            HorizontalTextAlignment = TextAlignment.Center,
            LineHeight = 1.4,
            Shadow = new Shadow
            {
                Brush = new SolidColorBrush(Color.FromArgb("#00F5FF")),
                Offset = new Point(4, 4),
                Radius = 0,
                Opacity = 0.65f
            }
        };

        var titleLine = new BoxView
        {
            Color = Color.FromArgb("#00F5FF"),
            HeightRequest = 2,
            Opacity = 0.35,
            Margin = new Thickness(0, 0, 0, 8)
        };

        LblBest = new Label
        {
            FontFamily = "PressStart2P",
            FontSize = 9,
            TextColor = Color.FromArgb("#FFE500"),
            HorizontalOptions = LayoutOptions.Center,
            CharacterSpacing = 2,
            Shadow = new Shadow
            {
                Brush = new SolidColorBrush(Color.FromArgb("#FFE500")),
                Offset = new Point(2, 2),
                Radius = 0,
                Opacity = 0.5f
            }
        };

        BtnPlayEasy = new Button
        {
            FontFamily = "PressStart2P",
            FontSize = 12,
            CornerRadius = 0,
            HeightRequest = 56,
            BackgroundColor = Color.FromArgb("#39FF14"),
            TextColor = Color.FromArgb("#0D0D1A"),
            BorderWidth = 0
        };
        BtnPlayEasy.Clicked += (s, e) => OnPlayClicked(GameMode.Easy);

        BtnPlayHard = new Button
        {
            FontFamily = "PressStart2P",
            FontSize = 12,
            CornerRadius = 0,
            HeightRequest = 56,
            BackgroundColor = Color.FromArgb("#FF3CAC"),
            TextColor = Color.FromArgb("#0D0D1A"),
            BorderWidth = 0
        };
        BtnPlayHard.Clicked += (s, e) => OnPlayClicked(GameMode.Hard);

        BtnSettings = new Button
        {
            FontFamily = "PressStart2P",
            FontSize = 10,
            CornerRadius = 0,
            HeightRequest = 48,
            BackgroundColor = Colors.Transparent,
            TextColor = Color.FromArgb("#FF3CAC"),
            BorderColor = Color.FromArgb("#FF3CAC"),
            BorderWidth = 2
        };
        BtnSettings.Clicked += OnSettingsClicked;

        var mainCard = new Border
        {
            BackgroundColor = Color.FromArgb("#1A1A2E"),
            StrokeThickness = 2,
            Stroke = new SolidColorBrush(Color.FromArgb("#39FF14")),
            StrokeShape = new Microsoft.Maui.Controls.Shapes.Rectangle(),
            Padding = new Thickness(20, 24),
            Content = new VerticalStackLayout
            {
                Spacing = 14,
                Children = { BtnPlayEasy, BtnPlayHard }
            }
        };

        Content = new Grid
        {
            Children =
        {
            decoTL, decoBR, decoTR, decoBL,
            new ScrollView
            {
                Content = new VerticalStackLayout
                {
                    Padding = new Thickness(28, 72, 28, 40),
                    Spacing = 20,
                    VerticalOptions = LayoutOptions.Center,
                    Children = { LblTitle, titleLine, LblBest, mainCard, BtnSettings }
                }
            }
        }
        };
    }

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


   
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        LanguageService.LanguageChanged += ApplyLocalization;

        _isNavigating = false;
        ApplyTheme();
        ApplyLocalization();
        RefreshBestScore();

        if (!_loginChecked)
        {
            _loginChecked = true;

            if (_accountService.GetCurrentName() == null)
                await GoToLogin(canCancel: false);
        }

        await PlayEntranceAnimation();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        LanguageService.LanguageChanged -= ApplyLocalization;
    }

   
    private void RefreshBestScore()
    {
        var playerName = _accountService.GetCurrentName();
        if (playerName == null) return;

        int best = _scoreService.GetBestScore(playerName);
        LblBest.Text = best > 0
            ? $"🏆: {best:N0}"
            : $"🏆: —";
    }

    
    public async Task<string?> GoToLogin(bool canCancel = false)
    {
        var loginPage = new LoginPage(
            _accountService, _scoreService, _themeService, canCancel);

        await Navigation.PushAsync(loginPage);
        var result = await loginPage.WaitForResult();

        RefreshBestScore();
        return result;
    }


    private void ApplyTheme()
    {
        var t = _themeService.Current;

        BackgroundColor = t.BackgroundColor;

        LblTitle.TextColor = t.AccentColor;
        LblBest.TextColor = t.Name == AppResources.light
            ? Color.FromArgb("#B8860B")   
            : Colors.Gold;

        
        BtnPlayEasy.BackgroundColor = t.AccentColor;
        BtnPlayEasy.TextColor = IsLightColor(t.AccentColor)
            ? Color.FromArgb("#0A0A14") : Colors.White;

       
        var hardColor = t.Name switch
        {
            "Light" => Color.FromArgb("#E53935"),
            "Dark" => Color.FromArgb("#FF6B6B"),
            "Colorful" => Color.FromArgb("#FF3CAC"),
            _ => Color.FromArgb("#E53935")
        };
        BtnPlayHard.BackgroundColor = hardColor;
        BtnPlayHard.TextColor = IsLightColor(hardColor)
            ? Color.FromArgb("#0A0A14") : Colors.White;

        BtnSettings.BackgroundColor = Colors.Transparent;
        BtnSettings.TextColor = t.TextColor;
        BtnSettings.BorderColor = t.CellBorderColor;
    }

    private static bool IsLightColor(Color c) =>
        (c.Red * 0.299 + c.Green * 0.587 + c.Blue * 0.114) > 0.6f;

    private void ApplyLocalization()
    {
        BtnPlayEasy.Text = AppResources.play + AppResources.easy;
        BtnPlayHard.Text = "🔥  " + AppResources.play + AppResources.hard;
        
        BtnSettings.Text = "⚙  " + AppResources.settings;
        RefreshBestScore();
    }

    // ── Анимации ──────────────────────────────────────────────
    private void PrepareForEntrance()
    {
        LblTitle.Opacity = 0;
        LblTitle.TranslationY = -30;
        LblBest.Opacity = 0;
        BtnPlayEasy.Opacity = 0;
        BtnPlayEasy.TranslationY = 20;
        BtnPlayHard.Opacity = 0;
        BtnPlayHard.TranslationY = 20;
        
        BtnSettings.Opacity = 0;
    }

    private async Task PlayEntranceAnimation()
    {
        await Task.WhenAll(
            LblTitle.FadeToAsync(1, 350, Easing.CubicOut),
            LblTitle.TranslateToAsync(0, 0, 350, Easing.CubicOut));

        await Task.Delay(60);

        await LblBest.FadeTo(1, 250, Easing.CubicOut);

        await Task.Delay(60);

        await Task.WhenAll(
            BtnPlayEasy.FadeToAsync(1, 220, Easing.CubicOut),
            BtnPlayEasy.TranslateToAsync(0, 0, 220, Easing.CubicOut),
            BtnPlayHard.FadeToAsync(1, 220, Easing.CubicOut),
            BtnPlayHard.TranslateToAsync(0, 0, 220, Easing.CubicOut));

        await Task.Delay(60);

        await Task.WhenAll(
            
            BtnSettings.FadeToAsync(1, 200, Easing.CubicOut));
    }

    private static async Task AnimateButtonPress(View btn)
    {
        await btn.ScaleToAsync(0.93, 80, Easing.CubicIn);
        await btn.ScaleToAsync(1.0, 100, Easing.CubicOut);
    }

   
    private async void OnPlayClicked(GameMode mode)
    {
        if (_isNavigating) return;

        var btn = mode == GameMode.Easy ? BtnPlayEasy : BtnPlayHard;
        await AnimateButtonPress(btn);

      
        var name = _accountService.GetCurrentName();
        if (name == null)
        {
            await GoToLogin(canCancel: false);
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
            await DisplayAlertAsync(AppResources.error, ex.Message, "OK");
        }
    }



    private async void OnSettingsClicked(object sender, EventArgs e)
    {
        if (_isNavigating) return;
        await AnimateButtonPress(BtnSettings);
        _isNavigating = true;
        await Navigation.PushAsync(
            new SettingsPage(_themeService, _accountService, this));
    }
}