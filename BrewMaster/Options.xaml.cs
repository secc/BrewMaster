using System;
using System.Collections.Generic;
using System.Timers;
using BrewMaster.ViewModels;
using Xamarin.Forms;

namespace BrewMaster
{
    public partial class Options : ContentPage
    {
        public Options()
        {
            InitializeComponent();
            BindingContext = MainPageViewModel.Current;
            Timer timer = new Timer(60*1000);
            timer.AutoReset = false;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await Navigation.PopAsync();
            });
        }

        void btnCancel_Clicked(System.Object sender, System.EventArgs e)
        {
            Navigation.PopAsync();
        }

        void btnHalf_Clicked(System.Object sender, System.EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                var result = await this.DisplayAlert("Confirm Half Pot",
                    "Please confirm you wish to start the timer for a new half pot of coffee.", "Yes", "No");
                if (result)
                {
                    MainPageViewModel.Current.StartHalfBrew();
                    await Navigation.PopAsync();
                }
            });

        }

        void btnFull_Clicked(System.Object sender, System.EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                var result = await this.DisplayAlert("Confirm Full Pot",
                    "Please confirm you wish to start the timer for a new full pot of coffee.", "Yes", "No");
                if (result)
                {
                    MainPageViewModel.Current.StartFullBrew();
                    await Navigation.PopAsync();
                }
            });
        }
    }
}
