using Block_Blast.Pages;
using Block_Blast.Services;
using Microsoft.Extensions.Logging;

namespace Block_Blast
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton<AccountService>();
            builder.Services.AddSingleton<ThemeService>();
            builder.Services.AddSingleton<ScoreService>();
            

 

            builder.Services.AddTransient<StartPage>();
            builder.Services.AddTransient<GamePage>();

            builder.Services.AddTransient<SettingsPage>();


#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
