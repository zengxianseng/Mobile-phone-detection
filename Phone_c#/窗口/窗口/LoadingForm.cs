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
    public partial class LoadingForm : Form
    {
        public LoadingForm()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedDialog;//固定对话框
            this.ControlBox = false;//禁用关闭最小化和最大化
            this.StartPosition = FormStartPosition.CenterScreen;
            //this.Load += LoadingForm_load;
        }
    }
}
