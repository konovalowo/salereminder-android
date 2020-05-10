using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace ListWithJson.Models
{
    public class ServerResponse<T>
    {
        public T Value { get; }

        public HttpStatusCode StatusCode { get; }

        public ServerResponse(T value, HttpStatusCode statusCode)
        {
            Value = value;
            StatusCode = statusCode;
        }
    }
}