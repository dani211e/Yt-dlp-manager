using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Yt_dlp_manager
{
    public class TextElement
    {
        private static readonly Lock printLock = new Lock();

        public string? Text
        {
            get;
            set
            {
                //OnTextChanged(field);
                int oldLen = field?.Length ?? 0;
                field = value;
                Print(oldLen);
            }
        }


        private readonly int x;
        private readonly int y;
        public int Width => Text!.Length;

        public TextElement(string text, int x, int y)
        {
            this.x = x;
            this.y = y;
            //TextChanged += (_, _) => Print();
            Text = text;
            Print();
        }

        public void Print(int postPadding = 0)
        {
            lock (printLock)
            {
                Console.SetCursorPosition(x, y);
                Console.Write(Text);
                string padding = new string(' ', postPadding);
                Console.Write(padding);
            }
        }
    }

    /*public event EventHandler<TextChangedEventArgs>? TextChanged;

    private void OnTextChanged(string? oldText = "")
    {
        TextChanged?.Invoke(this, new TextChangedEventArgs(oldText ?? string.Empty));
    }*/
}

