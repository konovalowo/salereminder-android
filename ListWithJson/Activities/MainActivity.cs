using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Content;
using Android.Support.V7.Widget;
using Newtonsoft.Json;
using Android.Support.Design.Widget;
using Android.Views;

using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

using ListWithJson.Activities;
using ListWithJson.Models;
using ListWithJson.Utils;
using System;
using Android.Util;
using Android.Gms.Common;
using Xamarin.Essentials;
using Android.Support.V4.Widget;

namespace ListWithJson
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : Activity
    {
        const string logTag = "MainActivity";

        RestService _restService;
        User _user;

        List<Product> products;
        ProductAdapter productAdapter;

        DrawerLayout drawerLayout;

        ArrayAdapter filterListAdapter;

        const string ChannelId = "salereminder_channel";

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            SetContentView(Resource.Layout.activity_main);

            _restService = new RestService();

            // Sign in
            // Check is signed in
            _user = AuthenticateUser();
            if (_user == null)
            {
                Finish();
                return;
            }
            
            // Send token after user logged in
            if (await _restService.Authenticate(_user, false) != null)
            {
                _restService.User = _user;
                await _restService.RegisterFirebaseToken(Preferences.Get(Constants.FirebaseTokenPreferenceTag, null));
            }
            else
            {
                AppPreferenceUser.RemoveUser();
                AuthenticateUser();
            }

            // Navigation drawer
            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawerLayout);
            var navigationView = FindViewById<NavigationView>(Resource.Id.navigationView);

            // Toolbar
            var toolbar = FindViewById<Android.Widget.Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            //ActionBar.Title = Resources.GetString(Resource.String.app_name);
            ActionBar.Title = GetString(Resource.String.all_products);
            ActionBar.SetHomeButtonEnabled(true);
            ActionBar.SetDisplayHomeAsUpEnabled(true);
            ActionBar.SetHomeAsUpIndicator(GetDrawable(Resource.Drawable.baseline_menu_white_18dp));

            toolbar.NavigationOnClick += (s, e) =>
            {
                drawerLayout.OpenDrawer(Android.Support.V4.View.GravityCompat.Start);
            };


            // drawer
            var drawer = FindViewById<DrawerLayout>(Resource.Id.drawerLayout);

            // Fab
            var fab = FindViewById<FloatingActionButton>(Resource.Id.fabAdd);
            fab.Click += (sender, e) =>
            {
                var transaction = FragmentManager.BeginTransaction();
                var dialogFragment = new AddDialogFragment();
                dialogFragment.onAddButtonClicked += OnAddButtonClickedHandler;
                dialogFragment.Show(transaction, "add_dialog_fragment");
            };

            // Product list
            products = new List<Product>();
            products.AddRange(await _restService.Get());

            // Setting up RecyclerView
            var productRecyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerViewProducts);
            productRecyclerView.SetLayoutManager(new GridLayoutManager(this, 2));
            productAdapter = new ProductAdapter(products, this);
            productRecyclerView.SetAdapter(productAdapter);

            // Handling itemClick event
            productAdapter.ItemClick += OnItemClickHandler;

            //Filter list on Navigation drawer
            var filterListView = FindViewById<ListView>(Resource.Id.filterListView);
            var websites = new List<string> { GetString(Resource.String.all_products) };
            websites.AddRange(productAdapter.GetWebsites());
            filterListAdapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItemActivated1, websites);
            filterListView.Adapter = filterListAdapter;
            filterListView.ItemClick += FilterListView_ItemClick;
            filterListView.ItemClick += (s, e) => drawerLayout.CloseDrawer(Android.Support.V4.View.GravityCompat.Start);
            filterListView.ChoiceMode = ChoiceMode.Single;
            filterListView.SetItemChecked(0, true);

            productAdapter.OnListChanged += (s, e) =>
            {
                filterListAdapter.Clear();
                var websites = new List<string> { GetString(Resource.String.all_products) };
                websites.AddRange(productAdapter.GetWebsites());
                filterListAdapter.AddAll(websites);
                filterListAdapter.NotifyDataSetChanged();
            };

            // Firebase messaging
            IsPlayServicesAvailable();
            CreateNotificationChannel();
        }

        private void FilterListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            try
            {
                if (e.Id == 0)
                {
                    ActionBar.Title = GetString(Resource.String.all_products);
                    productAdapter.FilterByWebsite(null);
                }
                else
                {
                    string websiteUrl = productAdapter.GetWebsites()[(int)e.Id - 1];
                    ActionBar.Title = websiteUrl;
                    productAdapter.FilterByWebsite(websiteUrl);
                }
            }
            catch (IndexOutOfRangeException)
            {
                Log.Error(logTag, "Websites filter out of range");
            }
        }

        private User AuthenticateUser()
        {
            if (AppPreferenceUser.isLogged)
            {
                 return AppPreferenceUser.GetUser(); 
            }
            else
            {
                var intent = new Intent(this, typeof(SignInActivity));
                StartActivity(intent);
                return null;
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void OnItemClickHandler(object sender, ItemEventArgs eProd)
        {
            // Starting new activity
            var intent = new Intent(this, typeof(ProductPageActivity));
            var product = eProd.Product;

            // Pass product object using json serialization
            intent.PutExtra("product", JsonConvert.SerializeObject(product));
            StartActivityForResult(intent, 0);

            // Animation
            OverridePendingTransition(Resource.Animation.abc_popup_enter, Resource.Animation.abc_popup_exit);
        }

        protected override async void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == 0)
            {
                if (resultCode == Result.Ok)
                {
                    bool isDeleteClicked = data.GetBooleanExtra("isDeleteClicked", false);
                    if (isDeleteClicked)
                    {
                        await _restService.Delete(data.GetIntExtra("id", 0));
                        await RefreshProductListPage();
                    }
                }
            }
        }

        private async void OnAddButtonClickedHandler(object sender, AddButtonEventArgs e)
        {
            string url = e.Text;
            var product = await _restService.Put(url);
            if (product != null)
            {
                await RefreshProductListPage();
                //productAdapter.AddItem(product);
            }
            else
            {
                Toast.MakeText(ApplicationContext, Resource.String.error_parsing, ToastLength.Long).Show();
            }
        }

        // Toolbar
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.top_menus, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.menuSignout:
                    AppPreferenceUser.RemoveUser();
                    AuthenticateUser();
                    break;
                case Resource.Id.menuRefresh:
                    var ignoreTask = OnOptionsItemSelectedAsync(item);
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }

        public async Task OnOptionsItemSelectedAsync(IMenuItem item)
        {
            await RefreshProductListPage();
        }

        public async Task RefreshProductListPage()
        {
            List<Product> productsNew = await _restService.Get();
            productAdapter.SetProducts(productsNew);
            //productAdapter.products = productsNew;
            //productAdapter.NotifyDataSetChanged();
        }

        // FirebaseMessaging
        public bool IsPlayServicesAvailable()
        {
            int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (resultCode != ConnectionResult.Success)
            {
                if (GoogleApiAvailability.Instance.IsUserResolvableError(resultCode))
                {
                    Log.Info(logTag, GoogleApiAvailability.Instance.GetErrorString(resultCode));
                }
                else
                {
                    Toast.MakeText(ApplicationContext, "Google Play services unavailable", ToastLength.Long).Show();
                    Finish();
                }
                return false;
            }
            else
            {
                return true;
            }
        }

        void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                return;
            }

            var channel = new NotificationChannel(ChannelId,
                                                  "Sale Reminder notifications",
                                                  NotificationImportance.Default)
            {

                Description = "Firebase Cloud Messages appear in this channel"
            };

            var notificationManager = (NotificationManager)GetSystemService(Android.Content.Context.NotificationService);
            notificationManager.CreateNotificationChannel(channel);
        }
    }
}