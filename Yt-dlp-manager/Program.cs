using Yt_dlp_manager.Downloads;

namespace Yt_dlp_manager
{
    internal abstract class Program
    {
        public static readonly TextElement DlCountElement =
            new TextElement($"Download queue: {DownloadManager.DownloadQueueCount}", 30, 1);


        [STAThread]
        private static void Main()
        {
            Console.CursorVisible = false;
            CancellationTokenSource cts = new CancellationTokenSource();

            InputThread inputThread = new InputThread(cts.Token);
            ClipboardWatcher clipboardWatcher = new ClipboardWatcher(cts.Token);
            ClipboardWatcher.ClearClipboard();
            var codeElement = new TextElement("Code: Invalid", 2, 1);
            _ = new DownloadManager(cts.Token);

            long freeSpace = DriveInfo.GetDrives()[0].AvailableFreeSpace;
            int skippedY = 5;

            inputThread.ButtonPressed += (_, e) =>
            {
                if (e.Key == ConsoleKey.X)
                    cts.Cancel();
            };
            clipboardWatcher.ClipboardChanged += (_, e) =>
            {
                string? code = e.OldText.TryExtractLink();
                if (code == null)
                    return;

                long estSpace = DownloadManager.EstimateRequiredSpace(code);
                if (2.5f * estSpace > freeSpace)
                {
                    new TextElement($"{code} - Skipped due to insufficient space: {estSpace}", 2, skippedY++).Print();
                    return;
                }

                DownloadManager.EnqueueNewDownload(code);
                ClipboardWatcher.ClearClipboard();
                codeElement.Text = $"Code: {code}";
            };

            while (!cts.Token.IsCancellationRequested)
            {
                Thread.Sleep(100);
            }
        }
    }
}
