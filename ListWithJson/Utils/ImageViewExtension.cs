using Android.Graphics;
using Android.Util;
using Android.Widget;
using System;
using System.Net.Http;

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