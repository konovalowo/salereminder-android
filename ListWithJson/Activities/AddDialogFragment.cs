using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;

namespace ListWithJson
{
    public class AddDialogFragment : DialogFragment
    {
        public event EventHandler<AddDialogEventArgs> onAddButtonClicked;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.add_dialog_fragment, container, false);
            EditText editText = view.FindViewById<EditText>(Resource.Id.editText1);

            view.FindViewById<Button>(Resource.Id.button1).Click += (sender, e) =>
            {
                onAddButtonClicked?.Invoke(this, new AddDialogEventArgs(editText.Text));
                Dismiss();
            };
            return view;
        }
    }
}