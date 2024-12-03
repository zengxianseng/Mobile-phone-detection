using EasyModbus;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ControllerDllCSharp.ClassLibControllerDll;
using static 窗口.CreateM;
using static System.Windows.Forms.LinkLabel;

namespace 窗口
{
    using ControllerHandle = Int64;
    public partial class CreateM : Form
    {
        ModbusClient _modbusClient;
        Label[] qz;
        Label p;
        int[] M;
        int[] id;
        Button button;
        Label label;
        Label[] delay;
        Label[] light;
        Button[] upButton;
        Button[] downButton;

        public CreateM(Label[] qz, Label p, int[] M, int[] id, Button button, Label label, Label[] delay, Label[] light, Button[] upButton, Button[] downButton)
        {
            _modbusClient = new ModbusClient("192.168.0.88", 502);
            _modbusClient.Connect();
            this.qz = qz;
            this.p = p;
            this.M = M;
            this.id = id;
            this.button = button;
            this.label = label;
            this.delay = delay;
            this.light = light;
            this.upButton = upButton;
            this.downButton = downButton;
            this.StartPosition = FormStartPosition.CenterScreen;
            InitializeComponent();
        }
        public CreateM()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("相机名称为空");
                return;
            }
            else
            {
                Sellect();
                Connect();
                label.Text = textBox1.Text;
                button.Text = "-";
                button.ForeColor = Color.Red;
                this.Close();
            }
            /*using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "./configure\\camera";
                openFileDialog.Title = "请选择配置文件";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string fp = openFileDialog.FileName;
                    string[] lines = File.ReadAllLines("./Peizhi\\" + Form1.GainName() + ".csv");
                    lines[int.Parse(p.Name.Substring(2)) - 1] = fp;
                    File.WriteAllLines("./Peizhi\\" + Form1.GainName() + ".csv", lines);
                    p.Text = fp.Substring(8);
                    Console.WriteLine(fp);
                    Console.WriteLine(p.Name);
                }
            }*/
            /*string[][] r = ReadCsvFile("./quanzhong\\" + Form1.GainName() + ".csv");
            for(int i = 0; i < 3; i++)
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.InitialDirectory = "C:\\configure\\camera";
                    openFileDialog.Title = "请选择权重文件";
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string fp = openFileDialog.FileName;//获得选择的文件路径
                        r[int.Parse(p.Name.Substring(2)) - 1][i] = fp;
                        qz[i].Text = fp.Substring(8);
                        Console.WriteLine(fp);
                    }
                }
            }*/
            /*try
            {
                using (StreamWriter writer = new StreamWriter("./quanzhong\\" + Form1.GainName() + ".csv"))
                {
                    for (int i = 0; i < r.Length; i++)
                    {
                        writer.WriteLine(string.Join(",", r[i]));
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("写入 CSV 文件时出错: " + ex.Message);
            }*/
        }
        private string[][] ReadCsvFile(string filePath)
        {
            try
            {
                string[] lines = File.ReadAllLines(filePath);
                string[][] csvData = new string[lines.Length][];

                for (int i = 0; i < lines.Length; i++)
                {
                    csvData[i] = lines[i].Split(',');
                }

                return csvData;
            }
            catch (Exception ex)
            {
                MessageBox.Show("读取 CSV 文件时出错: " + ex.Message);
                return null;
            }
        }//读csv配置文件
        private void Sellect()
        {
            for(int i = 0; i < delay.Length; i++)
            {
                light[i].Visible = ((i + 1) <= int.Parse(comboBox.Text));
                delay[i].Visible = ((i + 1) <= int.Parse(comboBox.Text));
                upButton[i].Visible = ((i + 1) <= int.Parse(comboBox.Text));
                downButton[i].Visible = ((i + 1) <= int.Parse(comboBox.Text));
            }
            for (int i = 0; i < M.Length; i++)
            {
                light[i].Text = M[i].ToString();
            }
        }
        private void Connect()
        {
            for (int i = 0; i < id.Length; i++) 
            {
                int index = i;
                upButton[i].Click += (s, e) =>
                {
                    light[index].Text = (int.Parse(light[index].Text) + 1).ToString();
                    _modbusClient.WriteSingleRegister(id[index], int.Parse(light[index].Text));
                };
                downButton[i].Click += (s, e) =>
                {
                    light[index].Text = (int.Parse(light[index].Text) - 1).ToString();
                    _modbusClient.WriteSingleRegister(id[index], int.Parse(light[index].Text));
                };
            }
        }

        private void CreateM_Load(object sender, EventArgs e)
        {

        }
    }
}