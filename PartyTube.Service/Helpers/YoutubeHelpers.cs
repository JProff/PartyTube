using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace PartyTube.Service.Helpers
{
    public class YoutubeHelpers
    {
        private const string YoutubeIdPattern =
            ".*(?:youtu.be\\/|v\\/|u\\/\\w\\/|embed\\/|watch\\?.*v=)([^#\\&\\?]*).*";

        [CanBeNull]
        public virtual string GetIdFromUrl([CanBeNull] string url)
        {
            if (url == null) return null;
            if (string.IsNullOrWhiteSpace(url)) return string.Empty;

            var match = Regex.Match(url, YoutubeIdPattern);
            return !match.Success ? url : match.Groups[1].ToString();
        }
    }
}