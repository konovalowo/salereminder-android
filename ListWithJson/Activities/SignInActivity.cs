﻿using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using ListWithJson.Models;
using ListWithJson.Utils;
using System;

namespace ListWithJson.Activities
{
    [Activity(Label = "SignInActivity", NoHistory = true, ExcludeFromRecents = true)]
    public class SignInActivity : Activity
    {
        EditText editTextEmail;
        EditText editTextPassword;
        RestService _restService;

        Button buttonSignIn;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_signin);

            _restService = new RestService();

            buttonSignIn = FindViewById<Button>(Resource.Id.buttonSignIn);
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

            buttonSignIn.Enabled = false;
            User user = await _restService.Authenticate(new User { Email = email, Password = password }, false);

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
                buttonSignIn.Enabled = true;
                Toast.MakeText(this, Resource.String.incorrect_pass_or_email, ToastLength.Long).Show();
            }
        }
    }
}