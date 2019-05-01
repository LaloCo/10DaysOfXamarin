using System;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using TenDaysOfXamarin.Model;

namespace TenDaysOfXamarin.Helpers
{
    public class AzureHelper
    {
        public static MobileServiceClient MobileService =
            new MobileServiceClient(
            "https://evernotelpa.azurewebsites.net"
        );

        public static IMobileServiceSyncTable<Experience> experienceTable;


    }
}
