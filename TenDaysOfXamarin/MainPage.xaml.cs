using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TenDaysOfXamarin
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        void Handle_Clicked(object sender, System.EventArgs e)
        {
            if(!string.IsNullOrWhiteSpace(nameEntry.Text))
            {
                greetingLabel.Text = $"Hello {nameEntry.Text}, welcome to 10 Days of Xamarin.";
            }
            else
            {
                DisplayAlert("Error", "Your name can't be empty", "Oh right");
            }
        }
    }
}
