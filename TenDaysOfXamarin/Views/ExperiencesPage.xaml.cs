using System;
using System.Collections.Generic;
using SQLite;
using TenDaysOfXamarin.Model;
using Xamarin.Forms;

namespace TenDaysOfXamarin.Views
{
    public partial class ExperiencesPage : ContentPage
    {
        public ExperiencesPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            ReadExperiences();
        }

        private void ReadExperiences()
        {
            // added using SQLite;
            using (SQLiteConnection conn = new SQLiteConnection(App.DatabasePath))
            {
                conn.CreateTable<Experience>(); // added using TenDaysOfXamarin.Model;
                List<Experience> experiences = conn.Table<Experience>().ToList();
                experiencesListView.ItemsSource = experiences;
            }
        }

        void Handle_Clicked(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new MainPage());
        }
    }
}
