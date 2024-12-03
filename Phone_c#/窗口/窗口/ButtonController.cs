using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ControllerDllCSharp.ClassLibControllerDll;

namespace 窗口
{
    using ControllerHandle = Int64;
    internal class ButtonController
    {
        private int valuel = 0;
        private ControllerHandle controllerHandle;
        private CreateCamera createCamera;
        public ButtonController(CreateCamera createCamera)
        {
            this.createCamera = createCamera;
        }

        public void InitializeButtons(Button upButton1, Button downButton1, Button upButton2, Button downButton2, Button upButton3, Button downButton3, Button upButton4, Button downButton4)
        {
            upButton1.Click += (sender, e) => createCamera.Up1();
            downButton1.Click += (sender, e) => createCamera.Down1();
            upButton2.Click += (sender, e) => createCamera.Up2();
            downButton2.Click += (sender, e) => createCamera.Down2();
            upButton3.Click += (sender, e) => createCamera.Up3();
            downButton3.Click += (sender, e) => createCamera.Down3();
            upButton4.Click += (sender, e) => createCamera.Up4();
            downButton4.Click += (sender, e) => createCamera.Down4();
        }
        }
    }
