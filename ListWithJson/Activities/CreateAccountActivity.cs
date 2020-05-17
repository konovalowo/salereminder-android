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
    [Activity(Label = "CreateAccountActivity")]
    public class CreateAccountActivity : Activity
    {
        EditText editTextEmail;
        EditText editTextPassword;
        EditText editTextConfirmPassword;
        RestService _restServiceRegister;

        Button buttonCreateAccount;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_create_account);

            _restServiceRegister = new RestService();

            buttonCreateAccount = FindViewById<Button>(Resource.Id.buttonCreateAccount);
            editTextEmail = FindViewById<EditText>(Resource.Id.editTextEmailCreateAccount);
            editTextPassword = FindViewById<EditText>(Resource.Id.editTextPasswordCreateAccount);
            editTextConfirmPassword = FindViewById<EditText>(Resource.Id.editTextConfirmPasswordCreateAccount);

            buttonCreateAccount.Click += ButtonCreateAccount_Click;
        }

        private async void ButtonCreateAccount_Click(object sender, EventArgs e)
        {
            string email = editTextEmail.Text;
            string password = editTextPassword.Text;

            if (!IsValidEmail(email))
            {
                Toast.MakeText(this, Resource.String.invalid_email_format, ToastLength.Long).Show();
                return;
            }
            if (password.Length < 8)
            {
                Toast.MakeText(this, Resource.String.password_tooshort, ToastLength.Long).Show();
                return;
            }
            if (password != editTextConfirmPassword.Text)
            {
                Toast.MakeText(this, Resource.String.passwords_dont_match, ToastLength.Long).Show();
                return;
            }

            buttonCreateAccount.Enabled = false;
            User user = await _restServiceRegister.Authenticate(new User { Email = email, Password = password }, true);

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
                buttonCreateAccount.Enabled = true;
                Toast.MakeText(this, Resource.String.incorrect_pass_or_email, ToastLength.Long).Show();
            }
        }

        private bool IsValidEmail(string email) => (Android.Util.Patterns.EmailAddress.Matcher(email).Matches());

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            var intent = new Intent(this, typeof(SignInActivity));
            StartActivity(intent);
        }
    }
}