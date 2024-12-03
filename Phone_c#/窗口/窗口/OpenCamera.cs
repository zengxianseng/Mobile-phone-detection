using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace 窗口
{
    internal class OpenCamera
    {
        public OpenCamera(Button button, string s, int i, int j)
        {
            Open(button, s, i, j);
        }
        public void Open(Button button, string s,int i, int j)
        {
            CameraForm cameraForm = new CameraForm(button);
            cameraForm.StartPosition = FormStartPosition.CenterScreen;
            cameraForm.Text = s;
            cameraForm.label2.Text = i.ToString();//延迟
            cameraForm.label4.Text = j.ToString();//曝光
            cameraForm.ShowDialog();
        }
    }
}
