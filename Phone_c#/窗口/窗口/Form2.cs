/*using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static ControllerDllCSharp.ClassLibControllerDll;

namespace 窗口
{
    using static System.Windows.Forms.VisualStyles.VisualStyleElement;
    using ControllerHandle = Int64;
    public partial class Form2 : Form
    {
        private ControllerHandle controllerHandle = 0;
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }
        private void button1_Click(object sender, EventArgs e)
        {
            int value = Int32.Parse(label1.Text) + 5;
            int channelIndex = 1;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            if (SetDigitalValue(channelIndex, value, controllerHandle) == SUCCESS)
            {
                sw.Stop();
                long t = sw.ElapsedMilliseconds;
                label1.Text = value.ToString();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int value = Int32.Parse(label1.Text) - 5;
            int channelIndex = 1;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            if (SetDigitalValue(channelIndex, value, controllerHandle) == SUCCESS)
            {
                sw.Stop();
                long t = sw.ElapsedMilliseconds;
                label1.Text = value.ToString();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (button3.Text == "连接串口")
            {
                int a = CreateSerialPort_Baud(5, 19200, ref controllerHandle);
                if (a == SUCCESS)
                {
                    button3.Text = "断开串口";
                    MessageBox.Show("打开串口成功");
                    int value = 0;
                    int channelIndex = 1;
                    if (GetDigitalValue(ref value, 1, controllerHandle) == SUCCESS)
                    {
                        label1.Text = value.ToString();
                    }
                    else
                    {

                    }
                }
                else
                {
                    MessageBox.Show("打开串口失败");
                }
            }
            else if (button3.Text == "断开串口")
            {
                int a = ReleaseSerialPort(controllerHandle);  //断开串口
                if (a == SUCCESS)
                {
                    button3.Text = "连接串口";
                    MessageBox.Show("关闭串口成功");
                }
                else
                {
                    MessageBox.Show("关闭串口失败");
                }
            }
        }
    }
}
*/