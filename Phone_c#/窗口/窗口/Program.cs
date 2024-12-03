using System;
using System.Threading;
using System.Windows.Forms;
using 窗口;

namespace 窗口
{
    internal static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            // 启用异常捕获
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            // 订阅应用程序未捕获异常事件
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Application.ThreadException += Application_ThreadException;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        /// <summary>
        /// 处理应用程序未捕获的异常
        /// </summary>
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            if (ex != null)
            {
                // 弹出异常信息
                MessageBox.Show("应用程序发生未处理的异常:\n" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 处理 UI 线程中的未捕获异常
        /// </summary>
        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            // 弹出 UI 线程异常信息
            MessageBox.Show("UI 线程发生未处理的异常:\n" + e.Exception.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
