using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Diagnostics;

using Xamarin.Android;
using Xamarin.Essentials;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Newtonsoft.Json;
using Android.Util;

namespace ListWithJson
{
    [Activity(Label = "ProductPage")]
    public class ProductPageActivity : Activity
    {
        Product product;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.product_page);

            var toolbar = FindViewById<Android.Widget.Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            ActionBar.Title = Resources.GetString(Resource.String.app_name);

            string productJson = Intent.GetStringExtra("product");
            product = JsonConvert.DeserializeObject<Product>(productJson);

            var imageView = FindViewById<ImageView>(Resource.Id.productImageView);
            var nameTextView = FindViewById<TextView>(Resource.Id.nameTextView);
            var priceTextView = FindViewById<TextView>(Resource.Id.priceTextView);
            var onSaleTextView = FindViewById<TextView>(Resource.Id.onSalePageTextView);
            var descTextView = FindViewById<TextView>(Resource.Id.descTextView);
            var brandTextView = FindViewById<TextView>(Resource.Id.brandTextView);

            if (product.IsOnSale)
            {
                priceTextView.SetTextColor(onSaleTextView.TextColors);
                onSaleTextView.Visibility = ViewStates.Visible;
            }
            else
            {
                onSaleTextView.Visibility = ViewStates.Gone;
            }

            var urlButton = FindViewById<Button>(Resource.Id.buttonOpenUrl);
            var deleteButton = FindViewById<Button>(Resource.Id.buttonDelete);

            urlButton.Click += (e, s) =>
            {
                try
                {
                    Browser.OpenAsync(new Uri(product.Url));
                }
                catch(Exception ex)
                {
                    Log.Error("ProductPageActivity", $"Exception while trying to open product page: {ex}");
                    Toast.MakeText(Application.Context, "Ivalid link", ToastLength.Long).Show();
                }
            };

            deleteButton.Click += (e, s) =>
            {
                Intent mainIntent = new Intent(this, typeof(MainActivity));
                mainIntent.PutExtra("isDeleteClicked", true);
                mainIntent.PutExtra("id", product.Id);
                SetResult(Result.Ok, mainIntent);
                Finish();
            };

            nameTextView.Text = product.Name;
            priceTextView.Text = product.Price.ToString("C", product.GetCulture());
            descTextView.Text = product.Description;
            brandTextView.Text = product.Brand;
            imageView.ImageFromUrlAsync(product.Image);
        }
    }
}