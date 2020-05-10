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
        const string logTag = "ProductPageActivity";

        Product product;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.product_page);

            // toolbar
            var toolbar = FindViewById<Android.Widget.Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            ActionBar.Title = Resources.GetString(Resource.String.app_name);
            ActionBar.SetHomeButtonEnabled(true);
            ActionBar.SetDisplayHomeAsUpEnabled(true);

            // back button
            toolbar.NavigationOnClick += (s, e) =>
            {
                base.OnBackPressed();
            };
            
            string productJson = Intent.GetStringExtra("product");
            product = JsonConvert.DeserializeObject<Product>(productJson);

            var imageView = FindViewById<ImageView>(Resource.Id.productImageView);
            var nameTextView = FindViewById<TextView>(Resource.Id.nameTextView);
            var websiteTextView = FindViewById<TextView>(Resource.Id.websiteTextView);
            var priceTextView = FindViewById<TextView>(Resource.Id.priceTextView);
            var onSaleTextView = FindViewById<TextView>(Resource.Id.onSalePageTextView);
            var descTextView = FindViewById<TextView>(Resource.Id.descTextView);
            var descScrollView = FindViewById<Android.Support.V4.Widget.NestedScrollView>(Resource.Id.descriptionScrollView);
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

            websiteTextView.Click += (e, s) =>
            {
                try
                {
                    Browser.OpenAsync(new Uri(product.GetWebsite()));
                }
                catch (Exception ex)
                {
                    Log.Error(logTag, $"Exception while trying to open product page: {ex}");
                    Toast.MakeText(Application.Context, "Ivalid link", ToastLength.Long).Show();
                }
            };

            var urlButton = FindViewById<Button>(Resource.Id.buttonOpenUrl);
            var deleteButton = FindViewById<Button>(Resource.Id.buttonDelete);

            urlButton.Click += (e, s) =>
            {
                try
                {
                    Browser.OpenAsync(new Uri(product.Url));
                }
                catch (Exception ex)
                {
                    Log.Error(logTag, $"Exception while trying to open product page: {ex}");
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
            websiteTextView.Text = product.GetWebsiteShort();
            priceTextView.Text = product.Price.ToString("C", product.GetCulture());
            descTextView.Text = System.Net.WebUtility.HtmlDecode(product.Description);
            brandTextView.Text = product.Brand;
            imageView.SetImageDrawable(GetDrawable(Resource.Drawable.ic_material_product_icon));
            imageView.ImageFromUrlAsync(product.Image);

            descScrollView.VerticalFadingEdgeEnabled = true;

            // set gone if no data available
            if (brandTextView.Text == "" || brandTextView.Text == null)
            {
                brandTextView.Visibility = ViewStates.Gone;
            }

            if (descTextView.Text == "" || descTextView.Text == null)
            {
               descScrollView.Visibility = ViewStates.Gone;
            }
            
            if (nameTextView.Text == "" || nameTextView.Text == null)
            {
                nameTextView.Visibility = ViewStates.Gone;
            }
        }
    }
}