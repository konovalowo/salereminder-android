using System;
using System.Globalization;
using System.Linq;

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
        public double Price { get; set; }
        public bool IsOnSale { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }

        public CultureInfo GetCulture()
        {
            CultureInfo culture;
            try
            {
                culture = CultureInfo.GetCultures(CultureTypes.SpecificCultures).First(culture =>
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
            }
            catch (InvalidOperationException e) // no match
            {
                culture = CultureInfo.CurrentCulture;
            }
            return culture;
        }

        public string GetWebsiteShort()
        {
            int startIndex = Url.IndexOf('.');
            int endIndex = Url.IndexOf('/', startIndex + 1); // //www.avc.ru/   -   //egg.com/
            if (Url.IndexOf('.', startIndex + 1, endIndex - startIndex) == -1)
            {
                startIndex = Url.IndexOf("//") + 1;
            }
            if (startIndex != -1 && endIndex != -1)
            {
                return Url.Substring(startIndex + 1, endIndex - startIndex - 1);
            }
            return null;
        }

        public string GetWebsite()
        {
            int dotIndex = Url.IndexOf('.');
            int endIndex = Url.IndexOf('/', dotIndex + 1);
            if (dotIndex != -1 && endIndex != -1)
            {
                return Url.Substring(0, endIndex);
            }
            return null;
        }
    }
}