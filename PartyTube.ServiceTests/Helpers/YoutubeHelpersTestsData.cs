using System.Collections.Generic;

namespace PartyTube.ServiceTests.Helpers
{
    public static class YoutubeHelpersTestsData
    {
        private static IEnumerable<object[]> GetRawUrls()
        {
            yield return new object[]
            {
                "www.youtube.com/user/Scobleizer#p/u/1/1p3vcRhsYGo", "1p3vcRhsYGo"
            };
            yield return new object[]
            {
                "www.youtube.com/watch?v=2cKZDdG9FTKY&feature=channel", "2cKZDdG9FTKY"
            };
            yield return new object[]
            {
                "www.youtube.com/watch?v=3yZ-K7nCVnBI&playnext_from=TL&videos=osPknwzXEas&feature=sub", "3yZ-K7nCVnBI"
            };
            yield return new object[]
            {
                "www.youtube.com/user/SilkRoadTheatre#p/a/u/2/4dwqZw0j_jY", "4dwqZw0j_jY"
            };
            yield return new object[]
            {
                "youtu.be/5dwqZw0j_jY", "5dwqZw0j_jY"
            };
            yield return new object[]
            {
                "www.youtube.com/watch?v=6dwqZw0j_jY&feature=youtu.be", "6dwqZw0j_jY"
            };
            yield return new object[]
            {
                "youtu.be/7afa-5HQHiAs", "7afa-5HQHiAs"
            };
            yield return new object[]
            {
                "www.youtube.com/user/Scobleizer#p/u/1/8p3vcRhsYGo?rel=0", "8p3vcRhsYGo"
            };
            yield return new object[]
            {
                "www.youtube.com/watch?v=9cKZDdG9FTKY&feature=channel", "9cKZDdG9FTKY"
            };
            yield return new object[]
            {
                "www.youtube.com/watch?v=10yZ-K7nCVnBI&playnext_from=TL&videos=osPknwzXEas&feature=sub", "10yZ-K7nCVnBI"
            };
            yield return new object[]
            {
                "www.youtube.com/embed/11nas1rJpm7wY?rel=0", "11nas1rJpm7wY"
            };
            yield return new object[]
            {
                "www.youtube.com/watch?v=12peFZbP64dsU", "12peFZbP64dsU"
            };
            yield return new object[]
            {
                "youtube.com/v/13dQw4w9WgXcQ?feature=youtube_gdata_player", "13dQw4w9WgXcQ"
            };
            yield return new object[]
            {
                "www.youtube.com/watch?v=14dQw4w9WgXcQ&feature=youtube_gdata_player", "14dQw4w9WgXcQ"
            };
            yield return new object[]
            {
                "youtube.com/watch?v=15dQw4w9WgXcQ&feature=youtube_gdata_player", "15dQw4w9WgXcQ"
            };
            yield return new object[]
            {
                "youtu.be/16dQw4w9WgXcQ?feature=youtube_gdata_player", "16dQw4w9WgXcQ"
            };
            yield return new object[]
            {
                "www.youtube.com/watch?v=17m3lF2qEA2cw&index=17&list=FLrMx82uqFqH19oV2XPa0btg&t=0s", "17m3lF2qEA2cw"
            };
        }

        public static IEnumerable<object[]> GetIdFromUrlData()
        {
            foreach (var raw in GetRawUrls())
            {
                yield return raw;
            }

            foreach (var raw in GetRawUrls())
            {
                raw[0] = string.Concat("http://", raw[0]);
                yield return raw;
            }

            foreach (var raw in GetRawUrls())
            {
                raw[0] = string.Concat("https://", raw[0]);
                yield return raw;
            }

            yield return new object[]
            {
                "18dQw4w9WgXcQ", "18dQw4w9WgXcQ"
            };

            yield return new object[]
            {
                "www.youtube.com", "www.youtube.com"
            };
        }
    }
}