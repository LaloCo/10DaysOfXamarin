using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;
using TenDaysOfXamarin.Helpers;

namespace TenDaysOfXamarin.Model
{
    public class Experience
    {
        //[PrimaryKey, AutoIncrement] // added using SQLite;
        public string Id { get; set; }

        //[MaxLength(50)]
        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public string VenueName { get; set; }

        public string VenueCategory { get; set; }

        public float VenueLat { get; set; }

        public float VenueLng { get; set; }

        public async Task<bool> InsertExperience()
        {
            /*int insertedItems = 0;
            using (SQLiteConnection conn = new SQLiteConnection(App.DatabasePath))
            {
                conn.CreateTable<Experience>();
                insertedItems = conn.Insert(this);
            }

            return insertedItems > 0;*/

            try
            {
                await AzureHelper.MobileService.GetTable<Experience>().InsertAsync(this);
            }
            catch(Exception ex)
            {
                return false;
            }

            return true;
        }

        public static async Task<List<Experience>> GetExperiences()
        {
            // added using System.Collections.Generic;
            /*using (SQLiteConnection conn = new SQLiteConnection(App.DatabasePath))
            {
                conn.CreateTable<Experience>();
                return conn.Table<Experience>().ToList();
            }*/

            return await AzureHelper.MobileService.GetTable<Experience>().ToListAsync();
        }

        public override string ToString()
        {
            return Title;
        }
    }
}


// Data Source=tcp:contactosudemy.database.windows.net,1433;Initial Catalog=deletelaterlpa_db;User ID=lalorosas;Password=LaloCo11235813#
