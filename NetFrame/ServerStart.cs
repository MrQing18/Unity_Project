using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetFrame
{
    class ServerStart
    {
        Socket server;
        int maxClient;
        Semaphore acceptClients;
        UserTokenPool pool;
        // 初始化通信监听
        public ServerStart(int max)
        {
            // 实例化监听对象
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            maxClient = max;
            // 创建连接池
            pool = new UserTokenPool(max);
            acceptClients = new Semaphore(max, max);
            for(int i = 0; i< max; i++)
            {
                UserToken token = new UserToken();
                // 初始化token信息
                pool.push(token);
            }
        }
        public void Start(int port)
        {
            // 监听当前服务器网卡所有可用的IP地址port端口
            server.Bind(new IPEndPoint(IPAddress.Any, port));
            // 置于监听状态
            server.Listen(10);
            StartAccept(null);
        }
        // 开始客户端连接监听
        public void StartAccept(SocketAsyncEventArgs e)
        {
            // 如果当前传入为空，说明调用新的客户端连接监听事件，否则移除当前客户端连接
            if (e == null)
            {
                e = new SocketAsyncEventArgs();
                e.Completed += new EventHandler<SocketAsyncEventArgs>(Accept_Completed);
            }
            else
            {
                e.AcceptSocket = null;
            }
            // 信号量 -1
            acceptClients.WaitOne();
            bool result = server.AcceptAsync(e);
            // 判断异步事件是否挂起，没挂起说明立刻执行完成，直接处理事件，否则会在处理完成后触发accept_Completed事件
            if (!result)  // 不是马上完成
            {
                ProcessAccept(e);
            }
        }
        public void ProcessAccept(SocketAsyncEventArgs e)
        {
            UserToken token = pool.pop();
        }
        public void Accept_Completed(Object sender, SocketAsyncEventArgs e)
        {
            ProcessAccept(e);
        }
    }
}
