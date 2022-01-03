using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using BrewMaster.ViewModels;
using Xamarin.Forms;

namespace BrewMaster
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();

            if (MainPageViewModel.Current == null)
            {
                new MainPageViewModel();
            }

            BindingContext = MainPageViewModel.Current;

        }

        void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new Options(), true);
        }
    }

   
}
