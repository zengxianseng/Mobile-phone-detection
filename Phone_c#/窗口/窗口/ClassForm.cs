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
    public partial class ClassForm : Form
    {
        string name;
        public ClassForm(string name)
        {
            this.name = name;
            InitializeComponent();
        }

        private void SelectBtn1_Click(object sender, EventArgs e)
        {
            ShowOpenFileDialog(address1);
        }

        private void SelectBtn2_Click(object sender, EventArgs e)
        {
            ShowOpenFileDialog(address2);
        }
        private void ShowOpenFileDialog(Label label)
        {
            try
            {
                // 创建OpenFileDialog实例
                OpenFileDialog openFileDialog = new OpenFileDialog();

                // 设置对话框的标题
                openFileDialog.Title = "选择文件";
                string s = name;

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
    }
}
