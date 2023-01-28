/************************************************************************************
* Copyright (c) 2019 All Rights Reserved.
*命名空间：TCPHelper
*文件名： BYSerial.ServerAsync
*创建人： Yuanbao.Xu
*创建时间：2019/1/18 13:22:46
*描述
*=====================================================================
*修改标记
*修改时间：2019/1/18 13:22:46
*修改人：Yuanbao.Xu
*描述：
************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace BYSerial.TCPHelper
{
   public class ServerAsync
    {
        private TcpListener listener = null;

        //用于控制异步接受连接
        private ManualResetEvent doConnect = new ManualResetEvent(false);
        //用于控制异步接收数据
        private ManualResetEvent doReceive = new ManualResetEvent(false);
        //标识服务端连接是否关闭
        private bool isClose = false;
        private Dictionary<string, TcpClient> listClient = new Dictionary<string, TcpClient>();
        /// <summary>
        /// 已建立连接的Dictionary集合
        /// key:ip:port
        /// value:TcpClient
        /// </summary>
        public Dictionary<string, TcpClient> DicListClient
        {
            get { return listClient; }
            private set { listClient = value; }
        }
        /// <summary>
        /// 连接、发送、关闭事件
        /// </summary>
        public event Action<TcpClient, EnSocketAction> Completed;
        /// <summary>
        /// 接收到数据事件
        /// </summary>
        public event Action<TcpClient,byte[]> Received;

        /// <summary>
        /// 字符串数据传输编码方式
        /// </summary>
        private Encoding Encoding { get; set; } = Encoding.ASCII;
        /// <summary>
        /// 使用默认ASCII传输字符串
        /// </summary>
        public ServerAsync()
        {

        }
        public ServerAsync(Encoding encoding)
        {
            Encoding = encoding;
        }
        /// <summary>
        /// 开始异步监听ip地址的端口
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public void StartAsync(string ip, int port)
        {
            IPAddress ipAddress = null;
            try
            {
                ipAddress = IPAddress.Parse(ip);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            listener = new TcpListener(new IPEndPoint(ipAddress, port));
            listener.Start();
            ThreadPool.QueueUserWorkItem(x =>
            {
                while (!isClose)
                {
                    doConnect.Reset();
                    listener.BeginAcceptTcpClient(AcceptCallBack, listener);
                    doConnect.WaitOne();
                }
            });
        }
        /// <summary>
        /// 开始异步监听本机127.0.0.1的端口号
        /// </summary>
        /// <param name="port"></param>
        public void StartAsync(int port)
        {
            StartAsync("127.0.0.1", port);
        }
        /// <summary>
        /// 开始异步发送数据
        /// </summary>
        /// <param name="key">客户端的ip地址和端口号</param>
        /// <param name="msg">要发送的内容</param>
        public void SendAsync(string key, string msg)
        {
            if (!DicListClient.ContainsKey(key))
            {
                throw new Exception("所用的socket不在字典中,请先连接！");
            }
            TcpClient client = DicListClient[key];
            byte[] arrayData = Encoding.GetBytes(msg); // Encoding.UTF8.GetBytes(msg);
            try
            {               
                client.Client.BeginSend(arrayData, 0, arrayData.Length, SocketFlags.None, SendCallBack, client);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        public void SendAsync(string key, string msg,Encoding encoding)
        {
            if (!DicListClient.ContainsKey(key))
            {
                throw new Exception("所用的socket不在字典中,请先连接！");
            }
            TcpClient client = DicListClient[key];
            Encoding = encoding;
            byte[] arrayData = encoding.GetBytes(msg); // Encoding.UTF8.GetBytes(msg);
            try
            {
                client.Client.BeginSend(arrayData, 0, arrayData.Length, SocketFlags.None, SendCallBack, client);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 开始异步发送数据
        /// </summary>
        /// <param name="key">客户端的ip地址和端口号</param>
        /// <param name="msg">要发送的内容</param>
        public void SendAsync(string key, byte[] msg)
        {
            if (!DicListClient.ContainsKey(key))
            {
                throw new Exception("所用的socket不在字典中,请先连接！");
            }
            TcpClient client = DicListClient[key];            
            try
            {
                client.Client.BeginSend(msg, 0, msg.Length, SocketFlags.None, SendCallBack, client);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 开始异步接收数据
        /// </summary>
        /// <param name="key">要接收的客户端的ip地址和端口号</param>
        private void ReceiveAsync(string key)
        {
            try
            {
                doReceive.Reset();
                if (DicListClient.ContainsKey(key))
                {
                    TcpClient client = DicListClient[key];
                    StateObject obj = new StateObject();
                    obj.Client = client;
                    try
                    {
                        client.Client.BeginReceive(obj.btArrayData, 0, obj.btArrayData.Length, SocketFlags.None, ReceiveCallBack, obj);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        DicListClient.Remove(key);
                        Completed(client, EnSocketAction.Close);
                    }
                    doReceive.WaitOne();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        /// <summary>
        /// 异步接收连接的回调函数
        /// </summary>
        /// <param name="ar"></param>
        private void AcceptCallBack(IAsyncResult ar)
        {
            try
            {
                TcpListener l = ar.AsyncState as TcpListener;
                TcpClient client = l.EndAcceptTcpClient(ar);
                doConnect.Set();

                IPEndPoint iep = client.Client.RemoteEndPoint as IPEndPoint;
                string key = string.Format("{0}:{1}", iep.Address.ToString(), iep.Port);
                if (DicListClient.ContainsKey(key))
                {
                    DicListClient.Remove(key);
                }
                DicListClient.Add(key, client);
                OnComplete(client, EnSocketAction.Connect);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        /// <summary>
        /// 异步发送数据的回调函数
        /// </summary>
        /// <param name="ar"></param>
        private void SendCallBack(IAsyncResult ar)
        {
            try
            {
                TcpClient client = ar.AsyncState as TcpClient;
                IPEndPoint iep = client.Client.RemoteEndPoint as IPEndPoint;
                string key = string.Format("{0}:{1}", iep.Address.ToString(), iep.Port);
                if (Completed != null)
                {
                    Completed(client, EnSocketAction.SendMsg);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        /// <summary>
        /// 异步接收数据的回调函数
        /// </summary>
        /// <param name="ar"></param>
        private void ReceiveCallBack(IAsyncResult ar)
        {
            try
            {
                StateObject obj = ar.AsyncState as StateObject;
                int count = -1;
                try
                {
                    count = obj.Client.Client.EndReceive(ar);
                }
                catch
                {
                    if (!obj.Client.Client.Connected)
                    {
                        IPEndPoint iep = obj.Client.Client.RemoteEndPoint as IPEndPoint;
                        string key = string.Format("{0}:{1}", iep.Address.ToString(), iep.Port);
                        DicListClient.Remove(key);
                        OnComplete(obj.Client, EnSocketAction.Close);
                        doReceive.Set();
                        return;
                    }
                }
                doReceive.Set();
                if (count > 0)
                {
                    string msg = Encoding.GetString(obj.btArrayData, 0, count); 
                    if (!string.IsNullOrEmpty(msg))
                    {
                        if (Received != null)
                        {
                            byte[] brec = new byte[count];
                            Array.Copy(obj.btArrayData, brec, count);
                            Received(obj.Client, brec);//触发接收事件
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }
        public virtual void OnComplete(TcpClient client, EnSocketAction enAction)
        {
            IPEndPoint iep = client.Client.RemoteEndPoint as IPEndPoint;
            string key = string.Format("{0}:{1}", iep.Address.ToString(), iep.Port);            
            if (Completed != null)
                Completed(client, enAction);
            if (enAction == EnSocketAction.Connect)//当连接建立时，则要一直接收
            {
                ThreadPool.QueueUserWorkItem(x =>
                {
                    while (DicListClient.ContainsKey(key) && !isClose)
                    {
                        Thread.Sleep(20);
                        ReceiveAsync(key);
                        Thread.Sleep(20);
                    }
                });

            }
        }
        public void Close()
        {
            try
            {
                isClose = true;                
                listener.Stop();
                if (listClient.Count>0)
                {
                    foreach (TcpClient client in listClient.Values)
                    {
                        client.Close();
                    }
                }
                this.listClient.Clear();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
    
   /// <summary>
   /// 接收socket的行为
   /// </summary>
   public enum EnSocketAction
   {
       /// <summary>
       /// socket发生连接
       /// </summary>
       Connect = 1,
       /// <summary>
       /// socket发送数据
       /// </summary>
       SendMsg = 2,
       /// <summary>
       /// socket关闭
       /// </summary>
       Close = 4
   }
   /// <summary>
   /// 对异步接收时的对象状态的封装，将socket与接收到的数据封装在一起
   /// </summary>
   public class StateObject
   {
       public TcpClient Client { get; set; }
       private byte[] btarrayData = new byte[2048];
       /// <summary>
       /// 接收的数据
       /// </summary>
       public byte[] btArrayData
       {
           get
           {
               return btarrayData;
           }
           set
           {
               btarrayData = value;
           }
       }
   }
}
