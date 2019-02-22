using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace TenDaysOfXamarin
{
    public partial class App : Application
    {
        public string DatabasePath;

        public App(string databasePath)
        {
            InitializeComponent();

            MainPage = new MainPage();

            DatabasePath = databasePath;
        }

        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
