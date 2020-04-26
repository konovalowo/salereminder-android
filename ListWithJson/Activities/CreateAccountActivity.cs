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
    [Activity(Label = "CreateAccountActivity")]
    public class CreateAccountActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
        }

        // Lmao thats for create account
        private bool ValidateEmail(string email)
        {
            if (email.Contains('@'))
            {
                string[] emailSplit = email.Split('@');
            }
            else
            {
                AlertDialog.Builder alert = new AlertDialog.Builder(this);
                alert.SetMessage("Invalid email format");
                Dialog dialog = alert.Create();
                dialog.Show();
            }
            return false;
        }
    }
}