using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace 窗口
{
    internal class DeleteCamera
    {
        public DeleteCamera(Button button, Label label)
        {
            // 显示带有“确定”和“取消”按钮的消息框
            DialogResult result = MessageBox.Show("确定要删除相机嘛？", "确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

            // 根据用户的选择执行相应的操作
            if (result == DialogResult.OK)
            {
                
                MessageBox.Show("相机已删除。");
                label.Text = "NULL";
                button.Text = "+";
                button.ForeColor = System.Drawing.Color.Green;
            }
        }
    }
}
