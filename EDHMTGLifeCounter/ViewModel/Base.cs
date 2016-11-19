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
    public abstract class Base
    {
        public int Id { get; set; }

        public bool Inserted { get; set; }
        public bool Updated { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string User { get; set; }
    }
}