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

namespace ListWithJson.Activities
{
    [Activity(Label = "SignInActivity")]
    public class SignInActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.activity_signin);

            var editTextEmail = FindViewById<EditText>(Resource.Id.editTextEmailSignIn);
            var editTextPassword = FindViewById<EditText>(Resource.Id.editTextPasswordSignIn);
            var buttonSignIn = FindViewById<Button>(Resource.Id.buttonSignIn);
            var buttonToCreateAccount = FindViewById<Button>(Resource.Id.buttonToCreateAccount);

            buttonSignIn.Click += ButtonSignIn_Click;
        }

        private void ButtonSignIn_Click(object sender, EventArgs e)
        {
            if (ValidateUserCredentials())
            {

            }
        }

        private bool ValidateUserCredentials()
        {
            throw new NotImplementedException();
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            // Minimize app of finish?
            //Intent main = new Intent(Intent.ActionMain);
            //main.AddCategory(Intent.CategoryHome);
            //StartActivity(main);
        }
    }
}