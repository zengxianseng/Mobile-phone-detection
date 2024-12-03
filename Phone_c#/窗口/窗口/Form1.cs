using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Reflection.Emit;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Reflection;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Numerics;
using System.Runtime.InteropServices;
//PLC通信包
using System.Net.Sockets;
using System.Net;
using EasyModbus;
using System.Net.NetworkInformation;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Collections;
using 窗口.quanzhong;
using MySql.Data.MySqlClient;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sunny.UI.Win32;
using Sunny.UI;

namespace 窗口
{
    using ControllerHandle = Int64;
    using Label = System.Windows.Forms.Label;
    using MyForms = System.Windows.Forms;
    public partial class Form1 : Form
    {
        //CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto
        [DllImport("Dll2.dll", EntryPoint = "detect", CallingConvention = CallingConvention.Cdecl)]
        public static extern int detect(StringBuilder filename, StringBuilder csvpath);
        private TcpClient tcpClient;
        private NetworkStream stream;
        private byte[] buffer;
        private int[][] id = new int[][]
        {
            new int[]{10,20,30,40 },
            new int[]{100,110,120,130,140,150},
            new int[]{200,210,220,230,240,250},
            new int[]{200,210,220,230,240,250},
            new int[]{200,210,220,230,240,250},
            new int[]{200,210,220,230,240,250},
        };//PLC延迟的ip
        private int[] q = Enumerable.Repeat(2, 10000).ToArray();//初始化都为2;//new int[10000];//状态数组
        public int h;
        private ModbusClient _modbusClient = new ModbusClient("192.168.0.88", 502);
        int[] a1 = new int[4];
        int[] a2 = new int[6];
        int[] a3 = new int[6];
        int[] a4 = new int[6];
        int[] a5 = new int[6];
        int[] a6 = new int[6];
        public DateTime startTime;
        public static List<string> M_Camera = new List<string>();
        private int currentImageIndex = 0;
        private string folderPath = @"./right_configure";//相机拍照总结果文件夹
        string path;//文件目录
        public static string namePath = "./Name.txt";
        string pathtxt = "./Path.txt";
        private Table table;
        private HistoryData hd;
        int[] m = new int[8];//面阵相机
        static int n = 0;//程序启动标志
        public static int j = 0;//开发者标志
        public static int A = 0;
        public Form1()
        {
            InitializeComponent();
            InitializePictureBox();
            this.StartPosition = FormStartPosition.CenterScreen;
            try
            {
                string content = File.ReadAllText(namePath);
                string path1 = File.ReadAllText(pathtxt);
                nameLabel.Text = now_label.Text = content;
                path = path1;
            }
            catch (Exception ex)
            {
                Console.WriteLine("读取文件时出错: " + ex.Message);
            }
            //replace(path);
            Replace();
            panel1.Visible = true;
            panel5.Visible = false;
            panel6.Visible = false;
            table = new Table(listView1);
            hd = new HistoryData(listView2);
            TablePanel.Visible = false;
            startTime = DateTime.Now;
            nowtimer.Start();
            OpenForm();
            string p = "./right_configure/" + GainName() + "/right_configure.csv";
            ReadCsv_WriteForm(p);
            sendon();
        }
        private void nowtimer_Tick(object sender, EventArgs e)
        {
            DateTime currentTime = DateTime.Now; // 获取当前时间
            nowtime.Text = currentTime.ToString("yyyy-MM-dd" + "\n " + "HH:mm:ss");
            TimeSpan ts = currentTime.Subtract(startTime);// 显示当前时间
                                                          // 提取小时、分钟和秒
            int hours = ts.Hours;
            int minutes = ts.Minutes;
            int seconds = ts.Seconds;

            // 如果需要显示总的小时、分钟和秒数
            int totalHours = (int)ts.TotalHours;
            int totalMinutes = (int)ts.TotalMinutes;
            int totalSeconds = (int)ts.TotalSeconds;
            runtime.Text = totalHours.ToString() + "h" + (totalMinutes % 60).ToString() + "min" + (totalSeconds % 60).ToString() + "s";
            rate.Text = ((float.Parse(questionImages.Text) / float.Parse(numberImages.Text)) * 100).ToString("F2");
        }
        private bool IsValidIPAddress(string ipAddress)
        {
            try
            {
                var ip = IPAddress.Parse(ipAddress);
                return ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork;
            }
            catch
            {
                return false;
            }
        }//判断ip是否正确
        private void OpenForm()
        {
            //PLC IP地址
            if (!IsValidIPAddress("192.168.0.88"))
            {
                MessageBox.Show("设备IP地址错误。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // 使用用户输入的IP地址和预设的端口号创建ModbusClient实例
                //_modbusClient = new ModbusClient("192.168.0.88", 502);
                _modbusClient.Connect();
                // 检查连接状态并更新UI
                if (_modbusClient.Connected)
                {
                    label19.ForeColor = System.Drawing.Color.Green;
                    label19.Text = "●";
                    MessageBox.Show("成功连接设备");
                    _modbusClient.WriteSingleRegister(0, 0);
                    _modbusClient.WriteSingleRegister(2, 0);
                    for (int i = 0; i < 4; i++)
                    {
                        int[] registers = _modbusClient.ReadHoldingRegisters((i + 1) * 10, 1);
                        if (registers.Length > 0)
                        {
                            int value = registers[0];
                            a1[i] = value;
                            //Console.WriteLine(value.ToString() + "\n");
                        }
                    }
                    for (int i = 0; i < 6; i++)
                    {
                        int[] registers = _modbusClient.ReadHoldingRegisters(i * 10 + 100, 1);
                        if (registers.Length > 0)
                        {
                            int value = registers[0];
                            a2[i] = value;
                            //Console.WriteLine(value.ToString() + "\n");
                        }
                    }
                    for (int i = 0; i < 6; i++)
                    {
                        int[] registers = _modbusClient.ReadHoldingRegisters(i * 10 + 200, 1);
                        if (registers.Length > 0)
                        {
                            int value1 = registers[0];
                            a3[i] = value1;
                            //Console.WriteLine(value1.ToString() + "\n");
                        }
                    }
                    ConnectComputer();
                    _modbusClient.WriteMultipleCoils(802, new bool[] { false });
                    _modbusClient.WriteMultipleCoils(800, new bool[] { false });
                    _modbusClient.WriteMultipleCoils(804, new bool[] { false });
                }
                else
                {
                    label19.ForeColor = System.Drawing.Color.Red;
                    label19.Text = "连接错误";
                    MessageBox.Show("连接失败。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (SocketException ex)
            {
                // 处理网络异常
                MessageBox.Show($"网络错误: {ex.Message}", "网络错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ArgumentNullException ex)
            {
                // 处理空引用异常
                MessageBox.Show($"参数错误: {ex.Message}", "参数错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                // 捕获其他所有异常
                MessageBox.Show($"连接过程中发生未知错误: {ex.Message}", "未知错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }//连接设备
        private void Form1_Load(object sender, EventArgs e)
        {
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                //WriteCsvFile(path);
                Console.WriteLine("保存成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show("保存失败:" + ex.Message);
            }
            timer2.Stop();
            timer3.Stop();
            stream?.Close();
            tcpClient?.Close();
            WriteTxt();
            sendoff();
        }//关闭窗口操作
        private async void button1_Click(object sender, EventArgs e)
        {
            //mmm();
            string serverIp = "192.168.0.6"; // 替换为服务器的 IP 地址
            int port = 5010;
            try
            {
                using (TcpClient client = new TcpClient(serverIp, port))
                {
                    NetworkStream stream = client.GetStream();
                    string command = "1";
                    byte[] data = Encoding.UTF8.GetBytes(command);

                    // 发送命令到服务器
                    stream.Write(data, 0, data.Length);
                    Console.WriteLine("命令已发送: " + command);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("发生错误: " + ex.Message);
            }
        }
        private void effection_Click(object sender, EventArgs e)
        {
            panel1.Visible = true;
            panel5.Visible = false;
            TablePanel.Visible = false;
            panel6.Visible = false;
        }//主界面显示
        private void control_Click(object sender, EventArgs e)
        {

            if (j != 1)
            {
                panel1.Visible = false;
                panel5.Visible = false;
                panel6.Visible = false;
                TablePanel.Visible = true;
                newConfigure.Visible = false;
                deleteButton.Visible = false;
            }
            else
            {
                //if (nameLabel.Text != "空")
                //{
                //    panel1.Visible = false;
                //    panel5.Visible = true;
                //    TablePanel.Visible = false;
                //}
                //else
                //{
                panel1.Visible = false;
                panel5.Visible = false;
                TablePanel.Visible = true;
                //}
            }
        }
        //配置界面显示
        private void DetectResultBtn_Click(object sender, EventArgs e)
        {
            panel6.Visible = true;
            panel1.Visible = false;
            TablePanel.Visible = false;
            panel5.Visible = false;
        }
        private void returnButton_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
            panel5.Visible = false;
            TablePanel.Visible = true;
        }//返回配置界面
        //面阵相机
        /* private void buttonM1_Click(object sender, EventArgs e)
        {
            if (cameraLabel_M1.Text != "NULL")
            {
                DialogResult result = MessageBox.Show("确定要删除相机嘛？", "确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                // 根据用户的选择执行相应的操作
                if (result == DialogResult.OK)
                {
                    cameraLabel_M1.Text = "NULL";
                    buttonM1.Text = "+";
                    buttonM1.ForeColor = System.Drawing.Color.Green;
                    lightM1_1.Text = lightM1_2.Text = lightM1_3.Text = lightM1_4.Text = lightM1_5.Text = lightM1_6.Text = 0.ToString();
                    lightM1_1.Visible = true;
                    lightM1_2.Visible = true;
                    lightM1_3.Visible = true;
                    lightM1_4.Visible = true;
                    lightM1_5.Visible = true;
                    upButtonM1_5.Visible = true;
                    downButtonM1_5.Visible = true;
                    lightM1_6.Visible = true;
                    upButtonM1_6.Visible = true;
                    downButtonM1_6.Visible = true;
                    MessageBox.Show("相机已删除。");
                }
            }
            else
            {
                Label[] qz = { qzM1_1,qzM1_2, qzM1_3};
                Label[] delay = { delayM1_1, delayM1_2, delayM1_3, delayM1_4, delayM1_5, delayM1_6 };
                Label[] light = { lightM1_1, lightM1_2, lightM1_3, lightM1_4, lightM1_5, lightM1_6 };
                MyForms.Button[] upButton = { upButtonM1_1, upButtonM1_2, upButtonM1_3, upButtonM1_4, upButtonM1_5, upButtonM1_6 };
                MyForms.Button[] downButton = { downButtonM1_1, downButtonM1_2, downButtonM1_3, downButtonM1_4, downButtonM1_5, downButtonM1_6 };
                CreateM createM = new CreateM(qz, Pz1, a1, id[0], buttonM1, cameraLabel_M1, delay, light, upButton, downButton);
                createM.Show();
            }
        }
        private void buttonM2_Click(object sender, EventArgs e)
        {
            if (cameraLabel_M2.Text != "NULL")
            {
                DialogResult result = MessageBox.Show("确定要删除相机嘛？", "确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                // 根据用户的选择执行相应的操作
                if (result == DialogResult.OK)
                {
                    cameraLabel_M2.Text = "NULL";
                    buttonM2.Text = "+";
                    buttonM2.ForeColor = System.Drawing.Color.Green;
                    lightM2_1.Text = lightM2_2.Text = lightM2_3.Text = lightM2_4.Text = lightM2_5.Text = lightM2_6.Text = 0.ToString();
                    lightM2_1.Visible = true;
                    lightM2_2.Visible = true;
                    lightM2_3.Visible = true;
                    lightM2_4.Visible = true;
                    lightM2_5.Visible = true;
                    lightM2_6.Visible = true;
                    MessageBox.Show("相机已删除。");
                }
            }
            else
            {
                Label[] qz = { qzM2_1, qzM2_2, qzM2_3 };
                Label[] delay = { delayM2_1, delayM2_2, delayM2_3, delayM2_4, delayM2_5, delayM2_6 };
                Label[] light = { lightM2_1, lightM2_2, lightM2_3, lightM2_4, lightM2_5, lightM2_6 };
                MyForms.Button[] upButton = { upButtonM2_1, upButtonM2_2, upButtonM2_3, upButtonM2_4, upButtonM2_5, upButtonM2_6 };
                MyForms.Button[] downButton = { downButtonM2_1, downButtonM2_2, downButtonM2_3, downButtonM2_4, downButtonM2_5, downButtonM2_6 };
                CreateM createM = new CreateM(qz, Pz2, a2, id[1], buttonM2, cameraLabel_M2, delay, light, upButton, downButton);
                createM.Show();
            }
        }
        private void buttonM3_Click(object sender, EventArgs e)
        {
            if (cameraLabel_M3.Text != "NULL")
            {
                DialogResult result = MessageBox.Show("确定要删除相机嘛？", "确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                // 根据用户的选择执行相应的操作
                if (result == DialogResult.OK)
                {
                    cameraLabel_M3.Text = "NULL";
                    buttonM3.Text = "+";
                    buttonM3.ForeColor = System.Drawing.Color.Green;
                    lightM3_1.Text = lightM3_2.Text = lightM3_3.Text = lightM3_4.Text = lightM3_5.Text = lightM3_6.Text = 0.ToString();
                    lightM3_1.Visible = true;
                    lightM3_2.Visible = true;
                    lightM3_3.Visible = true;
                    lightM3_4.Visible = true;
                    lightM3_5.Visible = true;
                    lightM3_6.Visible = true;
                    MessageBox.Show("相机已删除。");
                }
            }
            else
            {
                Label[] qz = { qzM3_1, qzM3_2, qzM3_3 };
                Label[] delay = { delayM3_1, delayM3_2, delayM3_3, delayM3_4, delayM3_5, delayM3_6 };
                Label[] light = { lightM3_1, lightM3_2, lightM3_3, lightM3_4, lightM3_5, lightM3_6 };
                MyForms.Button[] upButton = { upButtonM3_1, upButtonM3_2, upButtonM3_3, upButtonM3_4, upButtonM3_5, upButtonM3_6 };
                MyForms.Button[] downButton = { downButtonM3_1, downButtonM3_2, downButtonM3_3, downButtonM3_4, downButtonM3_5, downButtonM3_6 };
                CreateM createM = new CreateM(qz, Pz3, a3, id[2], buttonM3, cameraLabel_M3, delay, light, upButton, downButton);
                createM.Show();
            }
        }
        private void buttonM4_Click(object sender, EventArgs e)
        {
            if (cameraLabel_M4.Text != "NULL")
            {
                DialogResult result = MessageBox.Show("确定要删除相机嘛？", "确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                // 根据用户的选择执行相应的操作
                if (result == DialogResult.OK)
                {
                    cameraLabel_M4.Text = "NULL";
                    buttonM4.Text = "+";
                    buttonM4.ForeColor = System.Drawing.Color.Green;
                    lightM4_1.Text = lightM4_2.Text = lightM4_3.Text = lightM4_4.Text = lightM4_5.Text = lightM4_6.Text = 0.ToString();
                    lightM4_1.Visible = true;
                    lightM4_2.Visible = true;
                    lightM4_3.Visible = true;
                    lightM4_4.Visible = true;
                    lightM4_5.Visible = true;
                    lightM4_6.Visible = true;
                    MessageBox.Show("相机已删除。");
                }
            }
            else
            {
                Label[] qz = { qzM4_1, qzM4_2, qzM4_3 };
                Label[] delay = { delayM4_1, delayM4_2, delayM4_3, delayM4_4, delayM4_5, delayM4_6 };
                Label[] light = { lightM4_1, lightM4_2, lightM4_3, lightM4_4, lightM4_5, lightM4_6 };
                MyForms.Button[] upButton = { upButtonM4_1, upButtonM4_2, upButtonM4_3, upButtonM4_4, upButtonM4_5, upButtonM4_6 };
                MyForms.Button[] downButton = { downButtonM4_1, downButtonM4_2, downButtonM4_3, downButtonM4_4, downButtonM4_5, downButtonM4_6 };
                CreateM createM = new CreateM(qz, Pz4, a4, id[3], buttonM4, cameraLabel_M4, delay, light, upButton, downButton);
                createM.Show();
            }
        }
        private void buttonM5_Click(object sender, EventArgs e)
        {
            if (cameraLabel_M5.Text != "NULL")
            {
                DialogResult result = MessageBox.Show("确定要删除相机嘛？", "确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                // 根据用户的选择执行相应的操作
                if (result == DialogResult.OK)
                {
                    cameraLabel_M5.Text = "NULL";
                    buttonM5.Text = "+";
                    buttonM5.ForeColor = System.Drawing.Color.Green;
                    lightM5_1.Text = lightM5_2.Text = lightM5_3.Text = lightM5_4.Text = lightM5_5.Text = lightM5_6.Text = 0.ToString();
                    lightM5_1.Visible = true;
                    lightM5_2.Visible = true;
                    lightM5_3.Visible = true;
                    lightM5_4.Visible = true;
                    lightM5_5.Visible = true;
                    lightM5_6.Visible = true;
                    MessageBox.Show("相机已删除。");
                }
            }
            else
            {
                Label[] qz = { qzM5_1, qzM5_2, qzM5_3 };
                Label[] delay = { delayM5_1, delayM5_2, delayM5_3, delayM5_4, delayM5_5, delayM5_6 };
                Label[] light = { lightM5_1, lightM5_2, lightM5_3, lightM5_4, lightM5_5, lightM5_6 };
                MyForms.Button[] upButton = { upButtonM5_1, upButtonM5_2, upButtonM5_3, upButtonM5_4, upButtonM5_5, upButtonM5_6 };
                MyForms.Button[] downButton = { downButtonM5_1, downButtonM5_2, downButtonM5_3, downButtonM5_4, downButtonM5_5, downButtonM5_6 };
                CreateM createM = new CreateM(qz, Pz5, a5, id[4], buttonM5, cameraLabel_M5, delay, light, upButton, downButton);
                createM.Show();
            }
        }
        private void buttonM6_Click(object sender, EventArgs e)
        {
            if (cameraLabel_M6.Text != "NULL")
            {
                DialogResult result = MessageBox.Show("确定要删除相机嘛？", "确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                // 根据用户的选择执行相应的操作
                if (result == DialogResult.OK)
                {
                    cameraLabel_M6.Text = "NULL";
                    buttonM6.Text = "+";
                    buttonM6.ForeColor = System.Drawing.Color.Green;
                    lightM6_1.Text = lightM6_2.Text = lightM6_3.Text = lightM6_4.Text = lightM6_5.Text = lightM6_6.Text = 0.ToString();
                    lightM6_1.Visible = true;
                    lightM6_2.Visible = true;
                    lightM6_3.Visible = true;
                    lightM6_4.Visible = true;
                    lightM6_5.Visible = true;
                    lightM6_6.Visible = true;
                    MessageBox.Show("相机已删除。");
                }
            }
            else
            {
                Label[] qz = { qzM6_1, qzM6_2, qzM6_3 };
                Label[] delay = { delayM6_1, delayM6_2, delayM6_3, delayM6_4, delayM6_5, delayM6_6 };
                Label[] light = { lightM6_1, lightM6_2, lightM6_3, lightM6_4, lightM6_5, lightM6_6 };
                MyForms.Button[] upButton = { upButtonM6_1, upButtonM6_2, upButtonM6_3, upButtonM6_4, upButtonM6_5, upButtonM6_6 };
                MyForms.Button[] downButton = { downButtonM6_1, downButtonM6_2, downButtonM6_3, downButtonM6_4, downButtonM6_5, downButtonM6_6 };
                CreateM createM = new CreateM(qz, Pz6, a6, id[5], buttonM6, cameraLabel_M6, delay, light, upButton, downButton);
                createM.Show();
            }
        }*/
        private void deleteButton_Click(object sender, EventArgs e)
        {
            string folderPath1 = "./CameraConfiguration";
            if (listView1.SelectedItems.Count > 0)
            {
                ListViewItem selectedItem = listView1.SelectedItems[0];
                string filePath = Path.Combine(folderPath1, selectedItem.Text + ".csv");
                DialogResult result = MessageBox.Show("确定要删除选定的配置吗？", "确认删除", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                    listView1.Items.Remove(selectedItem);
                }
            }
            else
            {
                MessageBox.Show("请选择一个配置删除");
            }
        }//删除配置
        private void selectButton_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                MessageBox.Show("请选择一个配置");
                return;
            }
            DialogResult result = MessageBox.Show("确定要选择这个配置吗？", "确定选择", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                table.CopySelectedCsvToTrans();
                if (listView1.SelectedItems.Count > 0)
                {
                    ListViewItem selectedItem = listView1.SelectedItems[0];
                    nameLabel.Text = selectedItem.Text;
                    now_label.Text = nameLabel.Text;
                    try
                    {
                        // 使用 File.WriteAllText 将变量写入文件
                        File.WriteAllText(namePath, selectedItem.Text);
                        File.WriteAllText(pathtxt, "./CameraConfiguration//" + selectedItem.Text + ".csv");
                        //Console.WriteLine("变量已成功写入文件！");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("写入文件时出错: " + ex.Message);
                    }
                    path = "./CameraConfiguration//" + selectedItem.Text + ".csv";
                    //replace(path);
                    Replace();
                    string s = "./right_configure/" + selectedItem.Text + "/right_configure.csv";
                    //ReadCsv(s);
                    ReadCsv_WriteForm(s);
                    File.WriteAllText("./peizhipath.txt", "./Peizhi\\" + selectedItem.Text + ".csv");
                    _modbusClient.WriteSingleRegister(0, 0);
                    _modbusClient.WriteSingleRegister(2, 0);
                    //baocun();
                }
                else
                {
                    nameLabel.Text = string.Empty;
                }
                panel1.Visible = false;
                panel5.Visible = true;
                this.panel5.Refresh();
                TablePanel.Visible = false;
                if (j != 1)
                {
                    //Lock();
                }
                else
                {
                    //UnLock();
                }
                if (n != 1)
                {
                    MessageBox.Show("切换成功");
                }
                else
                {
                    MessageBox.Show("切换失败，程序已启动，请暂停程序");
                }
            }
        }//选择配置
        private async void mmm()
        {
            _modbusClient.WriteMultipleCoils(800, new bool[] { true });
            await Task.Delay(900);
            _modbusClient.WriteMultipleCoils(800, new bool[] { false });
        }
        private async void nnn()
        {
            _modbusClient.WriteMultipleCoils(802, new bool[] { true });
            await Task.Delay(900);
            _modbusClient.WriteMultipleCoils(802, new bool[] { false });
        }
        private async void zzz()
        {
            _modbusClient.WriteMultipleCoils(804, new bool[] { true });
            await Task.Delay(900);
            _modbusClient.WriteMultipleCoils(804, new bool[] { false });
        }
        private void Replace()
        {
            if (nameLabel != null || nameLabel.Text != "空")
            {
                camera_M1.Text = cameraLabel_M1.Text;
                camera_M2.Text = cameraLabel_M2.Text;
                camera_M3.Text = cameraLabel_M3.Text;
                camera_X1.Text = cameraLabel_X1.Text;
                camera_X2.Text = cameraLabel_X2.Text;
                camera_X3.Text = cameraLabel_X3.Text;
            }

            if (camera_M1.Text != "NULL")
            {
                M1.ForeColor = System.Drawing.Color.Green;
            }
            else
            {
                M1.ForeColor = System.Drawing.Color.Red;
            }

            if (camera_M2.Text != "NULL")
            {
                M2.ForeColor = System.Drawing.Color.Green;
            }
            else
            {
                M2.ForeColor = System.Drawing.Color.Red;
            }

            if (camera_M3.Text != "NULL")
            {
                M3.ForeColor = System.Drawing.Color.Green;
            }
            else
            {
                M3.ForeColor = System.Drawing.Color.Red;
            }

            if (camera_X1.Text != "NULL")
            {
                X1.ForeColor = System.Drawing.Color.Green;
            }
            else
            {
                X1.ForeColor = System.Drawing.Color.Red;
            }

            if (camera_X2.Text != "NULL")
            {
                X2.ForeColor = System.Drawing.Color.Green;
            }
            else
            {
                X2.ForeColor = System.Drawing.Color.Red;
            }

            if (camera_X3.Text != "NULL")
            {
                X3.ForeColor = System.Drawing.Color.Green;
            }
            else
            {
                X3.ForeColor = System.Drawing.Color.Red;
            }

        }//主界面刷新
        /*        private void Lock()
            {
                buttonM1.Visible = false;
                buttonM2.Visible = false;
                buttonM3.Visible = false;
                buttonM4.Visible = false;
                buttonM5.Visible = false;
                buttonM6.Visible = false;
                upButtonM1_1.Visible = false;
                upButtonM1_2.Visible = false;
                upButtonM1_3.Visible = false;
                upButtonM1_4.Visible = false;
                upButtonM1_5.Visible = false;
                upButtonM1_6.Visible = false;

                upButtonM2_1.Visible = false;
                upButtonM2_2.Visible = false;
                upButtonM2_3.Visible = false;
                upButtonM2_4.Visible = false;
                upButtonM2_5.Visible = false;
                upButtonM2_6.Visible = false;

                upButtonM3_1.Visible = false;
                upButtonM3_2.Visible = false;
                upButtonM3_3.Visible = false;
                upButtonM3_4.Visible = false;
                upButtonM3_5.Visible = false;
                upButtonM3_6.Visible = false;

                upButtonM4_1.Visible = false;
                upButtonM4_2.Visible = false;
                upButtonM4_3.Visible = false;
                upButtonM4_4.Visible = false;
                upButtonM4_5.Visible = false;
                upButtonM4_6.Visible = false;

                upButtonM5_1.Visible = false;
                upButtonM5_2.Visible = false;
                upButtonM5_3.Visible = false;
                upButtonM5_4.Visible = false;
                upButtonM5_5.Visible = false;
                upButtonM5_6.Visible = false;

                upButtonM6_1.Visible = false;
                upButtonM6_2.Visible = false;
                upButtonM6_3.Visible = false;
                upButtonM6_4.Visible = false;
                upButtonM6_5.Visible = false;
                upButtonM6_6.Visible = false;

                downButtonM1_1.Visible = false;
                downButtonM1_2.Visible = false;
                downButtonM1_3.Visible = false;
                downButtonM1_4.Visible = false;
                downButtonM1_5.Visible = false;
                downButtonM1_6.Visible = false;

                downButtonM2_1.Visible = false;
                downButtonM2_2.Visible = false;
                downButtonM2_3.Visible = false;
                downButtonM2_4.Visible = false;
                downButtonM2_5.Visible = false;
                downButtonM2_6.Visible = false;

                downButtonM3_1.Visible = false;
                downButtonM3_2.Visible = false;
                downButtonM3_3.Visible = false;
                downButtonM3_4.Visible = false;
                downButtonM3_5.Visible = false;
                downButtonM3_6.Visible = false;

                downButtonM4_1.Visible = false;
                downButtonM4_2.Visible = false;
                downButtonM4_3.Visible = false;
                downButtonM4_4.Visible = false;
                downButtonM4_5.Visible = false;
                downButtonM4_6.Visible = false;

                downButtonM5_1.Visible = false;
                downButtonM5_2.Visible = false;
                downButtonM5_3.Visible = false;
                downButtonM5_4.Visible = false;
                downButtonM5_5.Visible = false;
                downButtonM5_6.Visible = false;

                downButtonM6_1.Visible = false;
                downButtonM6_2.Visible = false;
                downButtonM6_3.Visible = false;
                downButtonM6_4.Visible = false;
                downButtonM6_5.Visible = false;
                downButtonM6_6.Visible = false;

                saveButton.Visible = false;
            }*/
        /* private void UnLock()
        {
            bool U = true;
            buttonM1.Visible = U;
            buttonM2.Visible = U;
            buttonM3.Visible = U;
            buttonM4.Visible = U;
            buttonM5.Visible = true;
            buttonM6.Visible = true;
            upButtonM1_1.Visible = true;
            upButtonM1_2.Visible = true;
            upButtonM1_3.Visible = true;
            upButtonM1_4.Visible = true;
            upButtonM1_5.Visible = true;
            upButtonM1_6.Visible = true;

            upButtonM2_1.Visible = true;
            upButtonM2_2.Visible = true;
            upButtonM2_3.Visible = true;
            upButtonM2_4.Visible = true;
            upButtonM2_5.Visible = true;
            upButtonM2_6.Visible = true;

            upButtonM3_1.Visible = true;
            upButtonM3_2.Visible = true;
            upButtonM3_3.Visible = true;
            upButtonM3_4.Visible = true;
            upButtonM3_5.Visible = true;
            upButtonM3_6.Visible = true;

            upButtonM4_1.Visible = true;
            upButtonM4_2.Visible = true;
            upButtonM4_3.Visible = true;
            upButtonM4_4.Visible = true;
            upButtonM4_5.Visible = true;
            upButtonM4_6.Visible = true;

            upButtonM5_1.Visible = true;
            upButtonM5_2.Visible = true;
            upButtonM5_3.Visible = true;
            upButtonM5_4.Visible = true;
            upButtonM5_5.Visible = true;
            upButtonM5_6.Visible = true;

            upButtonM6_1.Visible = true;
            upButtonM6_2.Visible = true;
            upButtonM6_3.Visible = true;
            upButtonM6_4.Visible = true;
            upButtonM6_5.Visible = true;
            upButtonM6_6.Visible = true;

            downButtonM1_1.Visible = true;
            downButtonM1_2.Visible = true;
            downButtonM1_3.Visible = true;
            downButtonM1_4.Visible = true;
            downButtonM1_5.Visible = true;
            downButtonM1_6.Visible = true;

            downButtonM2_1.Visible = true;
            downButtonM2_2.Visible = true;
            downButtonM2_3.Visible = true;
            downButtonM2_4.Visible = true;
            downButtonM2_5.Visible = true;
            downButtonM2_6.Visible = true;

            downButtonM3_1.Visible = true;
            downButtonM3_2.Visible = true;
            downButtonM3_3.Visible = true;
            downButtonM3_4.Visible = true;
            downButtonM3_5.Visible = true;
            downButtonM3_6.Visible = true;

            downButtonM4_1.Visible = true;
            downButtonM4_2.Visible = true;
            downButtonM4_3.Visible = true;
            downButtonM4_4.Visible = true;
            downButtonM4_5.Visible = true;
            downButtonM4_6.Visible = true;

            downButtonM5_1.Visible = true;
            downButtonM5_2.Visible = true;
            downButtonM5_3.Visible = true;
            downButtonM5_4.Visible = true;
            downButtonM5_5.Visible = true;
            downButtonM5_6.Visible = true;

            downButtonM6_1.Visible = true;
            downButtonM6_2.Visible = true;
            downButtonM6_3.Visible = true;
            downButtonM6_4.Visible = true;
            downButtonM6_5.Visible = true;
            downButtonM6_6.Visible = true;

            saveButton.Visible = true;
        }*/
        private void saveButton_Click(object sender, EventArgs e)
        {
            try
            {
                //WriteCsvFile(path);
                Replace();
                MessageBox.Show("保存成功！");
            }
            catch (Exception ex)
            {
                MessageBox.Show("保存失败:" + ex.Message);
            }

        }//保存按钮
        private void rrr(System.Windows.Forms.Label label, System.Windows.Forms.Button button)
        {
            if (label.Text == "NULL")
            {
                button.Text = "+";
                button.ForeColor = System.Drawing.Color.Green;
            }
            else
            {
                button.Text = "-";
                button.ForeColor = System.Drawing.Color.Red;
            }
        }//刷新
        /*private void replace(string path)
        {
            string[][] csvData = ReadCsvFile(path);
            string[][] csvData1 = ReadCsvFile("./Peizhi/" + GainName() + ".csv");
            cameraLabel_M1.Text = csvData[0][0];
            if(csvData[0][0] != "NULL")
            {
                int[] M1 = new int[6];
                for (int i = 0; i < 6; i++)
                {
                    M1[i] = int.Parse(csvData[i + 1][0]);
                }
            }
            rrr(cameraLabel_M1, buttonM1);//切换外观
            lightM1_1.Text = csvData[1][0];
            lightM1_2.Text = csvData[2][0];
            lightM1_3.Text = csvData[3][0];
            lightM1_4.Text = csvData[4][0];
            lightM1_5.Text = csvData[5][0];
            if (csvData1[0][0] != "NULL")
            {
                Pz1.Text = csvData1[0][0].Substring(8);
            }
            else
            {
                Pz1.Text = csvData1[0][0];
            }

            if (csvData[5][0] == "0")
            {
                delayM1_5.Visible = false;
                lightM1_5.Visible = false;
                upButtonM1_5.Visible = false;
                downButtonM1_5.Visible = false;
            }
            lightM1_6.Text = csvData[6][0];
            if (csvData[6][0] == "0")
            {
                delayM1_6.Visible = false;
                lightM1_6.Visible = false;
                upButtonM1_6.Visible = false;
                downButtonM1_6.Visible = false;
            }
           
            cameraLabel_M2.Text = csvData[0][1];
            if(csvData[0][1] != "NULL")
            {
                int[] M2 = new int[6];
                for (int i = 0; i < 6; i++)
                {
                    M2[i] = int.Parse(csvData[i + 1][1]);
                }
            }
            rrr(cameraLabel_M2, buttonM2);
            lightM2_1.Text = csvData[1][1];
            lightM2_2.Text = csvData[2][1];
            lightM2_3.Text = csvData[3][1];
            lightM2_4.Text = csvData[4][1];
            lightM2_5.Text = csvData[5][1];
            lightM2_6.Text = csvData[6][1];
            if (csvData1[1][0] != "NULL")
            {
                Pz2.Text = csvData1[1][0].Substring(8);
            }
            else
            {
                Pz2.Text = csvData1[1][0];
            }

            cameraLabel_M3.Text = csvData[0][2];
            if(csvData[0][2] != "NULL")
            {
                int[] M3 = new int[6];
                for (int i = 0; i < 6; i++)
                {
                    M3[i] = int.Parse(csvData[i + 1][2]);
                }
            }
            rrr(cameraLabel_M3, buttonM3);
            lightM3_1.Text = csvData[1][2];
            lightM3_2.Text = csvData[2][2];
            lightM3_3.Text = csvData[3][2];
            lightM3_4.Text = csvData[4][2];
            lightM3_5.Text = csvData[5][2];
            lightM3_6.Text = csvData[6][2];
            if (csvData1[2][0] != "NULL")
            {
                Pz3.Text = csvData1[2][0].Substring(8);
            }
            else
            {
                Pz3.Text = csvData1[2][0];
            }

            cameraLabel_M4.Text = csvData[0][3];
            if(csvData[0][3] != "NULL")
            {
                int[] M4 = new int[6];
                for (int i = 0; i < 6; i++)
                {
                    M4[i] = int.Parse(csvData[i + 1][3]);
                }
            }
            rrr(cameraLabel_M4, buttonM4);
            lightM4_1.Text = csvData[1][3];
            lightM4_2.Text = csvData[2][3];
            lightM4_3.Text = csvData[3][3];
            lightM4_4.Text = csvData[4][3];
            lightM4_5.Text = csvData[5][3];
            lightM4_6.Text = csvData[6][3];
            if (csvData1[3][0] != "NULL")
            {
                Pz4.Text = csvData1[3][0].Substring(8);
            }
            else
            {
                Pz4.Text = csvData1[3][0];
            }

            cameraLabel_M5.Text = csvData[0][4];
            if (csvData[0][4] != "NULL")
            {
                int[] M5 = new int[4];
                for (int i = 0; i < 4; i++)
                {
                    M5[i] = int.Parse(csvData[i + 1][4]);
                }
            }
            rrr(cameraLabel_M5, buttonM5);
            lightM5_1.Text = csvData[1][4];
            lightM5_2.Text = csvData[2][4];
            lightM5_3.Text = csvData[3][4];
            lightM5_4.Text = csvData[4][4];
            lightM5_5.Text = csvData[5][4];
            lightM5_6.Text = csvData[6][4];
            if (csvData1[4][0] != "NULL")
            {
                Pz5.Text = csvData1[4][0].Substring(8);
            }
            else
            {
                Pz5.Text = csvData1[4][0];
            }

            cameraLabel_M6.Text = csvData[0][5];
            if(csvData[0][5] != "NULL")
            {
                int[] M6 = new int[6];
                for (int i = 0; i < 6; i++)
                {
                    M6[i] = int.Parse(csvData[i + 1][5]);
                }
            }
            rrr(cameraLabel_M6, buttonM6);
            lightM6_1.Text = csvData[1][5];
            lightM6_2.Text = csvData[2][5];
            lightM6_3.Text = csvData[3][5];
            lightM6_4.Text = csvData[4][5];
            lightM6_5.Text = csvData[5][5];
            lightM6_6.Text = csvData[6][5];
            if (csvData1[5][0] != "NULL")
            {
                Pz6.Text = csvData1[5][0].Substring(8);
            }
            else
            {
                Pz6.Text = csvData1[5][0];
            }

        }//选择配置与主页替换*/
        private void newConfigure_Click(object sender, EventArgs e)
        {
            CreateForm cF = new CreateForm();
            cF.RefreshTableEvent += RefreshTableInForm1;
            cF.StartPosition = FormStartPosition.CenterScreen;
            cF.ShowDialog();
        }//新建配置
        private void RefreshTableInForm1()
        {
            new Table(listView1);
        }
        /*private void WriteCsvFile(string path)
        {
            // 读取整个 CSV 文件
            string[][] csvData = ReadCsvFile(path);

            // 确保文件内容符合预期
            if (csvData != null && csvData.Length >= 10 && csvData[0].Length >= 6)
            {
                // 更新与标签对应的列

                csvData[0][0] = cameraLabel_M1.Text;
                csvData[1][0] = lightM1_1.Text;
                csvData[2][0] = lightM1_2.Text;
                csvData[3][0] = lightM1_3.Text;
                csvData[4][0] = lightM1_4.Text;
                csvData[5][0] = lightM1_5.Text;
                csvData[6][0] = lightM1_6.Text;

                csvData[0][1] = cameraLabel_M2.Text;
                csvData[1][1] = lightM2_1.Text;
                csvData[2][1] = lightM2_2.Text;
                csvData[3][1] = lightM2_3.Text;
                csvData[4][1] = lightM2_4.Text;
                csvData[5][1] = lightM2_5.Text;
                csvData[6][1] = lightM2_6.Text;

                csvData[0][2] = cameraLabel_M3.Text;
                csvData[1][2] = lightM3_1.Text;
                csvData[2][2] = lightM3_2.Text;
                csvData[3][2] = lightM3_3.Text;
                csvData[4][2] = lightM3_4.Text;
                csvData[5][2] = lightM3_5.Text;
                csvData[6][2] = lightM3_6.Text;

                csvData[0][3] = cameraLabel_M4.Text;
                csvData[1][3] = lightM4_1.Text;
                csvData[2][3] = lightM4_2.Text;
                csvData[3][3] = lightM4_3.Text;
                csvData[4][3] = lightM4_4.Text;
                csvData[5][3] = lightM4_5.Text;
                csvData[6][3] = lightM4_6.Text;

                csvData[0][4] = cameraLabel_M5.Text;
                csvData[1][4] = lightM5_1.Text;
                csvData[2][4] = lightM5_2.Text;
                csvData[3][4] = lightM5_3.Text;
                csvData[4][4] = lightM5_4.Text;
                csvData[5][4] = lightM5_5.Text;
                csvData[6][4] = lightM5_6.Text;

                csvData[0][5] = cameraLabel_M6.Text;
                csvData[1][5] = lightM6_1.Text;
                csvData[2][5] = lightM6_2.Text;
                csvData[3][5] = lightM6_3.Text;
                csvData[4][5] = lightM6_4.Text;
                csvData[5][5] = lightM6_5.Text;
                csvData[6][5] = lightM6_6.Text;

                // 将更新后的数据写回 CSV 文件
                try
                {
                    using (StreamWriter writer = new StreamWriter(path))
                    {
                        for (int i = 0; i < csvData.Length; i++)
                        {
                            writer.WriteLine(string.Join(",", csvData[i]));
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("写入 CSV 文件时出错: " + ex.Message);
                }
            }
        }//写入csv配置文件*/
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
        }//读csv文件
        public static string GainName()
        {
            return File.ReadAllText(namePath);
        }//获得当前配置名称
        private void InitializePictureBox()
        {
            // 设置PictureBox的SizeMode为Zoom，以保持图片的宽高比
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxM1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxM2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxM3.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxX1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxX2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxX3.SizeMode = PictureBoxSizeMode.Zoom;
        }
        public int a;
        public int b;
        private int number = 0;
        private int number2 = 0;
        private int ok = 0;
        private int ng = 0;
        private int err = 0;
        private void qkbutton_Click(object sender, EventArgs e)
        {
            ok = 0;
            ng = 0;
            err = 0;
            errlabel.Text = ok.ToString();
            oklabel.Text = ng.ToString();
            nglabel.Text = err.ToString();
        }
        private async void timer3_Tick(object sender, EventArgs e)//检测数量
        {
            int[] registers = _modbusClient.ReadHoldingRegisters(0, 1);
            number = registers[0];
            numberImages.Text = number.ToString();
            int[] registers1 = _modbusClient.ReadHoldingRegisters(2, 1);
            //await Task.Delay(500);
            switch (q[registers1[0]])
            {
                case 2:
                    {
                        if (a != registers1[0])
                        {
                            GCHandle handle = GCHandle.Alloc(q, GCHandleType.Pinned);
                            try
                            {
                                int index = 1; // 指定要输出地址的元素索引
                                IntPtr ptr = Marshal.UnsafeAddrOfPinnedArrayElement(q, index);
                                Console.WriteLine($"输出plc111 {index} 的地址: {ptr}+{registers1[0]}");
                            }
                            finally
                            {
                                // 释放句柄
                                handle.Free();
                            }
                            a = registers1[0];
                            zzz();
                            WriteTxt();
                            err++;
                            errlabel.Text = err.ToString();
                            MessageBox.Show(registers1[0] + " p[registers1[0]]:" + q[registers1[0]]);
                            MessageBox.Show("程序故障，请检测！");
                            timer3.Stop();
                        }
                    };
                    break;
                case 0:
                    {
                        if (a != registers1[0])
                        {
                            a = registers1[0];
                            mmm();
                            number2++;
                            questionImages.Text = number2.ToString();
                            ng++;
                            nglabel.Text = ng.ToString();
                        }
                    };
                    break;
                case 1:
                    {
                        if (a != registers1[0])
                        {
                            a = registers1[0];
                            nnn();
                            ok++;
                            oklabel.Text = ok.ToString();
                        }
                    };
                    break;
            }
        }
        private void WriteTxt()
        {
            // 获取当前时间
            DateTime now = DateTime.Now;

            // 生成文件名，格式为 "yyMMdd_HHmm.txt"
            string fileName = $"{now:yyMMdd_HHmm}.txt";

            // 定义要写入的内容
            string content = ok.ToString() + "," + ng.ToString();

            // 定义目录路径
            string directoryPath = "./history";
            string last = "last.txt";

            string filePath = Path.Combine(directoryPath, fileName);
            // 创建或打开文件，并将内容写入文件
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.WriteLine(content);
                }
                using (StreamWriter writer1 = new StreamWriter(last))
                {
                    writer1.WriteLine(content);
                }

                Console.WriteLine($"文件 {fileName} 已成功创建并写入内容。");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"写入文件时发生错误: {ex.Message}");
            }
        }
        System.Threading.Timer timer;
        private async void startButton_Click(object sender, EventArgs e)
        {
            timer = new System.Threading.Timer(DetectPicture, null, 30000, 60000);
            var cts = new CancellationTokenSource();
            if (startButton.Text == "开始检测")
            {
                string[] pic = Directory.GetFiles("./right_configure/" + GainName(), "*.jpeg");
                pictureBox2.Image = Image.FromFile(pic[0]);
                pictureBox3.Image = Image.FromFile(pic[1]);
                LoadingForm loadingForm = new LoadingForm();
                // 启动 t 方法
                //var task = t(cts.Token);
                t();
                loadingForm.Show();
                await Task.Delay(15000);
                loadingForm.Close();
                //sendon();
                timer3.Start();
                n = 1;
                startButton.Text = "停止检测";
            }
            else
            {
                timer3.Stop();
                //server.shut();
                //stop();
                //sendoff();
                n = 0;
                startButton.Text = "开始检测";
            }
        }
        private void DetectPicture(object state)
        {
            //面一拍摄图
            string[] pic_M1 = Directory.GetFiles("E:\\result_source\\l3", "*.jpg");
            if (pic_M1.Length != 0)
            {
                pictureBoxM1.Image = Image.FromFile(pic_M1[pic_M1.Length - 1]);
            }
            //面二拍摄图
            string[] pic_M2 = Directory.GetFiles("E:\\result_source\\l4", "*.jpg");
            if (pic_M2.Length != 0)
            {
                pictureBoxM2.Image = Image.FromFile(pic_M2[pic_M2.Length - 1]);
            }
/*            //面三拍摄图
            string[] pic_M3 = Directory.GetFiles("E:\\result_source\\l4", "*.jpg");
            if (pic_M3.Length != 0)
            {
                pictureBoxM3.Image = Image.FromFile(pic_M3[pic_M3.Length - 1]);
            }*/
            //线一拍摄图
            string[] pic_X1 = Directory.GetFiles("E:\\result_source\\aike2", "*.jpg");
            if (pic_X1.Length != 0)
            {
                pictureBoxX1.Image = Image.FromFile(pic_X1[pic_X1.Length - 1]);
            }

        }
        string nm;
        async Task t()
        {
            DateTime dt = DateTime.Now;
            nm = dt.ToString("yyyy-MM-dd");
            //创建数据库
            TcpServer server = new TcpServer();
            Task openTask;
            string path1 = "./right_configure/"+GainName();
            string result1 = path1 + "/right_configure.csv";//Path.Combine(path1, "right_configure.csv");
            string result2 = path1 + "/right_single_class_conf.csv";
            //Console.WriteLine(result);
            openTask = Task.Run(() =>
            {
                try
                {
                    open(result1, result2);
                }
                catch (Exception ex)
                {
                    // 处理异常，避免崩溃
                    Console.WriteLine($"程序异常崩溃： {ex.Message}");
                }
            });
            /*if (GainName() == "ZS-C-249")
            {
                openTask = Task.Run(() =>
                {
                    try
                    {
                        open();
                    }
                    catch (Exception ex)
                    {
                        // 处理异常，避免崩溃
                        Console.WriteLine($"程序异常崩溃： {ex.Message}");
                    }
                });
            }
            else
            {
                openTask = Task.Run(() =>
                {
                    try
                    {
                        open1();
                    }
                    catch (Exception ex)
                    {
                        // 处理异常，避免崩溃
                        Console.WriteLine($"Error in open: {ex.Message}");
                    }
                });
            }*/
            Task start = server.StartServerAsync(q);
            await Task.WhenAll(openTask, start);
        }
        private void open(string path1, string path2)
        {
            try
            {
                StringBuilder sb = new StringBuilder(path1);
                StringBuilder cp = new StringBuilder(path2);
                int result = detect(sb, cp);
                Console.WriteLine($"Return Value: {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
        /*private void open1()
        {
            try
            {
                StringBuilder sb = new StringBuilder(@"./right_configure/ZS-D-243/right_configure.csv");
                StringBuilder cp = new StringBuilder(@"./right_configure/ZS-D-243/right_single_class_conf.csv");
                int result = detect(sb, cp);
                Console.WriteLine($"Return Value: {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }*/
        /*        private void stop()
        {
            string processName = "YOLOv8-ONNXRuntime-CPP"; // 进程名称，不带 .exe 后缀
            try
            {
                // 获取所有与指定名称匹配的进程
                Process[] processes = Process.GetProcessesByName(processName);
                if (processes.Length > 0)
                {
                    foreach (Process process in processes)
                    {
                        process.Kill(); // 终止进程
                        Console.WriteLine($"已终止进程: {process.ProcessName} (ID: {process.Id})");
                    }
                }
                else
                {
                    Console.WriteLine("没有找到指定的进程。");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发生错误: {ex.Message}");
            }
        }*/
        private void sendon()
        {
            string serverIp = "192.168.0.6"; // 替换为服务器的 IP 地址
            int port = 5010;

            try
            {
                using (TcpClient client = new TcpClient(serverIp, port))
                {
                    NetworkStream stream = client.GetStream();
                    string command = "1";
/*                    if (GainName() == "ZS-C-249")
                    {
                        command = "1";
                    }
                    else
                    {
                        command = "1";
                    }*/
                    byte[] data = Encoding.UTF8.GetBytes(command);

                    // 发送命令到服务器
                    stream.Write(data, 0, data.Length);
                    Console.WriteLine("命令已发送: " + command);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("发生错误: " + ex.Message);
            }
        }
        private void sendoff()
        {
            string serverIp = "192.168.0.6"; // 替换为服务器的 IP 地址
            int port = 5010;

            try
            {
                using (TcpClient client = new TcpClient(serverIp, port))
                {
                    NetworkStream stream = client.GetStream();
                    string command = "0";
                    byte[] data = Encoding.UTF8.GetBytes(command);

                    // 发送命令到服务器
                    stream.Write(data, 0, data.Length);
                    Console.WriteLine("命令已发送: " + command);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("发生错误: " + ex.Message);
            }
        }
        /*private void SendStart()
        {
            string serverIp = "192.168.0.6"; // 替换为服务器的 IP 地址
            int port = 5010;

            try
            {
                using (TcpClient client = new TcpClient(serverIp, port))
                {
                    NetworkStream stream = client.GetStream();
                    string command = "2";
                    byte[] data = Encoding.UTF8.GetBytes(command);

                    // 发送命令到服务器
                    stream.Write(data, 0, data.Length);
                    Console.WriteLine("命令已发送: " + command);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("发生错误: " + ex.Message);
            }
        }*/
        private void ConnectComputer()
        {
            try
            {
                using (Ping p = new Ping())
                {
                    PingReply reply = p.Send("192.168.0.6");
                    if (reply.Status == IPStatus.Success)
                    {
                        MessageBox.Show("设备1连接成功");
                        sbs.ForeColor = System.Drawing.Color.Green;
                    }
                    else
                    {
                        MessageBox.Show("连接失败");
                        sbs.ForeColor = System.Drawing.Color.Red;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("连接错误:{ex.Message}");
            }
        }
        private void pb_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("explorer.exe", "D:\\Quexian\\" + GainName() + "\\yuantu");
                Console.WriteLine("文件夹已打开。");
            }
            catch (Exception ex)
            {
                Console.WriteLine("无法打开文件夹: " + ex.Message);
            }
        }
        private void pzButton1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "C:\\configure\\camera";
                openFileDialog.Title = "请选择配置文件";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string fp = openFileDialog.FileName;
                    string[] lines = File.ReadAllLines("./Peizhi\\" + Form1.GainName() + ".csv");
                    //lines[int.Parse(Pzx1.Name.Substring(3)) + 5] = fp;
                    File.WriteAllLines("./Peizhi\\" + Form1.GainName() + ".csv", lines);
                    //Pzx1.Text = fp.Substring(20);
                    Console.WriteLine(fp);
                    //Console.WriteLine(p.Name);
                }
            }
        }
        private void pzButton2_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "C:\\configure\\camera";
                openFileDialog.Title = "请选择配置文件";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string fp = openFileDialog.FileName;
                    string[] lines = File.ReadAllLines("./Peizhi\\" + Form1.GainName() + ".csv");
                    //lines[int.Parse(Pzx2.Name.Substring(3)) + 5] = fp;
                    File.WriteAllLines("./Peizhi\\" + Form1.GainName() + ".csv", lines);
                    //Pzx2.Text = fp.Substring(20);
                    Console.WriteLine(fp);

                }
            }
        }
        private void qb_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("explorer.exe", "D:\\Quexian\\" + GainName() + "\\quexian");
                Console.WriteLine("文件夹已打开。");
            }
            catch (Exception ex)
            {
                Console.WriteLine("无法打开文件夹: " + ex.Message);
            }
        }
        private void hfButton_Click(object sender, EventArgs e)
        {
            string[] coordinates = ReadTxt("last.txt").Split(',');
            ok = int.Parse(coordinates[0]);
            ng = int.Parse(coordinates[1]);
            _modbusClient.WriteSingleRegister(0, ok + ng);
            _modbusClient.WriteSingleRegister(2, ng);
            Console.WriteLine(ok + "," + ng);
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
        private void delayBtnM1_Click(object sender, EventArgs e)
        {
            DelayForm delayForm = new DelayForm();
            delayForm.StartPosition = FormStartPosition.CenterScreen;
            delayForm.ShowDialog();
        }
        private void delayBtnM2_Click(object sender, EventArgs e)
        {
            DelayForm delayForm = new DelayForm();
            delayForm.StartPosition = FormStartPosition.CenterScreen;
            delayForm.ShowDialog();
        }
        private void delayBtnM3_Click(object sender, EventArgs e)
        {
            DelayForm delayForm = new DelayForm();
            delayForm.StartPosition = FormStartPosition.CenterScreen;
            delayForm.ShowDialog();
        }

        private void uiButton1_Click(object sender, EventArgs e)
        {
            Creater creater = new Creater();
            creater.StartPosition = FormStartPosition.CenterScreen;
            creater.ShowDialog();
            if (j == 1)
            {
                //UnLock();
                newConfigure.Visible = true;
                deleteButton.Visible = true;
            }
        }
        private void detectclassBtnM1_Click(object sender, EventArgs e)
        {
            ClassForm cf = new ClassForm(GainName());
            cf.StartPosition = FormStartPosition.CenterScreen;
            cf.Text = cameraLabel_M1.Text;
            cf.ShowDialog();
        }
        private void detectclassBtnM2_Click(object sender, EventArgs e)
        {
            ClassForm cf = new ClassForm(GainName());
            cf.StartPosition = FormStartPosition.CenterScreen;
            cf.Text = cameraLabel_M2.Text;
            cf.ShowDialog();
        }
        private void detectclassBtnM3_Click(object sender, EventArgs e)
        {
            ClassForm cf = new ClassForm(GainName());
            cf.StartPosition = FormStartPosition.CenterScreen;
            cf.Text = cameraLabel_M3.Text;
            cf.ShowDialog();
        }

        private void detectclassBtnD1_Click(object sender, EventArgs e)
        {
            ClassForm cf = new ClassForm(GainName());
            cf.StartPosition = FormStartPosition.CenterScreen;
            cf.Text = cameraLabel_X1.Text;
            cf.ShowDialog();
        }
        private void detectclassBtnD2_Click(object sender, EventArgs e)
        {
            ClassForm cf = new ClassForm(GainName());
            cf.StartPosition = FormStartPosition.CenterScreen;
            cf.Text = cameraLabel_X2.Text;
            cf.ShowDialog();
        }
        private void detectclassBtnD3_Click(object sender, EventArgs e)
        {
            ClassForm cf = new ClassForm(GainName());
            cf.StartPosition = FormStartPosition.CenterScreen;
            cf.Text = cameraLabel_X3.Text;
            cf.ShowDialog();
        }

        private void configBtnM1_Click(object sender, EventArgs e)
        {
            string s = "camera";
            string m = "MZ";
            /*            // 创建一个新的线程来运行文件选择对话框
                        Thread thread = new Thread(() => ShowOpenFileDialog(configLabel_M1));
                        thread.SetApartmentState(ApartmentState.STA); // 设置线程为STA模式
                        thread.Start();
                        thread.Join(); // 等待线程完成*/
            ShowOpenFileDialog(configLabel_M1, s, m);
        }
        private void weightBtnM1_1_Click(object sender, EventArgs e)
        {
            string s = "detect";
            string m = "MZ";
            ShowOpenFileDialog(weightLabelM1_1, s, m);
        }
        private void weightBtnM1_2_Click(object sender, EventArgs e)
        {
            string s = "detect";
            string m = "MZ";
            ShowOpenFileDialog(weightLabelM1_2, s, m);
        }

        private void configBtnM2_Click(object sender, EventArgs e)
        {
            string s = "camera";
            string m = "MZ";
            ShowOpenFileDialog(configLabel_M2, s, m);
        }
        private void weightBtnM2_1_Click(object sender, EventArgs e)
        {
            string s = "detect";
            string m = "MZ";
            ShowOpenFileDialog(weightLabelM2_1, s, m);
        }
        private void weightBtnM2_2_Click(object sender, EventArgs e)
        {
            string s = "detect";
            string m = "MZ";
            ShowOpenFileDialog(weightLabelM2_2, s, m);
        }

        private void configBtnM3_Click(object sender, EventArgs e)
        {
            string s = "camera";
            string m = "MZ";
            ShowOpenFileDialog(configLabel_M3, s, m);
        }
        private void weightBtnM3_1_Click(object sender, EventArgs e)
        {
            string s = "detect";
            string m = "MZ";
            ShowOpenFileDialog(weightLabelM3_1, s, m);
        }
        private void weightBtnM3_2_Click(object sender, EventArgs e)
        {
            string s = "detect";
            string m = "MZ";
            ShowOpenFileDialog(weightLabelM3_2, s, m);
        }

        private void CameraBtnD1_Click(object sender, EventArgs e)
        {
            string s = "camera";
            string m = "XZ";
            ShowOpenFileDialog(cameraLabel1);
        }
        private void configBtnD1_Click(object sender, EventArgs e)
        {
            string s = "camera";
            string m = "XZ";
            ShowOpenFileDialog(configLabel_D1, s, m);
        }
        private void weightBtnD1_1_Click(object sender, EventArgs e)
        {
            string s = "detect";
            string m = "XZ";
            ShowOpenFileDialog(weightLabelD1_1, s, m);
        }
        private void weightBtnD1_2_Click(object sender, EventArgs e)
        {
            string s = "detect";
            string m = "XZ";
            ShowOpenFileDialog(weightLabelD1_2, s, m);
        }

        private void CameraBtnD2_Click(object sender, EventArgs e)
        {
            string s = "camera";
            string m = "XZ";
            ShowOpenFileDialog(cameraLabel2, s, m);
        }
        private void configBtnD2_Click(object sender, EventArgs e)
        {
            ShowOpenFileDialog(configLabel_D2);
        }
        private void weightBtnD2_1_Click(object sender, EventArgs e)
        {
            string s = "detect";
            string m = "XZ";
            ShowOpenFileDialog(weightLabelD2_1, s, m);
        }
        private void weightBtnD2_2_Click(object sender, EventArgs e)
        {
            string s = "detect";
            string m = "XZ";
            ShowOpenFileDialog(weightLabelD2_2, s, m);
        }

        private void CameraBtnD3_Click(object sender, EventArgs e)
        {
            string s = "camera";
            string m = "XZ";
            ShowOpenFileDialog(cameraLabel3, s, m);
        }
        private void configBtnD3_Click(object sender, EventArgs e)
        {
            string s = "camera";
            string m = "XZ";
            ShowOpenFileDialog(configLabel_D3, s, m);
        }
        private void weightBtnD3_1_Click(object sender, EventArgs e)
        {
            string s = "detect";
            string m = "XZ";
            ShowOpenFileDialog(weightLabelD3_1, s, m);
        }
        private void weightBtnD3_2_Click(object sender, EventArgs e)
        {
            string s = "detect";
            string m = "XZ";
            ShowOpenFileDialog(weightLabelD3_2, s, m);
        }

        private void ShowOpenFileDialog(Label label)
        {
            try
            {
                // 创建OpenFileDialog实例
                OpenFileDialog openFileDialog = new OpenFileDialog();

                // 设置对话框的标题
                openFileDialog.Title = "选择文件";
                string s = GainName();

                // 设置对话框的初始目录
                openFileDialog.InitialDirectory = @"D:\phone C#\Phone_c#\窗口\窗口\bin\Debug\right_configure\" + s;

                // 设置对话框的文件过滤器
                openFileDialog.Filter = "所有文件 (*.*)|*.*|文本文件 (*.txt)|*.txt";

                // 设置对话框的默认文件过滤器
                openFileDialog.FilterIndex = 1;

                // 设置对话框是否可以多选文件
                openFileDialog.Multiselect = false;

                // 显示对话框并检查用户是否点击了“确定”按钮
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // 获取用户选择的文件路径
                    string filePath = openFileDialog.FileName;

                    // 输出文件路径
                    Console.WriteLine("选择的文件路径: " + filePath);
                    // 在UI线程上修改Label的Text属性
                    label.Text = filePath;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private void ShowOpenFileDialog(Label label, string m, string n)
        {
            try
            {
                // 创建OpenFileDialog实例
                OpenFileDialog openFileDialog = new OpenFileDialog();

                // 设置对话框的标题
                openFileDialog.Title = "选择文件";
                string s = GainName();
                string initialDirectory = Path.Combine(
                    @"D:\phone C#\Phone_c#\窗口\窗口\bin\Debug\right_configure",
                    s,
                    m,
                    n
                );

                openFileDialog.InitialDirectory = initialDirectory;
                // 设置对话框的初始目录
                //openFileDialog.InitialDirectory = @"D:\phone C#\Phone_c#\窗口\窗口\bin\Debug\right_configure" + @"\" + s + @"\" + "camera" + @"\" + "MZ";

                // 设置对话框的文件过滤器
                openFileDialog.Filter = "所有文件 (*.*)|*.*|文本文件 (*.txt)|*.txt";

                // 设置对话框的默认文件过滤器
                openFileDialog.FilterIndex = 1;

                // 设置对话框是否可以多选文件
                openFileDialog.Multiselect = false;

                // 显示对话框并检查用户是否点击了“确定”按钮
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // 获取用户选择的文件路径
                    string filePath = openFileDialog.FileName;

                    // 输出文件路径
                    Console.WriteLine("选择的文件路径: " + filePath);
                    // 在UI线程上修改Label的Text属性
                    label.Text = filePath;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        string classD1_1;
        string classD1_2;
        string classD2_1;
        string classD2_2;
        string classD3_1;
        string classD3_2;
        string classM1_1;
        string classM1_2;
        string classM2_1;
        string classM2_2;
        string classM3_1;
        string classM3_2;
        private void ReadCsv_WriteForm(string path)
        {
            // 读取CSV文件
            string[] lines = File.ReadAllLines(path);

            // 获取CSV文件的行数和列数
            int rowCount = lines.Length;
            int colCount = lines[0].Split(',').Length;

            // 创建二维数组
            string[,] data = new string[rowCount, colCount];

            // 将CSV文件的内容存储到二维数组中
            for (int i = 0; i < rowCount; i++)
            {
                string[] fields = lines[i].Split(',');
                for (int j = 0; j < colCount; j++)
                {
                    data[i, j] = fields[j];
                }
            }
            /*            weightLabelD1_1.Text = data[4, 1];
                        weightLabelD1_2.Text = data[34, 1];
                        configLabel_D1.Text = data[62, 1];
                        cameraLabel1.Text = data[63, 1];
                        sizeBoxD1_1.Text = data[5, 1];
                        zxdBoxD1_1.Text = data[6, 1];
                        iouBoxD1_1.Text = data[7, 1];
                        sizeBoxD1_2.Text = data[35, 1];
                        zxdBoxD1_2.Text = data[36, 1];
                        iouBoxD1_2.Text = data[37, 1];

                        weightLabelM1_1.Text = data[16, 1];
                        weightLabelM1_2.Text = data[46, 1];
                        configLabel_M1.Text = data[66, 1];
                        sizeBoxM1_1.Text = data[17, 1];
                        zxdBoxM1_1.Text = data[18, 1];
                        iouBoxM1_1.Text = data[19, 1];
                        sizeBoxM1_2.Text = data[47, 1];
                        zxdBoxM1_2.Text = data[48, 1];
                        iouBoxM1_2.Text = data[49, 1];
            */
            
            //线阵相机
            cameraLabel1.Text = data[0, 1];
            configLabel_D1.Text = data[1, 1];
            weightLabelD1_1.Text = data[2, 1];
            weightLabelD1_2.Text = data[3, 1];
            sizeBoxD1_1.Text = data[4, 1];
            sizeBoxD1_2.Text = data[5, 1];
            iouBoxD1_1.Text = data[6, 1];
            iouBoxD1_2.Text = data[7, 1];
            zxdBoxD1_1.Text =data[8, 1];
            zxdBoxD1_2.Text =data[9, 1];
            classD1_1 = data[10, 1];
            classD1_2 = data[11, 1];
            if(configLabel_D1.Text != "")
            {
                cameraLabel_X1.Text = "相机1";
                stateBtnD1.Text = "-";
                stateBtnD1.ForeColor = System.Drawing.Color.Red;
            }
            else
            {
                stateBtnD1.Text = "+";
                stateBtnD1.ForeColor = System.Drawing.Color.Green;
            }

            cameraLabel2.Text = data[13, 1];
            configLabel_D2.Text = data[14, 1];
            weightLabelD2_1.Text = data[15, 1];
            weightLabelD2_2.Text = data[16, 1];
            sizeBoxD2_1.Text = data[17, 1];
            sizeBoxD2_2.Text = data[18, 1];
            iouBoxD2_1.Text = data[19, 1];
            iouBoxD2_2.Text = data[20, 1];
            zxdBoxD2_1.Text = data[21, 1];
            zxdBoxD2_2.Text = data[22, 1];
            classD2_1 = data[23, 1];
            classD2_2 = data[24, 1];
            if (configLabel_D2.Text != "")
            {
                cameraLabel_X2.Text = "相机2";
                stateBtnD2.Text = "-";
                stateBtnD2.ForeColor = System.Drawing.Color.Red;
            }
            else
            {
                stateBtnD2.Text = "+";
                stateBtnD2.ForeColor = System.Drawing.Color.Green;
            }

            cameraLabel3.Text = data[26, 1];
            configLabel_D3.Text = data[27, 1];
            weightLabelD3_1.Text = data[28, 1];
            weightLabelD3_2.Text = data[29, 1];
            sizeBoxD3_1.Text = data[30, 1];
            sizeBoxD3_2.Text = data[31, 1];
            iouBoxD3_1.Text = data[32, 1];
            iouBoxD3_2.Text = data[33, 1];
            zxdBoxD3_1.Text = data[34, 1];
            zxdBoxD3_2.Text = data[35, 1];
            classD3_1 = data[36, 1];
            classD3_2 = data[37, 1];
            if (configLabel_D3.Text != "")
            {
                cameraLabel_X3.Text = "相机3";
                stateBtnD3.Text = "-";
                stateBtnD3.ForeColor = System.Drawing.Color.Red;
            }
            else
            {
                stateBtnD3.Text = "+";
                stateBtnD3.ForeColor = System.Drawing.Color.Green;
            }
            //面阵相机
            configLabel_M1.Text = data[39, 1];
            weightLabelM1_1.Text = data[40, 1];
            weightLabelM1_2.Text = data[41, 1];
            sizeBoxM1_1.Text = data[42, 1];
            sizeBoxM1_2.Text = data[43, 1];
            iouBoxM1_1.Text = data[44, 1];
            iouBoxM1_2.Text = data[45, 1];
            zxdBoxM1_1.Text = data[46, 1];
            zxdBoxM1_2.Text = data[47, 1];
            classM1_1 = data[48, 1];
            classM1_2 = data[49, 1];
            if (configLabel_M1.Text != "")
            {
                cameraLabel_M1.Text = "相机1";
                stateBtnM1.Text = "-";
                stateBtnM1.ForeColor = System.Drawing.Color.Red;
            }
            else
            {
                stateBtnM1.Text = "+";
                stateBtnM1.ForeColor = System.Drawing.Color.Green;
            }

            configLabel_M2.Text = data[51, 1];
            weightLabelM2_1.Text = data[52, 1];
            weightLabelM2_2.Text = data[53, 1];
            sizeBoxM2_1.Text = data[54, 1];
            sizeBoxM2_2.Text = data[55, 1];
            iouBoxM2_1.Text = data[56, 1];
            iouBoxM2_2.Text = data[57, 1];
            zxdBoxM2_1.Text = data[58, 1];
            zxdBoxM2_2.Text = data[59, 1];
            classM2_1 = data[60, 1];
            classM2_2 = data[61, 1];
            if (configLabel_M2.Text != "")
            {
                cameraLabel_M2.Text = "相机2";
                stateBtnM2.Text = "-";
                stateBtnM2.ForeColor = System.Drawing.Color.Red;
            }
            else
            {
                stateBtnM2.Text = "+";
                stateBtnM2.ForeColor = System.Drawing.Color.Green;
            }

            configLabel_M3.Text = data[63, 1];
            weightLabelM3_1.Text = data[64, 1];
            weightLabelM3_2.Text = data[65, 1];
            sizeBoxM3_1.Text = data[66, 1];
            sizeBoxM3_2.Text = data[67, 1];
            iouBoxM3_1.Text = data[68, 1];
            iouBoxM3_2.Text = data[69, 1];
            zxdBoxM3_1.Text = data[70, 1];
            zxdBoxM3_2.Text = data[71, 1];
            classM3_1 = data[72, 1];
            classM3_2 = data[73, 1];
            if (configLabel_M3.Text != "")
            {
                cameraLabel_M3.Text = "相机3";
                stateBtnM3.Text = "-";
                stateBtnM3.ForeColor = System.Drawing.Color.Red;
            }
            else
            {
                stateBtnM3.Text = "+";
                stateBtnM3.ForeColor = System.Drawing.Color.Green;
            }
            /*string class1aike2 = data[25, 1];
            string class1aike2x = data[55, 1];
            string classm3 = data[28, 1];
            string classm3x = data[58, 1];*/

            /*// 输出二维数组的内容
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < colCount; j++)
                {
                    Console.Write(data[i, j] + " ");
                }
                Console.WriteLine();
            }*/
        }
        private void WriteCSV(string path)
        {
            // 读取整个 CSV 文件
            string[][] data = ReadCsvFile(path);
            //线阵相机
            data[0][1] = cameraLabel1.Text;
            data[1][1] = configLabel_D1.Text;
            data[2][1] = weightLabelD1_1.Text;
            data[3][1] = weightLabelD1_2.Text;
            data[4][1] = sizeBoxD1_1.Text;
            data[5][1] = sizeBoxD1_2.Text;
            data[6][1] = iouBoxD1_1.Text;
            data[7][1] = iouBoxD1_2.Text;
            data[8][1] = zxdBoxD1_1.Text;
            data[9][1] = zxdBoxD1_2.Text;
            data[10][1] = classD1_1;
            data[11][1] = classD1_2;
            
            data[13][1] = cameraLabel2.Text;
            data[14][1] = configLabel_D2.Text;
            data[15][1] = weightLabelD2_1.Text;
            data[16][1] = weightLabelD2_2.Text;
            data[17][1] = sizeBoxD2_1.Text;
            data[18][1] = sizeBoxD2_2.Text;
            data[19][1] = iouBoxD2_1.Text;
            data[20][1] = iouBoxD2_2.Text;
            data[21][1] = zxdBoxD2_1.Text;
            data[22][1] = zxdBoxD2_2.Text;
            data[23][1] = classD2_1;
            data[24][1] = classD2_2;

            data[26][1] = cameraLabel3.Text;
            data[27][1] = configLabel_D3.Text;
            data[28][1] = weightLabelD3_1.Text;
            data[29][1] = weightLabelD3_2.Text;
            data[30][1] = sizeBoxD3_1.Text;
            data[31][1] = sizeBoxD3_2.Text;
            data[32][1] = iouBoxD3_1.Text;
            data[33][1] = iouBoxD3_2.Text;
            data[34][1] = zxdBoxD3_1.Text;
            data[35][1] = zxdBoxD3_2.Text;
            data[36][1] = classD3_1;
            data[37][1] = classD3_2;

            //面阵相机
            data[39][1] = configLabel_M1.Text;
            data[40][1] = weightLabelM1_1.Text;
            data[41][1] = weightLabelM1_2.Text;
            data[42][1] = sizeBoxM1_1.Text;
            data[43][1] = sizeBoxM1_2.Text;
            data[44][1] = iouBoxM1_1.Text;
            data[45][1] = iouBoxM1_2.Text;
            data[46][1] = zxdBoxM1_1.Text;
            data[47][1] = zxdBoxM1_2.Text;
            data[48][1] = classM1_1;
            data[49][1] = classM1_2;

            data[51][1] = configLabel_M2.Text;
            data[52][1] = weightLabelM2_1.Text;
            data[53][1] = weightLabelM2_2.Text;
            data[54][1] = sizeBoxM2_1.Text;
            data[55][1] = sizeBoxM2_2.Text;
            data[56][1] = iouBoxM2_1.Text;
            data[57][1] = iouBoxM2_2.Text;
            data[58][1] = zxdBoxM2_1.Text;
            data[59][1] = zxdBoxM2_2.Text;
            data[60][1] = classM2_1;
            data[61][1] = classM2_2;

            data[63][1] = configLabel_M3.Text;
            data[64][1] = weightLabelM3_1.Text;
            data[65][1] = weightLabelM3_2.Text;
            data[66][1] = sizeBoxM3_1.Text;
            data[67][1] = sizeBoxM3_2.Text;
            data[68][1] = iouBoxM3_1.Text;
            data[69][1] = iouBoxM3_2.Text;
            data[70][1] = zxdBoxM3_1.Text;
            data[71][1] = zxdBoxM3_2.Text;
            data[72][1] = classM3_1;
            data[73][1] = classM3_2;
            // 将更新后的数据写回 CSV 文件
            try
            {
                using (StreamWriter writer = new StreamWriter(path))
                {
                    for (int i = 0; i < data.Length; i++)
                    {
                        writer.WriteLine(string.Join(",", data[i]));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("写入 CSV 文件时出错: " + ex.Message);
            }
        }
        private void ReadData1(int i)
        {
            string constring = "data source=localhost;database=test;user id=root;password=123456;";
            using (MySqlConnection connection = new MySqlConnection(constring))
            {
                try
                {
                    // 打开连接
                    connection.Open();

                    // 使用参数化查询来避免SQL注入
                    string query1 = "SELECT * FROM employ WHERE id = @id;";
                    // 创建命令对象
                    MySqlCommand command1 = new MySqlCommand(query1, connection);
                    command1.Parameters.AddWithValue("@id", i);

                    // 创建数据读取器
                    MySqlDataReader reader1 = command1.ExecuteReader();

                    if (reader1.Read())
                    {
                        int id = reader1.GetInt32("id");
                        string jsonData1 = reader1.GetString("json_data");

                        try
                        {
                            JObject jsonObject1 = JObject.Parse(jsonData1);
                            foreach (var property in jsonObject1.Properties())
                            {
                                if (property.Name.StartsWith("Department"))
                                {
                                    string department = property.Value.ToString();
                                    Console.WriteLine($"ID: {id}, Department: {department}\n");
                                    //m++;
                                    // 在UI线程上更新UI
                                    this.Invoke((MethodInvoker)delegate
                                    {
                                        // 假设你有一个名为textBox1的TextBox控件
                                        // textBox1.AppendText($"ID: {id}, Department: {department}\n");
                                    });
                                }
                                /*                                else
                                                                {
                                                                    Console.WriteLine($"ID: {id}, Department: null\n");
                                                                }*/
                            }
                        }
                        catch (JsonException ex)
                        {
                            // 在UI线程上显示错误信息
                            this.Invoke((MethodInvoker)delegate
                            {
                                MessageBox.Show("Error parsing JSON: " + ex.Message);
                            });
                        }
                    }
                    else
                    {
                        // 在UI线程上显示错误信息
                        this.Invoke((MethodInvoker)delegate
                        {
                            MessageBox.Show("No data found for the given ID.");
                        });
                    }
                }
                catch (Exception ex)
                {
                    // 在UI线程上显示错误信息
                    this.Invoke((MethodInvoker)delegate
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    });
                }
                finally
                {
                    // 关闭连接
                    connection.Close();
                }
            }
        }

        private void yuantuBtn_Click(object sender, EventArgs e)
        {
            //ReadData1(1);
            OpenFolder("E:\\result");
        }

        private void jieguoBtn_Click(object sender, EventArgs e)
        {
            OpenFolder("E:\\result_source");
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        public void OpenFolder(string folderPath)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                Arguments = folderPath,
                FileName = "explorer.exe"
            };

            Process.Start(startInfo);
        }

        private void qkbutton_Click_1(object sender, EventArgs e)
        {
            oklabel.Text = "0";
            errlabel.Text = "0";
            nglabel.Text = "0";
        }
        private void Statement(Label label)
        {
            if (label.Text != "NUll")
            {
                CreateM create = new CreateM();
            }
            else
            {
                CreateM create = new CreateM();
            }
        }
        private void stateBtnM1_Click(object sender, EventArgs e)
        {
            Statement(cameraLabel_M1);
        }
        private void stateBtnM2_Click(object sender, EventArgs e)
        {
            Statement(cameraLabel_M2);
        }
        private void stateBtnM3_Click(object sender, EventArgs e)
        {
            Statement(cameraLabel_M3);
        }

        private void stateBtnD1_Click(object sender, EventArgs e)
        {
            Statement(cameraLabel_X1);
        }
        private void stateBtnD2_Click(object sender, EventArgs e)
        {
            Statement(cameraLabel_X2);
        }
        private void stateBtnD3_Click(object sender, EventArgs e)
        {
            Statement(cameraLabel_X3);
        }

        string s = "./right_configure/" + GainName() + "/right_configure.csv";
        private void renewBtnM1_Click(object sender, EventArgs e)
        {
            WriteCSV(s);
        }
        private void renewBtnM2_Click(object sender, EventArgs e)
        {
            WriteCSV(s);
        }

        private void renewBtnM3_Click(object sender, EventArgs e)
        {
            WriteCSV(s);
        }

        private void renewBtnD1_Click(object sender, EventArgs e)
        {
            WriteCSV(s);
        }

        private void renewBtnD2_Click(object sender, EventArgs e)
        {
            WriteCSV(s);
        }

        private void renewBtnD3_Click(object sender, EventArgs e)
        {
            WriteCSV(s);
        }
        string savePath;
        private void SaveBtn_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if(dialog.ShowDialog() == DialogResult.OK)
            {
                savePath = dialog.SelectedPath;
                //Console.WriteLine(savePath);
                MoveFolder("E:\\result", savePath);
                MoveFolder("E:\\result_source", savePath);
                CreateFolder();
            }
        }
        void MoveFolder(string source, string destination)
        {
            // 确保源文件夹存在
            if (!Directory.Exists(source))
            {
                throw new DirectoryNotFoundException("源文件夹不存在: " + source);
            }

            // 确保目标文件夹存在，如果不存在就创建
            if (!Directory.Exists(destination))
            {
                Directory.CreateDirectory(destination);
            }

            // 获取目标文件夹的路径（包含源文件夹的名称）
            string destinationPath = Path.Combine(destination, Path.GetFileName(source));

            // 如果目标文件夹已存在，抛出异常
            if (Directory.Exists(destinationPath))
            {
                throw new IOException("目标文件夹已存在: " + destinationPath);
            }

            // 移动文件夹到目标文件夹
            Directory.Move(source, destinationPath);
        }
        void CreateFolder()
        {
            string path = "E:\\result";
            string path1 = "E:\\result_source";
            Directory.CreateDirectory(path);
            Directory.CreateDirectory(path1);

            string path_aike2 = path + "\\aike2";
            string path_l3 = path + "\\l3";
            string path_l4 = path + "\\l4";
            Directory.CreateDirectory(path_aike2);
            Directory.CreateDirectory(path_l3);
            Directory.CreateDirectory(path_l4);

            string path1_aike2 = path1 + "\\aike2";
            string path1_l3 = path1 + "\\l3";
            string path1_l4 = path1 + "\\l4";
            Directory.CreateDirectory(path1_aike2);
            Directory.CreateDirectory(path1_l3);
            Directory.CreateDirectory(path1_l4);
        }
    }
}