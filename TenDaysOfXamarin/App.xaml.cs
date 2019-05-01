#define OFFLINE_SYNC_ENABLED

using System;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using TenDaysOfXamarin.Helpers;
using TenDaysOfXamarin.Model;
using TenDaysOfXamarin.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace TenDaysOfXamarin
{
    public partial class App : Application
    {
        public static string DatabasePath;

        public App(string databasePath)
        {
            InitializeComponent();

            DatabasePath = databasePath;

            MainPage = new NavigationPage(new ExperiencesPage()); // added using TenDaysOfXamarin.Views;

            var store = new MobileServiceSQLiteStore(databasePath);
            store.DefineTable<Experience>();

            AzureHelper.MobileService.SyncContext.InitializeAsync(store, AzureHelper.MobileService.SyncContext.Handler);
            AzureHelper.experienceTable = AzureHelper.MobileService.GetSyncTable<Experience>();
        }

        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new ExperiencesPage());
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
