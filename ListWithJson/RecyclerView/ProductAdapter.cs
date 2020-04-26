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


namespace ListWithJson
{
    class ProductAdapter : RecyclerView.Adapter
    {
        public List<Product> products;
        public event EventHandler<ItemEventArgs> ItemClick;

        public ProductAdapter(List<Product> products)
        {
            this.products = products;
        }

        public override int ItemCount => products.Count;

        private void OnClick(int position)
        {
            ItemClick?.Invoke(this, new ItemEventArgs(products[position]));
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ProductViewHolder vh = holder as ProductViewHolder;

            vh.Image.ImageFromUrlAsync(products[position].Image);
            vh.ProductName.Text = products[position].Name;
            vh.ProductPrice.Text = products[position].Price.ToString("C", products[position].GetCulture());
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).
                        Inflate(Resource.Layout.product_view, parent, false);

            ProductViewHolder vh = new ProductViewHolder(itemView, OnClick);
            return vh;
        }

        public void AddItem(Product product)
        {
            products.Add(product);
            NotifyDataSetChanged();
        }
    }
}