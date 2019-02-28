using System;
using System.ComponentModel;

namespace TenDaysOfXamarin.ViewModels
{
    public class ExperiencesVM : INotifyPropertyChanged
    {
        public ExperiencesVM()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
