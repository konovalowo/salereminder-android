using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace ListWithJson
{
    public static class ImageViewExtension
    {
        const string logTag = "ImageFromUrlAsync";

        public async static void ImageFromUrlAsync(this ImageView iv,  string url)
        {
            Bitmap imageBitmap = null;
            url = url.Trim('/');
            if (url.IndexOf("https://") == -1)
            {
                url = "https://" + url;
            }

            try
            {
                using (var client = new HttpClient())
                {
                    var imageBytes = await client.GetByteArrayAsync(new Uri(url));
                    if (imageBytes != null && imageBytes.Length > 0)
                    {
                        imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                    }
                    iv.SetImageBitmap(imageBitmap);
                }
            }
            catch (Java.Net.UnknownHostException e)
            {
                Log.Error(logTag, e.Message);
            }
            catch (NullReferenceException e)
            {
                Log.Error(logTag, "Null image url: " + e.Message);
            }
            catch (Exception e)
            {
                Log.Error(logTag, e.Message);
            }

        }
    }

}