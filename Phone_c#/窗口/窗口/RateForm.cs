using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using gts;

namespace 窗口
{
    public partial class RateForm : Form
    {
        int lGpiValue;
        uint clk;//时钟参数
        double[] enc = new double[2];
        int a1;
        double l = 0.1185;
        public RateForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            timer1.Start();
            //timer2.Tick += Timer1_Tick0;
            //timer2.Start();
        }

        private void RateForm_Load(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Start();//启动定时器，循环检测编码器位置值
            //读取第2路辅助编码器位置
            gts.mc.GT_GetEncVel(10, out enc[1], 2, out clk);//读取速度
            double v = enc[1] * 0.0200;
            label1.Text =(v).ToString();
            gts.mc.GT_GetDiRaw(mc.MC_GPI, out lGpiValue);
            a1 = lGpiValue & (1<<12);//读取exi13
            a1 = (a1 == 4096) ? 0 : 1;
            label5.Text = a1.ToString();
            if (a1 == 1)
            {
                // 使用线程来实现延迟
                new Thread(() =>
                {
                    Thread.Sleep((int)(l / v * 650)); //毫秒

                    // 执行 gts.mc.GT_SetDoBitReverse
                    gts.mc.GT_SetDoBitReverse(
                        gts.mc.MC_GPO,
                        15, // 第15号位置
                        0,  // 高电平
                        4  // 4代表持续1ms的高电平
                    );
                    new Thread(()=>
                    {
                        Thread.Sleep((int)(l / v * 650)); //毫秒
                                                          // 执行 gts.mc.GT_SetDoBitReverse
                        gts.mc.GT_SetDoBitReverse(
                            gts.mc.MC_GPO,
                            15, // 第15号位置
                            0,  // 高电平
                            4  // 4代表持续1ms的高电平
                        );
                        new Thread(() =>
                        {
                            Thread.Sleep((int)(l / v * 650)); //毫秒
                                                              // 执行 gts.mc.GT_SetDoBitReverse
                            gts.mc.GT_SetDoBitReverse(
                                gts.mc.MC_GPO,
                                15, // 第15号位置
                                0,  // 高电平
                                4  // 4代表持续1ms的高电平
                            );
                        }).Start();//第三个线程
                    }).Start();//第二个线程
                }).Start(); // 启动线程
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            gts.mc.GT_SetDoBitReverse(
                gts.mc.MC_GPO,
                        10, // 第15号位置
                        0,  // 高电平
                        20  // 4代表持续1ms的高电平
             );
        }
    }
}
