namespace Yt_dlp_manager
{
    public sealed class InputThread
    {
        public event EventHandler<ButtonPressedEventArgs>? ButtonPressed;

        private readonly CancellationToken token;

        public InputThread(CancellationToken t)
        {
            token = t;
            var thread = new Thread(readKey);
            thread.Start();
        }

        private void OnButtonPressed(ButtonPressedEventArgs e)
        {
            ButtonPressed?.Invoke(this, e);
        }

        private void readKey()
        {
            while (!token.IsCancellationRequested)
            {
                var key = Console.ReadKey(true).Key;
                OnButtonPressed(new ButtonPressedEventArgs(key));
            }
        }
    }

    public class ButtonPressedEventArgs(ConsoleKey key) : EventArgs
    {
        public readonly ConsoleKey Key = key;
    }
}
