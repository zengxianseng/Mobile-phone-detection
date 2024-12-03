using System;
using System.Windows.Forms;

namespace test1
{
    public partial class Form1 : Form
    {
        private Timer pingTimer;
        private int x;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Program.watchDog = new WatchDog(10); // 初始化 watchDog

            pingTimer = new Timer();
            pingTimer.Interval = 1000; // 1000ms
            pingTimer.Tick += PingTimer_Tick; // 订阅 Elapsed 事件
            pingTimer.Start();
        }

        private void PingTimer_Tick(object sender, EventArgs e)
        {
            Program.watchDog.Ping(x); // 发送当前的 x 值给看门狗
        }

        private void button1_Click(object sender, EventArgs e)
        {
            x = 1;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            x = 0;
        }
    }
}