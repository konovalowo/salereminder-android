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
using ListWithJson.Utils;

namespace ListWithJson.Activities
{
    [Activity(Label = "SignInActivity", NoHistory = true, ExcludeFromRecents = true)]
    public class SignInActivity : Activity
    {
        EditText editTextEmail;
        EditText editTextPassword;
        RestService _restServiceSignIn;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_signin);

            _restServiceSignIn = new RestService();

            var buttonSignIn = FindViewById<Button>(Resource.Id.buttonSignIn);
            var buttonToCreateAccount = FindViewById<Button>(Resource.Id.buttonToCreateAccount);
            editTextEmail = FindViewById<EditText>(Resource.Id.editTextEmailSignIn);
            editTextPassword = FindViewById<EditText>(Resource.Id.editTextPasswordSignIn);

            buttonSignIn.Click += ButtonSignIn_Click;
            buttonToCreateAccount.Click += (s, e) =>
            {
                var intent = new Intent(this, typeof(CreateAccountActivity));
                StartActivity(intent);
            };
        }

        private async void ButtonSignIn_Click(object sender, EventArgs e)
        {
            string email = editTextEmail.Text;
            string password = editTextPassword.Text;

            User user = await _restServiceSignIn.Authenticate(new User { Email = email, Password = password }, false);

            if (user != null)
            {
                user.Password = password;
                AppPreferenceUser.SetUser(user);
                var intent = new Intent(this, typeof(MainActivity));
                StartActivity(intent);
                Finish();
            }
            else
            {
                Toast.MakeText(this, Resource.String.incorrect_pass_or_email, ToastLength.Long).Show();
            }
        }
    }
}