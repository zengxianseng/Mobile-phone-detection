using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using gts;

namespace 窗口
{
    public partial class shezhi : Form
    {
        private Stopwatch stopwatch = new Stopwatch();
        private System.Windows.Forms.Timer timer;
        bool issending =false ;
        int lGpiValue;
        uint clk;//时钟参数
        double[] enc = new double[2];
        private bool w0 = false;//初始开关为关
        int a;
        int a1;
        short t = 400;
        public shezhi()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            timer1.Start();
        }

        private void shezhi_Load(object sender, EventArgs e)
        {
            
        }
        private void timer1_Tick(object sender, EventArgs e) 
        {
            gts.mc.GT_GetDiRaw(mc.MC_GPI, out lGpiValue);
            a1 = lGpiValue & (1 << 15);//读取exi15
            a1 = (a1 != 32768) ? 0 : 1;
            label1.Text = a1.ToString();
            if (a1 == 1 && !issending)
            {
                issending = true;
                new Thread(() =>
                {
                    Thread.Sleep(1000); //毫秒

                    // 执行 gts.mc.GT_SetDoBitReverse
                    gts.mc.GT_SetDoBitReverse(
                        gts.mc.MC_GPO,
                        11, // 第15号位置
                        0,  // 高电平
                        4  // 4代表持续1ms的高电平
                    );
                    gts.mc.GT_SetDoBitReverse(
                        gts.mc.MC_GPO,
                        10, // 第15号位置
                        0,  // 高电平
                        4  // 4代表持续1ms的高电平
                    );
                }).Start();
                new Thread(() =>
                {
                    Thread.Sleep(1000); //毫秒
                                                        // 执行 gts.mc.GT_SetDoBitReverse
                    gts.mc.GT_SetDoBitReverse(
                        gts.mc.MC_GPO,
                        11, // 第15号位置
                        0,  // 高电平
                        4  // 4代表持续1ms的高电平
                    );
                    gts.mc.GT_SetDoBitReverse(
                        gts.mc.MC_GPO,
                        10, // 第15号位置
                        0,  // 高电平
                        4  // 4代表持续1ms的高电平
                    );
                }).Start();
                new Thread(() =>
                {
                    Thread.Sleep(1000); //毫秒
                                                        // 执行 gts.mc.GT_SetDoBitReverse
                    gts.mc.GT_SetDoBitReverse(
                        gts.mc.MC_GPO,
                        11, // 第15号位置
                        0,  // 高电平
                        4  // 4代表持续1ms的高电平
                    );
                    gts.mc.GT_SetDoBitReverse(
                        gts.mc.MC_GPO,
                        10, // 第15号位置
                        0,  // 高电平
                        4  // 4代表持续1ms的高电平
                    );
                    issending = false;
                }).Start();

                //gts.mc.GT_SetDoBitReverse(
                //        gts.mc.MC_GPO,
                //        10, // EXO 9位置
                //        0,  // 高电平
                //        t  // 4代表持续1ms的高电平
                //    );
                //gts.mc.GT_SetDoBitReverse(
                //        gts.mc.MC_GPO,
                //        11, // EXO 10位置
                //        0,  // 高电平
                //        t  // 4代表持续1ms的高电平
                //    );
                //gts.mc.GT_SetDoBitReverse(
                //        gts.mc.MC_GPO,
                //        5, // EXO 4位置
                //        0,  // 高电平
                //        t  // 4代表持续1ms的高电平
                //    );
                //gts.mc.GT_SetDoBitReverse(
                //        gts.mc.MC_GPO,
                //        7, // EXO 6位置
                //        0,  // 高电平
                //        t  // 4代表持续1ms的高电平
                //    );
                //gts.mc.GT_SetDoBitReverse(
                //        gts.mc.MC_GPO,
                //        9, // EXO 8位置
                //        0,  // 高电平
                //        t  // 4代表持续1ms的高电平
                //    );
            }
}

private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            t = short.Parse(textBox1.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            gts.mc.GT_SetDoBitReverse(
                    gts.mc.MC_GPO,
                    11, // EXO 10位置
                    0,  // 高电平
                    400  // 4代表持续1ms的高电平
                );
            gts.mc.GT_SetDoBitReverse(
                gts.mc.MC_GPO,
                10, // EXO 10位置
                0,  // 高电平
                0  // 4代表持续1ms的高电平
                );
        }
    }
}
