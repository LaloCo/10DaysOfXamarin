using System;
using System.ComponentModel;
using System.Windows.Input;
using SQLite;
using TenDaysOfXamarin.Model;
using Xamarin.Forms;

namespace TenDaysOfXamarin.ViewModels
{
    public class MainVM : INotifyPropertyChanged // added using System.ComponentModel;
    {
        private string title;
        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                OnPropertyChanged("Title");
                OnPropertyChanged("CanSave");
            }
        }

        private string query;
        public string Query
        {
            get { return query; }
            set
            {
                query = value;
                OnPropertyChanged("Query");
            }
        }

        private Venue selectedVenue; // added using TenDaysOfXamarin.Model;
        public Venue SelectedVenue
        {
            get { return selectedVenue; }
            set
            {
                selectedVenue = value;
                if (selectedVenue != null)
                {
                    ShowVenues = false;
                    Query = string.Empty;
                }

                OnPropertyChanged("SelectedVenue");
                OnPropertyChanged("ShowSelectedVenue");
            }
        }

        private string content;
        public string Content
        {
            get { return content; }
            set
            {
                content = value;
                OnPropertyChanged("Content");
                OnPropertyChanged("CanSave");
            }
        }

        public bool CanSave
        {
            get { return !string.IsNullOrWhiteSpace(Title) && !string.IsNullOrWhiteSpace(Content); }
        }

        public bool ShowSelectedVenue
        {
            get { return SelectedVenue != null; }
        }

        private bool showVenues;
        public bool ShowVenues
        {
            get { return showVenues; }
            set
            {
                showVenues = value;
                OnPropertyChanged("ShowVenues");
            }
        }

        // added using System.Windows.Input;
        public ICommand CancelCommand { get; set; }
        public ICommand SaveCommand { get; set; }

        public MainVM()
        {
            // added using Xamarin.Forms;
            CancelCommand = new Command(CancelAction);
            SaveCommand = new Command<bool>(SaveAction, CanExecuteSave);
        }

        bool CanExecuteSave(bool arg)
        {
            return arg;
        }

        void SaveAction(bool obj)
        {
            Experience newExperience = new Experience()
            {
                Title = Title,
                Content = Content,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                VenueName = SelectedVenue.name,
                VenueCategory = SelectedVenue.MainCategory,
                VenueLat = float.Parse(SelectedVenue.location.Coordinates.Split(',')[0]),
                VenueLng = float.Parse(SelectedVenue.location.Coordinates.Split(',')[1])
            };

            int insertedItems = 0;
            // added using SQLite;
            using (SQLiteConnection conn = new SQLiteConnection(App.DatabasePath))
            {
                conn.CreateTable<Experience>();
                insertedItems = conn.Insert(newExperience);
            }
            // here the conn has been disposed of, hence closed
            if (insertedItems > 0)
            {
                Title = string.Empty;
                Content = string.Empty;
                SelectedVenue = null;
            }
            else
            {
                App.Current.MainPage.DisplayAlert("Error", "There was an error inserting the Experience, please try again", "Ok");
            }
        }

        void CancelAction(object obj)
        {
            App.Current.MainPage.Navigation.PopAsync();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
