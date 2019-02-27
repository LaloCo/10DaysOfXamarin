using System;
using SQLite;

namespace TenDaysOfXamarin.Model
{
    public class Experience
    {
        [PrimaryKey, AutoIncrement] // added using SQLite;
        public int Id { get; set; }

        [MaxLength(50)]
        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public string VenueName { get; set; }

        public string VenueCategory { get; set; }

        public float VenueLat { get; set; }

        public float VenueLng { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }
}
