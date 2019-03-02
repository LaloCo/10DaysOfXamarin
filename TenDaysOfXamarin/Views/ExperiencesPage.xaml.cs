using System;
using System.Collections.Generic;
using SQLite;
using TenDaysOfXamarin.Model;
using TenDaysOfXamarin.ViewModels;
using Xamarin.Forms;

namespace TenDaysOfXamarin.Views
{
    public partial class ExperiencesPage : ContentPage
    {
        ExperiencesVM viewModel;
        public ExperiencesPage()
        {
            InitializeComponent();

            viewModel = Resources["vm"] as ExperiencesVM;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            viewModel.ReadExperiences();
        }
    }
}
