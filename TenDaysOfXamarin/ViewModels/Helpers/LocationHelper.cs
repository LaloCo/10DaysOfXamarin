using System;
using System.Threading.Tasks;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;

namespace TenDaysOfXamarin.ViewModels.Helpers
{
    public class LocationHelper
    {
        // added using Plugin.Geolocator;
        // added using Plugin.Geolocator.Abstractions;
        public IGeolocator locator = CrossGeolocator.Current;
        public Position position;

        public LocationHelper()
        {
            locator.PositionChanged += Locator_PositionChanged;
        }

        void Locator_PositionChanged(object sender, PositionEventArgs e)
        {
            position = e.Position;
        }

        // added using System.Threading.Tasks;
        public async Task<Position> GetLocation(TimeSpan minimumTime, int minimumMeters)
        {
            position = await locator.GetPositionAsync();
            await locator.StartListeningAsync(minimumTime, minimumMeters);
            return position;
        }

        public async void StopListening()
        {
            await locator.StopListeningAsync();
        }
    }
}
