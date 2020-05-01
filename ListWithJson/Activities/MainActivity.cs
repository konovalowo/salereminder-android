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

namespace ListWithJson
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : Activity
    {
        RestService _restService;
        User _user;

        List<Product> products;
        ProductAdapter productAdapter;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            SetContentView(Resource.Layout.activity_main);

            // Sign in
            // Check is signed in
            //AuthenticateUser();

            //delete
            _user = new User("boy", "bang");
            _restService = new RestService(_user);

            // Toolbar
            var toolbar = FindViewById<Android.Widget.Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            ActionBar.Title = Resources.GetString(Resource.String.app_name);

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
            products = await _restService.Get();

            // Setting up RecyclerView
            var productRecyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerViewProducts);
            productRecyclerView.SetLayoutManager(new GridLayoutManager(this, 2));
            productAdapter = new ProductAdapter(products);
            productRecyclerView.SetAdapter(productAdapter);

            // Handling itemClick event
            productAdapter.ItemClick += OnItemClickHandler;
        }

        private void AuthenticateUser()
        {
            if (AppPreferenceUser.isLogged)
            {
                _user = AppPreferenceUser.GetUser();
            }
            else
            {
                var intent = new Intent(this, typeof(SignInActivity));
                StartActivity(intent);
                Finish();
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
            var product = await _restService.Post(new Product() { Url = url });
            if (product != null)
            {
                productAdapter.AddItem(product);
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
            productAdapter.products = productsNew;
            productAdapter.NotifyDataSetChanged();
        }
    }
}