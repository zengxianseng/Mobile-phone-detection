using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace 窗口
{
    internal class WatchDog
    {
        private readonly Timer _timer;
        private readonly int _timeout;
        private DateTime _lastPingTime;

        public WatchDog(int timeout)
        {
            _timeout = timeout;
            _timer = new Timer(CheckStatus, null, 0, 1000); // 每秒检查一次
        }

        public void Ping()
        {
            _lastPingTime = DateTime.Now;
        }

        private void CheckStatus(object state)
        {
            if ((DateTime.Now - _lastPingTime).TotalSeconds > _timeout)
            {
                Console.WriteLine("程序无响应，执行恢复操作...");
                // 在这里执行恢复操作，比如重启程序或记录日志
                RestartApplication();
            }
        }

        private void RestartApplication()
        {
            // 实现重启程序的逻辑
            System.Diagnostics.Process.Start(System.Reflection.Assembly.GetExecutingAssembly().Location);
            Environment.Exit(0);
        }
    }
}
