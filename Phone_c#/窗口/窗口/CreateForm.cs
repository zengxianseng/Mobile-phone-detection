using CsvHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace 窗口
{
    public partial class CreateForm : Form
    {
        public delegate void RefreshTableDelegate();
        public event RefreshTableDelegate RefreshTableEvent;
        private static double l = 0;
        public int a = 0;
        public CreateForm()
        {
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
        private void Judgment_function()
        {
            float ubf = -1, lrf = -1, lf = -1, wf = -1;
            if (string.IsNullOrEmpty(nameBox.Text))
            {
                MessageBox.Show("配置名为空！");
                a = 1;
                return;
            }
            else if (string.IsNullOrEmpty(ubBox.Text))
            {
                MessageBox.Show("上下间距为空！");
                a = 1;
                return;
            }
            else if (string.IsNullOrEmpty(lengthBox.Text))
            {
                a = 1;
                MessageBox.Show("长度为空！");
                return;
            }
            else if (string.IsNullOrEmpty(lrBox.Text))
            {
                MessageBox.Show("左右间距为空！");
                a = 1;
                return;
            }
            else if (string.IsNullOrEmpty(widthBox.Text))
            {
                a = 1;
                MessageBox.Show("宽度为空");
                return;
            }
            else if (string.IsNullOrEmpty(colorBox.Text))
            {
                a = 1;
                MessageBox.Show("颜色为空");
                return;
            }


            try
            {
                ubf = float.Parse(ubBox.Text);
                if (ubf <= 0)
                {
                    a = 1;
                    MessageBox.Show("上下间距非正数！");
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("上下间距存在非法字符");
                a = 1;
                return;
            }

            try
            {
                lf = float.Parse(lengthBox.Text);
                if (lf <= 0)
                {
                    a = 1;
                    MessageBox.Show("长度数据非正数！");
                    return;
                }
            }
            catch (Exception e)
            {
                a = 1;
                MessageBox.Show("长度数据存在非法字符");
                return;
            }
            if (ubf >= lf)
            {
                MessageBox.Show("上下边距大于等于产品长度");
                a = 1;
                return;
            }


            try
            {
                lrf = float.Parse(lrBox.Text);
                if (lrf <= 0)
                {
                    a = 1;
                    MessageBox.Show("左右间距非正数！");
                    return;
                }
            }
            catch (Exception e)
            {
                a = 1;
                MessageBox.Show("左右间距数据出现非法字符");
                return;
            }

            try
            {
                wf = float.Parse(widthBox.Text);
                if (wf <= 0)
                {
                    a = 1;
                    MessageBox.Show("宽度数据非正数！");
                    return;
                }
            }
            catch (Exception e)
            {
                a = 1;
                MessageBox.Show("宽度数据存在非法字符");
                return;
            }
            if (lrf >= wf)
            {
                MessageBox.Show("左右间距大于等于宽度");
                a = 1;
                return;
            }
        }//判断是否合理

        private void Action()
        {
            Judgment_function();
            string templateFilePath = "./Template.csv";
            string outputFolderPath = @"./CameraConfiguration"; // 指定输出文件夹路径
            string outputFileName = string.Concat(nameBox.Text, ".csv"); // 指定配置名
            string outputFilePath = Path.Combine(outputFolderPath, outputFileName);

            // 确保模板文件存在
            if (!File.Exists(templateFilePath))
            {
                MessageBox.Show("模板文件不存在。");
                return;
            }

            // 读取模板文件内容
            string[] lines = File.ReadAllLines(templateFilePath);

            // 确保至少有10行
            if (lines.Length < 10)
            {
                Array.Resize(ref lines, 10);
            }

            // 在第9行添加列名
            lines[8] = "上下边距(mm),左右边距(mm),长度(mm),宽度(mm),颜色(mm)";

            // 获取第10行的内容（如果有）
            string[] columns = lines[9]?.Split(',') ?? new string[0];

            // 确保至少有5列
            if (columns.Length < 5)
            {
                Array.Resize(ref columns, 5);
            }

            // 写入TextBox中的内容
            columns[0] = ubBox.Text;
            columns[1] = lrBox.Text;
            columns[2] = lengthBox.Text;
            l = double.Parse(lengthBox.Text);
            columns[3] = widthBox.Text;
            columns[4] = colorBox.Text;

            // 更新第13行
            lines[9] = string.Join(",", columns);

            // 确保输出文件夹存在
            if (!Directory.Exists(outputFolderPath))
            {
                Directory.CreateDirectory(outputFolderPath);
            }

            // 写入文件
            File.WriteAllLines(outputFilePath, lines);
            this.Close();
            MessageBox.Show("成功创建配置文件！");
        }//手机壳信息，包括相机数量，手机壳的信息等
        private void CreateDocument()
        {
            string path = @"./right_configure/" + nameBox.Text; // 这里的路径和文件夹名可以自定义
            string absolutePath = Path.GetFullPath(path);//获得绝对路径
            try
            {
                // 判断路径是否存在，不存在则创建  
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    //camera
                    string cameradocument = path + "\\camera";
                    Directory.CreateDirectory(cameradocument);
                    string cameradocument1 = cameradocument + "\\XZ";
                    Directory.CreateDirectory(cameradocument1);
                    string cameradocument2 = cameradocument + "\\MZ";
                    Directory.CreateDirectory(cameradocument2);
                    string aike2 = cameradocument1 + "\\aike2";
                    Directory.CreateDirectory(aike2);
                    string I3 = cameradocument2 + "\\I3";
                    Directory.CreateDirectory(I3);
                    string I4 = cameradocument2 + "\\I4";
                    Directory.CreateDirectory(I4);

                    string camerafile = aike2 + "\\file";
                    Directory.CreateDirectory(camerafile);
                    //detect
                    string detectdocument = path + "\\detect";
                    Directory.CreateDirectory(detectdocument);
                    string detectdocument1 = detectdocument + "\\XZ";
                    Directory.CreateDirectory(detectdocument1);
                    string detectdocument2 = detectdocument + "\\MZ";
                    Directory.CreateDirectory(detectdocument2);
                    string aike2_ = detectdocument1 + "\\aike2";
                    Directory.CreateDirectory(aike2_);
                    string I3_ = detectdocument2 + "\\I3";
                    Directory.CreateDirectory(I3);
                    string I4_ = detectdocument2 + "\\I4";
                    Directory.CreateDirectory(I4);

                    string classaike2 = aike2_ + "\\Class";
                    string classI3 = I3_ + "\\Class";
                    string classI4 = I4_ + "\\Class";
                    Directory.CreateDirectory(classaike2);
                    Directory.CreateDirectory(classI3);
                    Directory.CreateDirectory(classI4);

                    string weightaike2 = aike2_ + "\\Weight";
                    string weightI3 = I3_ + "\\Weight";
                    string weightI4 = I4_ + "\\Weight";
                    Directory.CreateDirectory(weightaike2);
                    Directory.CreateDirectory(weightI3);
                    Directory.CreateDirectory(weightI4);
                    //configure

                    string configpath = path + "\\right_configure.csv";
                    string template = "configuretemplate.csv";
                    string[] lines = File.ReadAllLines(template);
                    File.WriteAllLines(configpath, lines);
                    Console.WriteLine("文件夹创建成功！");
                }
                else
                {
                    Console.WriteLine("文件夹已存在。");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("创建文件夹时出错：" + e.Message);
            }
        }//创建相机配置文件
        private void c()
        {
            string f = "./Peizhi/"+nameBox.Text+".csv";
            string[] a = { "NULL", "NULL" };
            using (StreamWriter stream = new StreamWriter(f))
            {
                for(int i = 0; i < 6; i++)
                {
                    stream.WriteLine(string.Join(",", a));
                }
                stream.WriteLine("NULL");
                stream.WriteLine("NULL");
            }
            Console.WriteLine("配置文件创建成功");
        }//创建配置文件
        private void b()
        {
            string f = "quanzhong\\" + nameBox.Text + ".csv";
            string[] a = { "NULL", "NULL", "NULL" };
            using (StreamWriter stream = new StreamWriter(f))
            {
                for (int i = 0; i < 8; i++)
                {
                    stream.WriteLine(string.Join(",", a));
                }
            }
            Console.WriteLine("权重文件创建成功");
        }//创建权重文件
        private void button1_Click_1(object sender, EventArgs e)
        {
            Judgment_function();
            if (a != 1)
            {
                Action();
                c();
                b();
                CreateDocument();
                RefreshTableEvent?.Invoke();
                /*string fp = @"D:\Quexian\"+ nameBox.Text;
                try
                {
                    DirectoryInfo di = Directory.CreateDirectory(fp);
                    string yT = fp + "\\yuantu";
                    string qX = fp + "\\quexian";
                    try
                    {
                        DirectoryInfo dy = Directory.CreateDirectory(yT);
                        DirectoryInfo dm = Directory.CreateDirectory(qX);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }*/
            }
            a = 0;
        }
        public static double Length()
        {
            return l;
        }
    }
}

