using System;
using System.Threading;

public class WatchDog
{
    private readonly Timer _timer;
    private readonly int _timeout;
    private DateTime _lastPingTime;
    private int _lastReceivedValue;

    public WatchDog(int timeout)
    {
        _timeout = timeout;
        _timer = new Timer(CheckStatus, null, 0, 1000); // 每秒检查一次
    }

    public void Ping(int value)
    {
        _lastPingTime = DateTime.Now;
        _lastReceivedValue = value;
    }

    private void CheckStatus(object state)
    {
        
        if (_lastReceivedValue != 0)
        {
            Console.WriteLine("接收到的值不是1，关闭程序...");
            RestartApplication();
        }
    }

    private void RestartApplication()
    {
        Environment.Exit(0);
        Console.WriteLine("结果是1");
    }
}