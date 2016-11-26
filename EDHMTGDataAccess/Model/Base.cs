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
using SQLite;

namespace EDHMTGDataAccess.Model
{
    public class Base
    {
        [AutoIncrement, PrimaryKey]
        public int Id { get; set; }
        public bool Inserted { get; set; }
        public bool Updated { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string User { get; set; }
    }
}