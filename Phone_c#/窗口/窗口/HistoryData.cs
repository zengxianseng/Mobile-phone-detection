using System;
using System.IO;
using System.Windows.Forms;

namespace 窗口
{
    internal class HistoryData
    {
        public string folderPath;
        ListView listView1;

        public HistoryData(ListView list)
        {
            this.listView1 = list;
            LoadFiles();
        }

        private void LoadFiles()
        {
            folderPath = "./history";
            string[] txtFiles = Directory.GetFiles(folderPath, "*.txt");

            // 根据文件的创建时间对 csvFiles 进行排序
            Array.Sort(txtFiles, (x, y) => File.GetCreationTime(x).CompareTo(File.GetCreationTime(y)));
            Array.Reverse(txtFiles);
            listView1.Items.Clear();
            foreach (string file in txtFiles)
            {
                string fileName = Path.GetFileName(file);
                string txtPath = Path.Combine(folderPath, fileName);
                fileName = fileName.Substring(0, fileName.Length - 4);
                DateTime creationTime = File.GetCreationTime(file);
                // 读取TXT文件中的坐标数据
                string[] coordinates = ReadTxt(txtPath).Split(',');
                string oknumber = coordinates[0];
                string ngnumber = coordinates[1];
                ListViewItem item = new ListViewItem(new[] { creationTime.ToString("yyyy-MM-dd HH:mm"), oknumber, ngnumber });
                listView1.Items.Add(item);
            }
        }

        public string ReadTxt(string filePath)
        {
            string fileContent = "";
            using (var sr = new StreamReader(filePath))
            {
                fileContent = sr.ReadToEnd();
            }
            return fileContent;
        }
    }
}