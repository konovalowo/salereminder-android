﻿using System;
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
    class ProductViewHolder : RecyclerView.ViewHolder
    {
        public ImageView Image { get; private set; }
        public TextView ProductName { get; private set; }
        public TextView ProductPrice { get; private set; }

        public ProductViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            Image = itemView.FindViewById<ImageView>(Resource.Id.imageView);
            ProductName = itemView.FindViewById<TextView>(Resource.Id.NameTView);
            ProductPrice = itemView.FindViewById<TextView>(Resource.Id.PriceTView);

            itemView.Click += (object sender, EventArgs e) => listener(base.LayoutPosition);
        }
    }
}