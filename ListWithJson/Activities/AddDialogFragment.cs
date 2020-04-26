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
    public class AddDialogFragment : DialogFragment
    {
        public event EventHandler<AddButtonEventArgs> onAddButtonClicked;

        EditText editText;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.add_dialog_fragment, container, false);
            editText = view.FindViewById<EditText>(Resource.Id.editText1);

            view.FindViewById<Button>(Resource.Id.button1).Click += (sender, e) =>
            {
                onAddButtonClicked?.Invoke(this, new AddButtonEventArgs(editText.Text));
                Dismiss();
            };
            return view;
        }
    }
}