using System;
using System.ComponentModel;
using TenDaysOfXamarin.ViewModels.Commands;

namespace TenDaysOfXamarin.ViewModels
{
    public class ExperiencesVM : INotifyPropertyChanged
    {
        // added using TenDaysOfXamarin.ViewModels.Commands;
        public NewExperienceCommand NewExperienceCommand { get; set; }

        public ExperiencesVM()
        {
            NewExperienceCommand = new NewExperienceCommand(this);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NewExperience()
        {
            App.Current.MainPage.Navigation.PushAsync(new MainPage());
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
