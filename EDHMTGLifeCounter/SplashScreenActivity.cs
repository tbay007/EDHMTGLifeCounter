using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace EDHMTGLifeCounter
{
    [Activity(Label = "EDH Life Counter", MainLauncher = true, Theme = "@style/Theme.Splash", NoHistory = true, Icon = "@drawable/ic_launcher")]
    public class SplashScreenActivity : Activity
    {
        private System.Timers.Timer _timer;
        private int _countSeconds;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _timer = new System.Timers.Timer();
            //Trigger event every second
            _timer.Interval = 1000;
            _timer.Elapsed += OnTimedEvent;
            //count down 5 seconds
            _countSeconds = 2;

            _timer.Enabled = true;
        }

        private void OnTimedEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            _countSeconds--;

            if (_countSeconds == 0)
            {
                StartActivity(typeof(MainActivity));
                _timer.Stop();
            }
        }
    }
}