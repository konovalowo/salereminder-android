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
using Android.Support.V7.Widget;
using Android.Content.Res;
using static Android.Support.V7.Widget.RecyclerView;

namespace ListWithJson
{
    class ProductAdapter : RecyclerView.Adapter
    {
        private List<Product> products;
        private List<Product> currentList;

        public event EventHandler<ItemEventArgs> ItemClick;

        private Context _context;

        private string currentWebsiteFilter;

        public event EventHandler OnListChanged;

        public ProductAdapter(List<Product> products, Context context)
        {
            this.products = products;
            currentList = products;
            _context = context;
        }

        public override int ItemCount => products.Count;

        private void OnClick(int position)
        {
            ItemClick?.Invoke(this, new ItemEventArgs(products[position]));
        }

        public override void OnBindViewHolder(ViewHolder holder, int position)
        {
            ProductViewHolder vh = holder as ProductViewHolder;

            vh.Image.SetImageDrawable(_context.GetDrawable(Resource.Drawable.ic_material_product_icon)); // placeholder
            vh.Image.ImageFromUrlAsync(products[position].Image);
            vh.ProductName.Text = products[position].Name;
            vh.ProductPrice.Text = products[position].Price.ToString("C", products[position].GetCulture());
            if (products[position].IsOnSale)
            {
                vh.ProductPrice.SetTextColor(vh.OnSaleLabel.TextColors);
                vh.OnSaleLabel.Visibility = ViewStates.Visible;
            }
            else
            {
                vh.OnSaleLabel.Visibility = ViewStates.Gone;
            }
        }

        public override ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).
                        Inflate(Resource.Layout.product_view, parent, false);

            ProductViewHolder vh = new ProductViewHolder(itemView, OnClick);
            return vh;
        }

        public void SetProducts(List<Product> products)
        {
            currentList = products;
            FilterByWebsite(currentWebsiteFilter);
            OnListChanged?.Invoke(this, EventArgs.Empty);
        }

        //public void AddItem(Product product)
        //{
        //    products.Add(product);
        //    NotifyDataSetChanged();
        //}

        public List<string> GetWebsites()
        {
            var websites = currentList.GroupBy(p => p.GetWebsiteShort())
                                      .OrderBy(g => g.Key)
                                      .Select(g => g.Key);
            return websites.ToList();
        }

        public void FilterByWebsite(string websiteUrl)
        {
            currentWebsiteFilter = websiteUrl;
            if (websiteUrl != null)
            {
                products = currentList.Where(p => p.GetWebsiteShort() == websiteUrl).ToList();
            }
            else
            {
                products = currentList;
            }
            NotifyDataSetChanged();
        }
    }
}