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
using System.Net.Http;
using Newtonsoft.Json;
using TenDaysOfXamarin.ViewModels;

namespace TenDaysOfXamarin
{
    public partial class MainPage : ContentPage
    {
        // added using Plugin.Geolocator;
        // added using Plugin.Geolocator.Abstractions;
        IGeolocator locator = CrossGeolocator.Current;
        Position position;

        // added using TenDaysOfXamarin.ViewModels;
        MainVM viewModel;

        public MainPage()
        {
            InitializeComponent();

            viewModel = new MainVM();
            BindingContext = viewModel;

            locator.PositionChanged += Locator_PositionChanged;
        }

        void Locator_PositionChanged(object sender, PositionEventArgs e)
        {
            position = e.Position;
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
            position = await locator.GetPositionAsync();
            await locator.StartListeningAsync(TimeSpan.FromMinutes(30), 500);
        }

        async void SearchEntry_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(viewModel.Query))
            {
                string url = $"https://api.foursquare.com/v2/venues/search?ll={position.Latitude},{position.Longitude}&radius=500&query={viewModel.Query}&limit=3&client_id={Helpers.Constants.FOURSQR_CLIENT_ID}&client_secret={Helpers.Constants.FOURSQR_CLIENT_SECRET}&v={DateTime.Now.ToString("yyyyMMdd")}";

                // added using System.Net.Http;
                using (HttpClient client = new HttpClient())
                {
                    // made the method async
                    string json = await client.GetStringAsync(url);

                    // added using Newtonsoft.Json;
                    Search searchResult = JsonConvert.DeserializeObject<Search>(json);
                    viewModel.ShowVenues = true;
                    venuesListView.ItemsSource = searchResult.response.venues;
                }
            }
            else
            {
                viewModel.ShowVenues = false;
            }
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
