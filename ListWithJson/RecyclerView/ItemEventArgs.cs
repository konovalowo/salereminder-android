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

namespace ListWithJson
{
    public class ItemEventArgs : EventArgs
    {
        public Product Product { get; }

        public ItemEventArgs(Product product)
        {
            Product = product;
        }
    }
}