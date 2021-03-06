﻿using Android.Views;
using Android.Widget;
using System;
using static Android.Support.V7.Widget.RecyclerView;

namespace ListWithJson
{
    class ProductViewHolder : ViewHolder
    {
        public ImageView Image { get; private set; }
        public TextView ProductName { get; private set; }
        public TextView ProductPrice { get; private set; }
        public TextView OnSaleLabel { get; private set; }

        public ProductViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            Image = itemView.FindViewById<ImageView>(Resource.Id.imageView);
            ProductName = itemView.FindViewById<TextView>(Resource.Id.NameTView);
            ProductPrice = itemView.FindViewById<TextView>(Resource.Id.PriceProductViewTextView);
            OnSaleLabel = itemView.FindViewById<TextView>(Resource.Id.OnSaleTextView);

            itemView.Click += (object sender, EventArgs e) => listener(base.LayoutPosition);
        }
    }
}