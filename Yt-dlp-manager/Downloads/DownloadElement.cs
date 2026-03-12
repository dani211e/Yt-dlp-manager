using System.Diagnostics;

namespace Yt_dlp_manager.Downloads
{
    public sealed class DownloadElement : TextElement
    {
        private readonly Process process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "yt-dlp",
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
            },
            EnableRaisingEvents = true,
        };

        private readonly string code;
        private readonly int slotIndex;

        public DownloadElement(string code, int x, int y, int slotIndex) : base(string.Empty, x, y + slotIndex)
        {
            this.code = code;
            this.slotIndex = slotIndex;

            Debug.Assert(!DownloadManager.OccupiedSlot[slotIndex]);
            DownloadManager.OccupiedSlot[slotIndex] = true;

            //process.StartInfo.Arguments = $"--simulate -- {code}";
            if (code.Contains("twitch"))
                process.StartInfo.Arguments = $"-f 1080p60 -- {code}";
            else
                process.StartInfo.Arguments = $"-- {code}";

            process.OutputDataReceived += handleOutputData;
            process.Exited += finish;
        }

        private void finish(object? o, EventArgs args)
        {
            Text = string.Empty;
            DownloadManager.OccupiedSlot[slotIndex] = false;
            Program.DlCountElement.Text = $"Download queue: {DownloadManager.DownloadQueueCount}";
        }

        private void handleOutputData(object sendingProcess, DataReceivedEventArgs outLine)
        {
            if (string.IsNullOrEmpty(outLine.Data))
                return;

            int len = outLine.Data.Length;
            Text = code + " - " + outLine.Data[..Math.Min(len, 80)];
        }

        public void StartDownload()
        {
            Text = $"{code} - Started!";
            process.Start();
            process.BeginOutputReadLine();
            Print();
        }
    }
}
