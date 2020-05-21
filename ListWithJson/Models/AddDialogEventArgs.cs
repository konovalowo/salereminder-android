namespace ListWithJson
{
    public class AddDialogEventArgs
    {
        public string Text { get; set; }

        public AddDialogEventArgs(string text)
        {
            Text = text;
        }
    }
}