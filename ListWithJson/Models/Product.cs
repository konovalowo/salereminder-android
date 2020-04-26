using System;
using System.Collections.Generic;
using System.Globalization;
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
    [Serializable]
    public class Product
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public string Currency { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }

        public CultureInfo GetCulture()
        {
            var culture = CultureInfo.GetCultures(CultureTypes.SpecificCultures).First(culture =>
            {
                try
                {
                    var regionInfo = new RegionInfo(culture.Name);
                    if (regionInfo.ISOCurrencySymbol == Currency)
                    {
                        return true;
                    }
                    return false;
                }
                catch
                {
                    return false;
                }
            });
            return culture;
        }
    }
}