using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using SQLite;
using TenDaysOfXamarin.Model;
using Xamarin.Forms;

namespace TenDaysOfXamarin
{
    public partial class MainPage : ContentPage
    {
        // added using Plugin.Geolocator;
        // added using Plugin.Geolocator.Abstractions;
        IGeolocator locator = CrossGeolocator.Current;

        public MainPage()
        {
            InitializeComponent();

            locator.PositionChanged += Locator_PositionChanged;
        }

        void Locator_PositionChanged(object sender, PositionEventArgs e)
        {
            var position = e.Position;
        }


        private async void GetLocationPermission()
        {
            // added using Plugin.Permissions;
            // added using Plugin.Permissions.Abstractions;
            var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.LocationWhenInUse);
            if(status != PermissionStatus.Granted)
            {
                // Not granted, request permission
                if(await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.LocationWhenInUse))
                {
                    // This is not the actual permission request
                    await DisplayAlert("Need your permission", "We need to access your location", "Ok");
                }

                // This is the actual permission request
                var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.LocationWhenInUse);
                if (results.ContainsKey(Permission.LocationWhenInUse))
                    status = results[Permission.LocationWhenInUse];
            }

            // Already granted (maybe), go on
            if(status == PermissionStatus.Granted)
            {
                // Granted! Get the location
                GetLocation();
            }
            else
            {
                await DisplayAlert("Access to location denied", "We don't have access to your location", "Ok");
            }
        }

        private async void GetLocation()
        {
            var position = await locator.GetPositionAsync();
            await locator.StartListeningAsync(TimeSpan.FromMinutes(30), 500);
        }

        private void CheckIfShouldBeEnabled()
        {
            saveButton.IsEnabled = false;
            if (!string.IsNullOrWhiteSpace(titleEntry.Text) && !string.IsNullOrWhiteSpace(contentEditor.Text))
                saveButton.IsEnabled = true;
        }

        void TitleEntry_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            CheckIfShouldBeEnabled();
        }

        void ContentEditor_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            CheckIfShouldBeEnabled();
        }

        void SaveButton_Clicked(object sender, System.EventArgs e)
        {
            // added using TenDaysOfXamarin.Model;
            Experience newExperience = new Experience()
            {
                Title = titleEntry.Text,
                Content = contentEditor.Text,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
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
                titleEntry.Text = string.Empty;
                contentEditor.Text = string.Empty;
            }
            else
            {
                DisplayAlert("Error", "There was an error inserting the Experience, please try again", "Ok");
            }
        }

        void ContentEntry_Clicked(object sender, System.EventArgs e)
        {
            Navigation.PopAsync();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            GetLocationPermission();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            locator.StopListeningAsync();
        }
    }
}
