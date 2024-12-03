using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace 窗口
{
    internal class SwichCamera
    {
        public SwichCamera(Button button,int x,int y)
        {
            if (button.Text != "(新建相机)")
            {
                new OpenCamera(button, button.Text, x, y);
            }
            else
            {
                //CreateCamera createCamera = new CreateCamera(button,label);
                MessageBox.Show("是否建立新相机");
                //createCamera.StartPosition = FormStartPosition.CenterScreen;
                //createCamera.Show();
            }
        }
    }
}
