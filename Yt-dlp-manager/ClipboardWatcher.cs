namespace Yt_dlp_manager
{
    public class ClipboardWatcher
    {
        private readonly CancellationToken token;

        public ClipboardWatcher(CancellationToken t)
        {
            token = t;

            var thread = new Thread(watchClipboard);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        [STAThread]
        private void watchClipboard()
        {
            string clipboardText = "";
            while (!token.IsCancellationRequested)
            {
                Thread.Sleep(100);

                string text = Clipboard.GetText();
                if (text == clipboardText || text == string.Empty)
                    continue;

                clipboardText = text;
                OnClipboardChanged(clipboardText);
            }
        }

        public static void ClearClipboard() => Clipboard.Clear();

        public event EventHandler<TextChangedEventArgs>? ClipboardChanged;

        private void OnClipboardChanged(string text)
        {
            ClipboardChanged?.Invoke(this, new TextChangedEventArgs(text));
        }
    }

    public class TextChangedEventArgs(string oldText) : EventArgs
    {
        public readonly string OldText = oldText;
    }
}
