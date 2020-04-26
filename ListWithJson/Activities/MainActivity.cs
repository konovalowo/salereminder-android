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

namespace ListWithJson
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : Activity
    {
        RestService _restService;
        User _user;

        List<Product> products;
        ProductAdapter productAdapter;
        RecyclerView productRecyclerView;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            SetContentView(Resource.Layout.activity_main);

            // Sign in
            // Check is signed in
            if (_user == null)
            {
                var intent = new Intent(this, typeof(SignInActivity));
                StartActivityForResult(intent, 1);
            }

            _restService = new RestService();

            // Toolbar
            var toolbar = FindViewById<Android.Widget.Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            ActionBar.Title = "actionbar";

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
            else if (requestCode == 1)
            {
                // Sign In handler
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

        // Menu
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.top_menus, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            var ignoreTask = OnOptionsItemSelectedAsync(item);
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