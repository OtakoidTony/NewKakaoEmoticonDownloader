using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace NewKakaoEmoticonDownloader
{
    public class KakaoEmoticonCrawler
    {
        public static List<SearchedEmoticonInfo> Search(String query)
        {
            var client = new RestClient("https://e.kakao.com/api/v1/search?query=" + query);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            var response = client.Execute(request);
            var result = new List<SearchedEmoticonInfo>();
            foreach (var element in JArray.FromObject(JObject.Parse(response.Content)["result"]["content"]))
            {
                result.Add(new SearchedEmoticonInfo(
                        element["title"].ToString(),
                        element["artist"].ToString(),
                        element["titleDetailUrl"].ToString(),
                        element["titleUrl"].ToString()
                    )
                );
            }
            return result;
        }
        
        public static string[] GetThumbUrls(string titleUrl)
        {
            var client = new RestClient("https://e.kakao.com/api/v1/items/t/" + titleUrl);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            JObject res = JObject.Parse(response.Content);
            return JArray.FromObject(res["result"]["thumbnailUrls"]).ToObject<string[]>();
        }
    }
}