using System.Diagnostics;

namespace Yt_dlp_manager.Downloads
{
    public class DownloadManager
    {
        private const int max_concurrent_downloads = 1;
        private readonly CancellationToken token;
        private static readonly Queue<string> downloadQueue = new Queue<string>();
        private static int activeDownloads => OccupiedSlot.Count(s => s);
        public static int DownloadQueueCount => downloadQueue.Count + activeDownloads;
        public static readonly bool[] OccupiedSlot = new bool[max_concurrent_downloads];


        public DownloadManager(CancellationToken t)
        {
            token = t;
            var thread = new Thread(downloadHandler);
            thread.Start();
        }

        private void downloadHandler()
        {
            while (!token.IsCancellationRequested)
            {
                if (downloadQueue.Count == 0 || activeDownloads == max_concurrent_downloads)
                {
                    Thread.Sleep(1000);
                    continue;
                }

                int slotIndex = OccupiedSlot.Index().First(s => !s.Item).Index;
                string code = downloadQueue.Dequeue();
                var dlElement = new DownloadElement(code, 2, 2, slotIndex);
                dlElement.StartDownload();
            }
        }

        public static void EnqueueNewDownload(string code)
        {
            Program.DlCountElement.Text = $"Download queue: {DownloadQueueCount}";
            downloadQueue.Enqueue(code);
        }

        public static long EstimateRequiredSpace(string code)
        {
            string format;
            if (code.Contains("twitch"))
                format = "1080p60";
            else //youtube
                format = "bv[height<=1080]+ba";

            var proc = new Process()
            {
                StartInfo =
                {
                    FileName = "yt-dlp",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    Arguments = $"--ignore-config -f {format} --print \"%(duration)s,%(tbr)d\" -- \"{code}\"",
                },
            };

            proc.Start();

            //Expected output duration,bitrate - '123123,123'
            string outPut = proc.StandardOutput.ReadToEnd();
            string[] split = outPut.Split(',');
            if (split.Length != 2)
                throw new InvalidOperationException($"Failed to get filesize for: {code}");

            if (!long.TryParse(split[0], out long duration))
                throw new InvalidOperationException($"Failed to parse {split[0]} as long");
            if (!long.TryParse(split[1], out long bitrate))
                throw new InvalidOperationException($"Failed to parse {split[1]} as long");

            const long bitrate_to_bytes = 1000 / 8;
            return duration * bitrate * bitrate_to_bytes;
        }
    }
}
