using System;
using System.Collections.Generic;
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

        public bool InsertExperience()
        {
            int insertedItems = 0;
            using (SQLiteConnection conn = new SQLiteConnection(App.DatabasePath))
            {
                conn.CreateTable<Experience>();
                insertedItems = conn.Insert(this);
            }

            return insertedItems > 0;
        }

        public static List<Experience> GetExperiences()
        {
            // added using System.Collections.Generic;
            using (SQLiteConnection conn = new SQLiteConnection(App.DatabasePath))
            {
                conn.CreateTable<Experience>();
                return conn.Table<Experience>().ToList();
            }
        }

        public override string ToString()
        {
            return Title;
        }
    }
}
