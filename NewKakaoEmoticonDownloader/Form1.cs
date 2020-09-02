using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RestSharp;
using Newtonsoft.Json.Linq;
using HtmlAgilityPack;
using System.IO;
using System.Net;

namespace NewKakaoEmoticonDownloader
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Form1_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }
        public JToken searchResult;
        public int selectedIndex = -1;

        private void whenSelect()
        {
            if (listView1.SelectedItems.Count == 1)
            {
                ListView.SelectedListViewItemCollection items = listView1.SelectedItems;
                ListViewItem lvItem = items[0];
                selectedIndex = listView1.Items.IndexOf(lvItem);
                pictureBox1.Load(searchResult[selectedIndex]["titleDetailUrl"].ToString());
            }
        }
        
        private void printListView()
        {
            searchResult = KakaoEmoticon.Search(textBox1.Text)["items"];
            foreach (var i in searchResult)
            {
                String[] aa = { (String)i["title"], (String)i["artist"], (String)i["encoded"] };
                ListViewItem newitem = new ListViewItem(aa);
                listView1.Items.Add(newitem);
            }
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            whenSelect();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            printListView();
        }



        private void listView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                whenSelect();
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                printListView();
            }
            
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private bool DownloadRemoteImageFile(string uri, string fileName)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                bool bImage = response.ContentType.StartsWith("image",
                    StringComparison.OrdinalIgnoreCase);
                if ((response.StatusCode == HttpStatusCode.OK ||
                    response.StatusCode == HttpStatusCode.Moved ||
                    response.StatusCode == HttpStatusCode.Redirect) &&
                    bImage)
                {
                    using (Stream inputStream = response.GetResponseStream())
                    using (Stream outputStream = File.OpenWrite(fileName))
                    {
                        byte[] buffer = new byte[4096];
                        int bytesRead;
                        do
                        {
                            bytesRead = inputStream.Read(buffer, 0, buffer.Length);
                            outputStream.Write(buffer, 0, bytesRead);
                        } while (bytesRead != 0);
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                using (WebClient webClient = new WebClient())
                {
                    ListView.SelectedListViewItemCollection items = listView1.SelectedItems;
                    ListViewItem lvItem = items[0];
                    string title = lvItem.SubItems[0].Text;
                    string itemcode = lvItem.SubItems[2].Text;
                    string folderPath = Application.StartupPath + "\\" + title;

                    string[] result = KakaoEmoticon.GetThumbUrl(itemcode);
                    int size = result.Length;
                    DirectoryInfo di = new DirectoryInfo(folderPath);
                    if (di.Exists == false)
                    {
                        di.Create();
                    }
                    progressBar1.Style = ProgressBarStyle.Continuous;
                    progressBar1.Minimum = 0;
                    progressBar1.Maximum = size;
                    progressBar1.Step = 1;
                    progressBar1.Value = 0;
                    for (int i = 0; i < size; i++)
                    {
                        webClient.DownloadFile(result[i], folderPath + "\\" + (i+".png"));
                        progressBar1.PerformStep();
                    }
                }
            }
        }
    }
    public class KakaoEmoticon
    {
        public static JObject Search(string searchText)
        {
            var client = new RestClient("https://e.kakao.com/search?q=" + searchText);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            var response = client.Execute(request);
            var httpDoc = new HtmlAgilityPack.HtmlDocument();
            httpDoc.LoadHtml(response.Content);
            var nodeCol = httpDoc.DocumentNode.SelectNodes("//*[@id=\"mArticle\"]/div[2]");
            var searchResult = nodeCol[0].Attributes["data-react-props"].Value;
            return JObject.Parse(searchResult.Replace("&quot;", "\""));
        }

        public static JObject Info(string titleUrl)
        {
            var client = new RestClient("https://e.kakao.com/t/" + titleUrl);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("DNT", "1");
            request.AddHeader("Upgrade-Insecure-Requests", "1");
            request.AddHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
            IRestResponse response = client.Execute(request);
            var httpDoc = new HtmlAgilityPack.HtmlDocument();
            httpDoc.LoadHtml(response.Content);
            var nodeCol = httpDoc.DocumentNode.SelectNodes("//*[@id=\"dkWrap\"]/div[2]");
            return JObject.Parse(nodeCol[0].Attributes["data-react-props"].Value.Replace("&quot;", "\""));
        }

        public static string[] GetThumbUrl(string itemCode)
        {
            var client = new RestClient("https://e.kakao.com/detail/thumb_url?item_code=" + itemCode);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            JObject res = JObject.Parse(response.Content);
            return JArray.FromObject(res["body"]).ToObject<string[]>();
        }
    }
}
