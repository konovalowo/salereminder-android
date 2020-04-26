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

using ListWithJson.Models;
using Newtonsoft.Json;
using Xamarin.Essentials;

namespace ListWithJson.Utils
{
    static class AppPreferenceUser
    {
        public static bool isLogged => Preferences.ContainsKey("user");

        public static void SetUser(User user)
        {
            string userJson = JsonConvert.SerializeObject(user);
            Preferences.Set("user", userJson);
        }

        public static User GetUser()
        {
            if (Preferences.ContainsKey("user"))
            {
                string userJson = Preferences.Get("user", null);
                User user = JsonConvert.DeserializeObject<User>(userJson);
                return user;
            }
            else
            {
                return null;
            }
        }
    }
}