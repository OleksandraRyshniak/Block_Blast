using Block_Blast.Resources.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;

namespace Block_Blast.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        //public string alertCp => AppResources.alertCp;


        public ICommand SetEnglishCommand { get; }
        public ICommand SetRussianCommand { get; }
        public ICommand SetEstonianCommand { get; }

        public MainViewModel()
        {
            SetEnglishCommand = new Command(() => ChangeLanguage("en"));
            SetRussianCommand = new Command(() => ChangeLanguage("ru"));
            SetEstonianCommand = new Command(() => ChangeLanguage("et"));

            LanguageService.LanguageChanged += OnLanguageChanged;
        }

        private void ChangeLanguage(string code)
        {
            LanguageService.ChangeLanguage(code);
        }

        private void OnLanguageChanged()
        {
            //OnPropertyChanged(nameof(Greeting));
            //OnPropertyChanged(nameof(EnglishButton));
            //OnPropertyChanged(nameof(EstonianButton));
            //OnPropertyChanged(nameof(Year));
            //OnPropertyChanged(nameof(descCsharp));
            //OnPropertyChanged(nameof(alertCsharp));
            //OnPropertyChanged(nameof(descPython));
            //OnPropertyChanged(nameof(alertPython));
            //OnPropertyChanged(nameof(descJS));
            //OnPropertyChanged(nameof(alertJS));
            //OnPropertyChanged(nameof(descJava));
            //OnPropertyChanged(nameof(alertJava));
            //OnPropertyChanged(nameof(descCp));
            //OnPropertyChanged(nameof(alertCp));

        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
