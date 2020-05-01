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
        const string userKey = "user";

        public static bool isLogged => Preferences.ContainsKey(userKey);

        public static void SetUser(User user)
        {
            string userJson = JsonConvert.SerializeObject(user);
            Preferences.Set(userKey, userJson);
        }

        public static User GetUser()
        {
            if (Preferences.ContainsKey(userKey))
            {
                string userJson = Preferences.Get(userKey, null);
                User user = JsonConvert.DeserializeObject<User>(userJson);
                return user;
            }
            else
            {
                return null;
            }
        }

        public static void RemoveUser()
        {
            Preferences.Remove(userKey);
        }
    }
}