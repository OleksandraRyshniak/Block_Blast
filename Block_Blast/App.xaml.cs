using Block_Blast.Resources.Localization;
using Block_Blast.Pages;

namespace Block_Blast
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Регистрируем AppResources в словаре
            Resources["AppStrings"] = new AppResources();

            LanguageService.LoadSaved();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}