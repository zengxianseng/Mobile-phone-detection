using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 窗口
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public class TcpServer
    {
        private TcpListener server;
        private bool isRunning;
        public async Task StartServerAsync(int[] q)
        {
            
            IPAddress ip = IPAddress.Parse("192.168.0.9");
            server = new TcpListener(ip, 502);
            server.Start();
            isRunning = true;

            Console.WriteLine("Server started. Waiting for a connection...");

            TcpClient client = await server.AcceptTcpClientAsync();
            Console.WriteLine("Client connected.");

            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];

            try
            {
                while (isRunning)
                {
                    // 每隔100毫秒读取一次数据
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                    // 如果没有读取到数据，跳出循环
                    if (bytesRead == 0)
                    {
                        Console.WriteLine("Client disconnected.");
                        break;
                    }

                    string receivedMessage = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"Received: {receivedMessage}");
                    int code = int.Parse(receivedMessage.Substring(0, receivedMessage.Length - 1));
                    int state = int.Parse(receivedMessage.Substring(receivedMessage.Length - 1));
                    q[code] = state;
                    Console.WriteLine(q[code]);
                    Console.WriteLine($"q[{code}] = {state}");
                    GCHandle handle = GCHandle.Alloc(q, GCHandleType.Pinned);
                    try
                    {
                        int index = 1; // 指定要输出地址的元素索引
                        IntPtr ptr = Marshal.UnsafeAddrOfPinnedArrayElement(q, index);
                        Console.WriteLine($"输出plc {index} 的地址: {ptr}");
                    }
                    finally
                    {
                        // 释放句柄
                        handle.Free();
                    }
                    // 向发送方发送确认消息
                    string responseMessage = "Message received";
                    byte[] responseBuffer = Encoding.ASCII.GetBytes(responseMessage);
                    await stream.WriteAsync(responseBuffer, 0, responseBuffer.Length);
                    Console.WriteLine("Response sent to client.");
                    // 等待100毫秒
                    await Task.Delay(100);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                // 清理资源
                client.Close();
                server.Stop();
                isRunning = false;
                Console.WriteLine("Server stopped.");
            }
        }
        public void shut()
        {
            isRunning = false; 
        }
    }
}
