using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static ControllerDllCSharp.ClassLibControllerDll;

namespace 窗口
{
    using ControllerHandle = Int64;
    public partial class CreateCamera : Form
    {
        public delegate void LightControlDelegate();

        public LightControlDelegate Up1;
        public LightControlDelegate Down1;
        public LightControlDelegate Up2;
        public LightControlDelegate Down2;
        public LightControlDelegate Up3;
        public LightControlDelegate Down3;
        public LightControlDelegate Up4;
        public LightControlDelegate Down4;

        Button button;
        Label label;
        Label light1;
        Label light2;
        Label light3;
        Label light4;
        int channelIndex1 = 0;
        int channelIndex2 = 0;
        int channelIndex3 = 0;
        int channelIndex4 = 0;
        int[] i = new int[4];
        public ControllerHandle controllerHandle = 0;
        public event EventHandler<ControllerHandle> CameraFormClosed;//关闭窗口事件
        public CreateCamera(Button button, Label label,Label light1, Label light2, Label light3, Label light4)
        {
            this.button = button;
            this.light1 = light1;
            this.light2 = light2;
            this.light3 = light3;
            this.light4 = light4;
            this.label = label;
            this.StartPosition = FormStartPosition.CenterScreen;
            InitializeComponent();

            Up1 = new LightControlDelegate(up1);
            Down1 = new LightControlDelegate(down1);
            Up2 = new LightControlDelegate(up2);
            Down2 = new LightControlDelegate(down2);
            Up3 = new LightControlDelegate(up3);
            Down3 = new LightControlDelegate(down3);
            Up4 = new LightControlDelegate(up4);
            Down4 = new LightControlDelegate(down4);
        }//新建相机

        private void button1_Click(object sender, EventArgs e)
        {
            string newString;
            if(textBox1.Text == "")
            {
                MessageBox.Show("相机名称为空");
                return;
            }
            if(comboBox5.SelectedIndex == -1)
            {
                MessageBox.Show("请输入连接光源");
                return;
            }
            label.Text = textBox1.Text;
            button.Text = "-";
            button.ForeColor = Color.Red;
            newString = comboBox5.Text.Substring(3);
            int m = int.Parse(newString);
            int a = CreateSerialPort_Baud(m, 19200, ref controllerHandle);
            if (a != SUCCESS)
            {
                MessageBox.Show("连接串口失败");
                return;
            }

            int value1 = 0;
            MessageBox.Show("连接光源成功");
            channelIndex1 = int.Parse(comboBox1.Text.Substring(2));
            i[0] = channelIndex1;
            channelIndex2 = int.Parse(comboBox2.Text.Substring(2));
            i[1] = channelIndex2;
            channelIndex3 = int.Parse(comboBox3.Text.Substring(2));
            i[2] = channelIndex3;
            channelIndex4 = int.Parse(comboBox4.Text.Substring(2));
            i[3] = channelIndex4;
            if (GetDigitalValue(ref value1, channelIndex1, controllerHandle) == SUCCESS)
            {
                light1.Text = value1.ToString();
            }
            else
            {
                MessageBox.Show(controllerHandle.ToString());
            }
            if (GetDigitalValue(ref value1, channelIndex2, controllerHandle) == SUCCESS)
            {
                light2.Text = value1.ToString();
            }
            if (GetDigitalValue(ref value1, channelIndex3, controllerHandle) == SUCCESS)
            {
                light3.Text = value1.ToString();
            }
            if (GetDigitalValue(ref value1, channelIndex4, controllerHandle) == SUCCESS)
            {
                light4.Text = value1.ToString();
            }

            Judeg();
            LightCsvFile();
            this.Close();
        }
        public int[] ChannelIndex()
        {
            return i;
        }
        private void Judeg()
        {
            int value = 0;
            GetDigitalValue(ref value, channelIndex1, controllerHandle);
            light1.Text = value.ToString();
            GetDigitalValue(ref value, channelIndex2, controllerHandle);
            light2.Text = value.ToString();
            GetDigitalValue(ref value, channelIndex3, controllerHandle);
            light3.Text = value.ToString();
            GetDigitalValue(ref value, channelIndex4, controllerHandle);
            light4.Text = value.ToString();
        }

        public void up1()
        {
            int value = Int32.Parse(light1.Text) + 5;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            if (SetDigitalValue(channelIndex1, value, controllerHandle) == SUCCESS)
            {
                sw.Stop();
                long t = sw.ElapsedMilliseconds;
                light1.Text = value.ToString();
            }
            Judeg();
        }
        public void down1()
        {
            int value = Int32.Parse(light1.Text) - 5;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            if (SetDigitalValue(channelIndex1, value, controllerHandle) == SUCCESS)
            {
                sw.Stop();
                long t = sw.ElapsedMilliseconds;
                light1.Text = value.ToString();
            }
            Judeg();
        }

