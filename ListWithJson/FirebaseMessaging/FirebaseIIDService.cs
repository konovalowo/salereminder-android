using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Firebase.Iid;
using ListWithJson.Utils;
using Xamarin.Essentials;

namespace ListWithJson.FirebaseMessaging
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
    public class FirebaseIIDService : FirebaseInstanceIdService
    {
        const string logTag = "MyFirebaseIIDService";
        public override void OnTokenRefresh()
        {
            var refreshedToken = FirebaseInstanceId.Instance.Token;
            Log.Debug(logTag, "Refreshed token: " + refreshedToken);
            Preferences.Set(Constants.FirebaseTokenPreferenceTag, refreshedToken);
        }
    }
}