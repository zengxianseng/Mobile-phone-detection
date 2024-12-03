using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace 窗口
{
    internal class Table
    {
        public string folderPath;
        ListView listView1;
        public Table(ListView list)
        {
            this.listView1 = list;
            LoadCsvFiles();
        }

        private void LoadCsvFiles()
        {
            folderPath = "./CameraConfiguration";
            string[] csvFiles = Directory.GetFiles(folderPath, "*.csv");

            // 根据文件的创建时间对 csvFiles 进行排序
            Array.Sort(csvFiles, (x, y) => File.GetCreationTime(x).CompareTo(File.GetCreationTime(y)));

            listView1.Items.Clear();

            foreach (string file in csvFiles)
            {
                string fileName = Path.GetFileName(file);
                string csvPath = Path.Combine(folderPath, fileName);
                fileName = fileName.Substring(0, fileName.Length - 4);
                DateTime creationTime = File.GetCreationTime(file);
                int a = ReadSpecificRowUntilEmptyCell(csvPath, 1);
                string UB = ReadData(csvPath, 1);
                string LR = ReadData(csvPath, 2);
                string L = ReadData(csvPath, 3);
                string W = ReadData(csvPath, 4);
                string C = ReadData(csvPath, 5);
                ListViewItem item = new ListViewItem(new[] { fileName, creationTime.ToString("yyyy-MM-dd HH:mm:ss"), a.ToString(), UB, LR, L, W, C });
                listView1.Items.Add(item);
            }
        }

        public int ReadSpecificRowUntilEmptyCell(string csvFilePath, int rowNumber)
        {
            int a = 0;
            try
            {
                using (StreamReader reader = new StreamReader(csvFilePath))
                {
                    int currentLineNumber = 0;
                    string line;

                    while ((line = reader.ReadLine()) != null)
                    {
                        currentLineNumber++;

                        if (currentLineNumber == rowNumber)
                        {
                            string[] fields = line.Split(',');
                            foreach (string field in fields)
                            {
                                if (string.IsNullOrEmpty(field) || field == "NULL")
                                {
                                    //Console.WriteLine("Encountered an empty cell. Terminating read.");
                                    continue;
                                }
                                a++;
                            }
                            return a;
                        }
                    }

                    Console.WriteLine($"The CSV file does not have a row number {rowNumber}.");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading CSV file: {ex.Message}");
            }
            return a;
        }

        public string ReadData(string csvFilePath, int l)
        {
            l = l - 1;
            string[] lines = File.ReadAllLines(csvFilePath);

            if (lines.Length >= 10)
            {
                string[] columns = lines[9].Split(',');
                if (columns.Length > 0)
                {
                    return columns[l];
                }
                else
                {
                    Console.WriteLine("The 10th line is empty.");
                }
            }
            return null;
        }

        private class ListViewItemComparer : IComparer
        {
            private int col;
            private SortOrder order;

            public ListViewItemComparer(int column, SortOrder order)
            {
                col = column;
                this.order = order;
            }

            public int Compare(object x, object y)
            {
                ListViewItem itemX = x as ListViewItem;
                ListViewItem itemY = y as ListViewItem;

                int result = String.Compare(itemX.SubItems[col].Text, itemY.SubItems[col].Text);

                if (order == SortOrder.Descending)
                {
                    result = -result;
                }

                return result;
            }
        }
        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // 获取当前排序列的索引
            int sortColumnIndex = e.Column;

            // 获取当前排序顺序
            bool isAscending = listView1.Sorting == SortOrder.Ascending;

            // 设置排序顺序
            listView1.Sorting = isAscending ? SortOrder.Descending : SortOrder.Ascending;

            // 创建一个比较器
            listView1.ListViewItemSorter = new ListViewItemComparer(sortColumnIndex, listView1.Sorting);

            // 对 ListView 进行排序
            listView1.Sort();
        }

        public void CopySelectedCsvToTrans()
        {
            if (listView1.SelectedItems.Count > 0)
            {
                ListViewItem selectedItem = listView1.SelectedItems[0];
                string filePath = Path.Combine(folderPath, selectedItem.Text + ".csv");
                // 读取选中的CSV文件内容
                string[] lines = File.ReadAllLines(filePath);

                // 将内容写入到 trans.csv 文件
/*                string transFilePath = Path.Combine(folderPath, "Train.csv");
                File.WriteAllLines(transFilePath, lines);*/
            }
            else
            {
                MessageBox.Show("请选择一个配置");
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            CreateForm cF = new CreateForm();
            cF.StartPosition = FormStartPosition.CenterScreen;
            cF.ShowDialog();
        }
        private void button4_Click(object sender, EventArgs e)
        {
            LoadCsvFiles();
        }
    }
}