        public void up2()
        {
            int value = Int32.Parse(light2.Text) + 5;
            int channelIndex = 1;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            if (SetDigitalValue(channelIndex2, value, controllerHandle) == SUCCESS)
            {
                sw.Stop();
                long t = sw.ElapsedMilliseconds;
                light2.Text = value.ToString();
            }
            Judeg();
        }
        public void down2()
        {
            int value = Int32.Parse(light2.Text) - 5;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            if (SetDigitalValue(channelIndex2, value, controllerHandle) == SUCCESS)
            {
                sw.Stop();
                long t = sw.ElapsedMilliseconds;
                light2.Text = value.ToString();
            }
            Judeg();
        }

        public void up3()
        {
            int value = Int32.Parse(light3.Text) + 5;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            if (SetDigitalValue(channelIndex3, value, controllerHandle) == SUCCESS)
            {
                sw.Stop();
                long t = sw.ElapsedMilliseconds;
                light3.Text = value.ToString();
            }
            Judeg();
        }
        public void down3()
        {
            int value = Int32.Parse(light3.Text) - 5;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            if (SetDigitalValue(channelIndex3, value, controllerHandle) == SUCCESS)
            {
                sw.Stop();
                long t = sw.ElapsedMilliseconds;
                light3.Text = value.ToString();
            }
            Judeg();
        }

        public void up4()
        {
            int value = Int32.Parse(light4.Text) + 5;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            if (SetDigitalValue(channelIndex4, value, controllerHandle) == SUCCESS)
            {
                sw.Stop();
                long t = sw.ElapsedMilliseconds;
                light4.Text = value.ToString();
            }
            Judeg();
        }
        public void down4()
        {
            int value = Int32.Parse(light4.Text) - 5;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            if (SetDigitalValue(channelIndex4, value, controllerHandle) == SUCCESS)
            {
                sw.Stop();
                long t = sw.ElapsedMilliseconds;
                light4.Text = value.ToString();
            }
            Judeg();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            // 触发 CameraFormClosed 事件，并传递 controllerHandle 的值
            CameraFormClosed?.Invoke(this, controllerHandle);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            // 清空ComboBox中的现有项
            comboBox5.Items.Clear();

            // 获取所有可用的COM端口
            string[] ports = SerialPort.GetPortNames();

            // 将COM端口添加到ComboBox中
            foreach (string port in ports)
            {
                if (!IsPortInUse(port) && port!="COM13" && port != "COM1" && port!="COM2" && port != "COM3" && port != "COM4" && port != "COM5" && port != "COM6" && port != "COM7") 
                {
                    comboBox5.Items.Add(port);
                }
            }
        }//获取串口信息
        static bool IsPortInUse(string portName)
        {
            try
            {
                using (SerialPort port = new SerialPort(portName))
                {
                    port.Open();
                    return false;
                }
            }
            catch (UnauthorizedAccessException)
            {
                return true;
            }
            catch (Exception ex)
            {
                return true;
            }
        }
        private void LightCsvFile()
        {
            string path = "D:\\phone C#\\Phone_c#\\ff\\CameraLight\\" + Form1.GainName() + ".csv";
            string[][] csvCamera = ReadCsvFile(path);
            string index = button.Name.Substring(6);
            if (index[0] == 'D')
            {
                if (csvCamera != null)
                {
                    int i = int.Parse(index.Substring(1));
                    csvCamera[i - 1][0] = comboBox5.Text;
                    csvCamera[i - 1][1] = comboBox1.Text;
                    csvCamera[i - 1][2] = comboBox2.Text;
                    csvCamera[i - 1][3] = comboBox3.Text;
                    csvCamera[i - 1][4] = comboBox4.Text;
                    try
                    {
                        using (StreamWriter writer = new StreamWriter(path))
                        {
                            for (int j = 0; j < csvCamera.Length; j++)
                            {
                                writer.WriteLine(string.Join(",", csvCamera[j]));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("写入 CSV 文件时出错: " + ex.Message);
                    }
                }
            }
            else
            {
                if (csvCamera != null)
                {
                    int i = int.Parse(index.Substring(1));
                    csvCamera[i + 7][0] = comboBox5.Text;
                    csvCamera[i + 7][1] = comboBox1.Text;
                    csvCamera[i + 7][2] = comboBox2.Text;
                    csvCamera[i + 7][3] = comboBox3.Text;
                    csvCamera[i + 7][4] = comboBox4.Text;
                    try
                    {
                        using (StreamWriter writer = new StreamWriter(path))
                        {
                            for (int j = 0; j < csvCamera.Length; j++)
                            {
                                writer.WriteLine(string.Join(",", csvCamera[j]));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("写入 CSV 文件时出错: " + ex.Message);
                    }
                }
            }
            
        }//保存光源信息，连接的串口和光源接口
        private string[][] ReadCsvFile(string filePath)
        {
            try
            {
                string[] lines = System.IO.File.ReadAllLines(filePath);
                string[][] csvData = new string[lines.Length][];

                for (int i = 0; i < lines.Length; i++)
                {
                    csvData[i] = lines[i].Split(',');
                }

                return csvData;
            }
            catch (Exception ex)
            {
                MessageBox.Show("读取CSV文件时出错: " + ex.Message);
                return null;
            }
        }//读csv配置文件
    }
}
