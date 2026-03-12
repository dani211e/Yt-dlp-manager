namespace Yt_dlp_manager
{
    public static class StringExtensions
    {
        public static string? TryExtractLink(this string s)
        {
            if (!s.StartsWith("https://"))
                return null; // Could throw but we silly
            int index = s.IndexOf('/', "https://".Length);
            if (index == -1)
                return null;

            string domain = s[8..index];
            return domain switch
            {
                "www.youtube.com" => tryExtractYoutube(s),
                "www.youtu.be"    => tryExtractYoutube(s),
                "www.twitch.tv"   => tryExtractTwitch(s),
                _                 => null,
            };
        }

        private static string? tryExtractYoutube(string s)
        {
            //youtube.com || youtu.be
            if (!s.Contains("www.youtu"))
                return null;

            // https://www.youtube.com/watch?v=ncxRAvchBpQ&t=933s
            string watch = s.Split('/').Last();
            int first = watch.IndexOf('=');
            int last = watch.IndexOf('&');
            if (last == -1)
                return watch[(first + 1)..];

            return watch.Substring(first + 1, last - first - 1);
        }

        private static string? tryExtractTwitch(string s) => s; //Pass through unchanged
    }
}
