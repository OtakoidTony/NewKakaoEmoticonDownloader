using System;
using System.Collections.Generic;
using System.Windows.Forms;
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

        public List<SearchedEmoticonInfo> searchResult;
        public int selectedIndex = -1;

        private string TitleUrl;

        private void whenSelect()
        {
            if (listView1.SelectedItems.Count == 1)
            {
                ListView.SelectedListViewItemCollection items = listView1.SelectedItems;
                ListViewItem lvItem = items[0];
                selectedIndex = listView1.Items.IndexOf(lvItem);
                pictureBox1.Load(searchResult[selectedIndex].Thumbnail);
                TitleUrl = searchResult[selectedIndex].TitleUrl;
            }
        }

        private void printListView()
        {
            searchResult = KakaoEmoticonCrawler.Search(textBox1.Text);
            listView1.Items.Clear();
            foreach (var i in searchResult)
            {
                String[] aa = {i.Title, i.Author};
                ListViewItem newitem = new ListViewItem(aa);
                listView1.Items.Add(newitem);
            }
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            whenSelect();
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

        private void button1_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                ListView.SelectedListViewItemCollection items = listView1.SelectedItems;
                ListViewItem lvItem = items[0];
                string title = lvItem.SubItems[0].Text;
                string folderPath = Application.StartupPath + "\\" + title;

                string[] result = KakaoEmoticonCrawler.GetThumbUrls(TitleUrl);
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
                    WebClient webClient = new WebClient();
                    
                    webClient.DownloadFileAsync(new Uri(result[i]), folderPath + "\\" + (i + ".png"));
                    webClient.DownloadFileCompleted += (s1, e1) =>
                    {
                        progressBar1.PerformStep();
                    };
                }
            }
        }
    }
}