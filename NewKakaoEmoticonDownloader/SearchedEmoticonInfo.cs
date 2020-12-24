using System;

namespace NewKakaoEmoticonDownloader
{
    public class SearchedEmoticonInfo
    {
        public string Title;
        public string Author;

        public string Thumbnail;

        public string TitleUrl;

        public SearchedEmoticonInfo(string title, string author, string thumbnail, string titleUrl)
        {
            Title = title;
            Author = author;
            Thumbnail = thumbnail;
            TitleUrl = titleUrl;
        }
    }
}