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

namespace EDHMTGLifeCounter.ViewModel
{
    public class LifeCounter : Base
    {
        public int Life { get; set; }
        public int PoisonDamageCounter { get; set; }
        public int CommanderDamageCounter { get; set; }

        public bool CheckPoisonCounterDeath()
        {
            if (PoisonDamageCounter >= 10)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}