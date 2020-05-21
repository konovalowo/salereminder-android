using System;

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