using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 窗口
{
    public partial class Creater : Form
    {
        public Creater()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "123456")
            {
                MessageBox.Show("密码错误");
            }
            else
            {
                MessageBox.Show("登录成功");
                this.Close();
                Form1.j = 1;
            }
        }
    }
}
