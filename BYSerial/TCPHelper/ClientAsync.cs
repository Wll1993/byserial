/************************************************************************************
* Copyright (c) 2019 All Rights Reserved.
*命名空间：BYSerial.TCPHelper
*文件名： ClientAsync
*创建人： Yuanbao.Xu
*创建时间：2019/1/18 13:22:19
*描述
*=====================================================================
*修改标记
*修改时间：2019/1/18 13:22:19
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
   public class ClientAsync
    {

        private TcpClient client;
        /// <summary>
        /// 客户端连接完成、发送完成、连接异常或者服务端关闭触发的事件
        /// </summary>
        public event Action<TcpClient, EnSocketAction> Completed;
        /// <summary>
        /// 客户端接收消息触发的事件
        /// </summary>
        public event Action<TcpClient, byte[]> Received;
        /// <summary>
        /// 用于控制异步接收消息
        /// </summary>
        private ManualResetEvent doReceive = new ManualResetEvent(false);
        //标识客户端是否关闭
        private bool isClose = false;
        public bool IsConnected { get; private set; } = false;
        public ClientAsync()
        {
            client = new TcpClient();
        }
        /// <summary>
        /// 异步连接
        /// </summary>
        /// <param name="ip">要连接的服务器的ip地址</param>
        /// <param name="port">要连接的服务器的端口</param>
        public void ConnectAsync(string ip, int port)
        {
            IPAddress ipAddress = null;
            try
            {
                ipAddress = IPAddress.Parse(ip);
            }
            catch (Exception)
            {
                throw new Exception("ip地址格式不正确，请使用正确的ip地址！");
            }
            client.BeginConnect(ipAddress, port, ConnectCallBack, client);
        }
        /// <summary>
        /// 异步连接，连接ip地址为127.0.0.1
        /// </summary>
        /// <param name="port">要连接服务端的端口</param>
        public void ConnectAsync(int port)
        {
            ConnectAsync("127.0.0.1", port);
        }
        /// <summary>
        /// 异步接收消息
        /// </summary>
        private void ReceiveAsync()
        {
            doReceive.Reset();
            StateObject obj = new StateObject();
            obj.Client = client;

            client.Client.BeginReceive(obj.btArrayData, 0, obj.btArrayData.Length, SocketFlags.None, ReceiveCallBack, obj);
            doReceive.WaitOne();
        }
        /// <summary>
        /// 异步发送消息
        /// </summary>
        /// <param name="msg"></param>
        public void SendAsync(string msg)
        {
            if (msg == null) return;
            byte[] listData = Encoding.UTF8.GetBytes(msg);
            client.Client.BeginSend(listData, 0, listData.Length, SocketFlags.None, SendCallBack, client);
        }
        public void SendAsync(byte[] msg)
        {           
            client.Client.BeginSend(msg, 0, msg.Length, SocketFlags.None, SendCallBack, client);
        }       
        /// <summary>
        /// 异步连接的回调函数
        /// </summary>
        /// <param name="ar"></param>
        private void ConnectCallBack(IAsyncResult ar)
        {
            try
            {
                TcpClient client = ar.AsyncState as TcpClient;
                client.EndConnect(ar);
                OnComplete(client, EnSocketAction.Connect);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        /// <summary>
        /// 异步接收消息的回调函数
        /// </summary>
        /// <param name="ar"></param>
        private void ReceiveCallBack(IAsyncResult ar)
        {
            StateObject obj = ar.AsyncState as StateObject;
            int count = -1;
            try
            {
                count = obj.Client.Client.EndReceive(ar);
                doReceive.Set();
            }
            catch (Exception)
            {
                //如果发生异常，说明客户端失去连接，触发关闭事件
                Close();
                OnComplete(obj.Client, EnSocketAction.Close);
            }
            if (count > 0)
            {
                if (Received != null)
                {
                    byte[] brec=new byte[count];
                    Array.Copy(obj.btArrayData,brec,count);
                    Received(obj.Client, brec);
                }
                
            }
        }
        private void SendCallBack(IAsyncResult ar)
        {
            TcpClient client = ar.AsyncState as TcpClient;
            try
            {
                client.Client.EndSend(ar);
                OnComplete(client, EnSocketAction.SendMsg);
            }
            catch (Exception)
            {
                //如果发生异常，说明客户端失去连接，触发关闭事件
                Close();
                OnComplete(client, EnSocketAction.Close);
            }
        }
        public virtual void OnComplete(TcpClient client, EnSocketAction enAction)
        {
            if (Completed != null)
                Completed(client, enAction);
            if (enAction == EnSocketAction.Connect)//建立连接后，开始接收数据
            {
                IsConnected=true;
                ThreadPool.QueueUserWorkItem(x =>
                {
                    while (!isClose)
                    {
                        try
                        {
                            Thread.Sleep(20);
                            ReceiveAsync();
                            Thread.Sleep(20);
                        }
                        catch (Exception)
                        {
                            Close();
                            OnComplete(client, EnSocketAction.Close);
                        }
                    }
                });
            }
        }

        public void Close()
        {
            try
            {
                if (client != null)
                {
                    client.Close();
                }
                isClose = true;
                IsConnected = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }
    }
   
}
