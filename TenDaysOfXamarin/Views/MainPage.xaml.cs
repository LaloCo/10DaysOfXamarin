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
using TenDaysOfXamarin.ViewModels.Helpers;

namespace TenDaysOfXamarin
{
    public partial class MainPage : ContentPage
    {
        // added using TenDaysOfXamarin.ViewModels;
        MainVM viewModel;

        public MainPage()
        {
            InitializeComponent();

            viewModel = new MainVM();
            BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            viewModel.GetLocationPermission();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            viewModel.StopListeningLocationUpdates();
        }
    }
}
