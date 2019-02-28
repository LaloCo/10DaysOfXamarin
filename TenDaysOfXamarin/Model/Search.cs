using System;
using System.Linq;
using System.Collections.Generic;

namespace TenDaysOfXamarin.Model
{
    public class Meta
    {
        public int code { get; set; }
        public string requestId { get; set; }
    }

    public class LabeledLatLng
    {
        public string label { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Location
    {
        public double lat { get; set; }
        public double lng { get; set; }
        public IList<LabeledLatLng> labeledLatLngs { get; set; }
        public int distance { get; set; }
        public string cc { get; set; }
        public string country { get; set; }
        public IList<string> formattedAddress { get; set; }

        private string coordinates;
        public string Coordinates
        {
            get { return $"{lat:0.000}, {lng:0.000}"; }
        }
    }

    public class Icon
    {
        public string prefix { get; set; }
        public string suffix { get; set; }
    }

    public class Category
    {
        public string id { get; set; }
        public string name { get; set; }
        public string pluralName { get; set; }
        public string shortName { get; set; }
        public Icon icon { get; set; }
        public bool primary { get; set; }
    }

    public class Venue
    {
        public string id { get; set; }
        public string name { get; set; }
        public Location location { get; set; }
        public IList<Category> categories { get; set; }
        public string referralId { get; set; }
        public bool hasPerk { get; set; }

        private string mainCategory;
        public string MainCategory
        {
            // added using System.Linq;
            get { return categories.FirstOrDefault()?.name; }
        }
    }

    public class Response
    {
        public IList<Venue> venues { get; set; }
        public bool confident { get; set; }
    }

    public class Search
    {
        public Meta meta { get; set; }
        public Response response { get; set; }
    }
}
