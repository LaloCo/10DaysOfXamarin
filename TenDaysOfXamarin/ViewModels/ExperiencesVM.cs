using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using TenDaysOfXamarin.Model;
using TenDaysOfXamarin.ViewModels.Commands;

namespace TenDaysOfXamarin.ViewModels
{
    public class ExperiencesVM : INotifyPropertyChanged
    {
        // added using TenDaysOfXamarin.ViewModels.Commands;
        public NewExperienceCommand NewExperienceCommand { get; set; }

        public ObservableCollection<Experience> Experiences { get; set; }

        public ExperiencesVM()
        {
            NewExperienceCommand = new NewExperienceCommand(this);
            Experiences = new ObservableCollection<Experience>();
            ReadExperiences();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NewExperience()
        {
            App.Current.MainPage.Navigation.PushAsync(new MainPage());
        }

        public async void ReadExperiences()
        {
            // added using TenDaysOfXamarin.Model;
            var experiences = await Experience.GetExperiences();

            Experiences.Clear();
            foreach(var experience in experiences)
            {
                Experiences.Add(experience);
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
