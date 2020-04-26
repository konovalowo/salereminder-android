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

namespace ListWithJson
{
    public static class Constants
    {
        public static string BaseAdress = "https://10.0.2.2:5001";
        public static string ProductApiUrl = BaseAdress + "/api/Products/{0}";
    }
}