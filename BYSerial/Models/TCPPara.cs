using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using BYSerial.Base;

namespace BYSerial.Models
{
    
     public class TCPPara : NotificationObject
    {
        

        private Visibility _IsTcpTest = Visibility.Collapsed;
        /// <summary>
        /// 是否是串口测试
        /// </summary>
        public Visibility IsTcpTest
        {
            get { return _IsTcpTest; }
            set
            {
                _IsTcpTest = value;
                RaisePropertyChanged("IsTcpTest");
            }
        }
        private Visibility _IsTcpServer = Visibility.Collapsed;
        /// <summary>
        /// 是否是串口测试
        /// </summary>
        public Visibility IsTcpServer
        {
            get { return _IsTcpServer; }
            set
            {
                _IsTcpServer = value;
                if(value==Visibility.Collapsed)
                {
                    bIsTcpServer = false;
                }
                else
                {
                    bIsTcpServer = true;
                }
                RaisePropertyChanged("IsTcpServer");
            }
        }
        private bool _bIsTcpServer = false;
        /// <summary>
        /// 是否是串口测试
        /// </summary>
        public bool bIsTcpServer
        {
            get { return _bIsTcpServer; }
            set
            {
                _bIsTcpServer = value;
                RaisePropertyChanged("bIsTcpServer");
            }
        }
        private Visibility _IsTcpClient = Visibility.Visible;
        /// <summary>
        /// 是否是串口测试
        /// </summary>
        public Visibility IsTcpClient
        {
            get { return _IsTcpClient; }
            set
            {
                _IsTcpClient = value;
                RaisePropertyChanged("IsTcpClient");
                if (value == Visibility.Visible)
                {
                    IsTcpServer = Visibility.Collapsed;
                    bIsTcpClient = true;
                }
                else
                {
                    IsTcpServer=Visibility.Visible;
                    bIsTcpClient=false;
                }
            }
        }
        private bool _bIsTcpClient = true;
        /// <summary>
        /// 是否是串口测试
        /// </summary>
        public bool bIsTcpClient
        {
            get { return _bIsTcpClient; }
            set
            {
                _bIsTcpClient = value;
                RaisePropertyChanged("bIsTcpClient");                
            }
        }

        private int _NetModeIndex;
        /// <summary>
        /// TCP通讯模式
        /// </summary>
        public int NetModeIndex
        {
            get { return _NetModeIndex; }
            set { _NetModeIndex = value;
                RaisePropertyChanged("NetModeIndex");
            }
        }

        private ObservableCollection<string> _TcpClients = new ObservableCollection<string>();

        public ObservableCollection<string> TcpClients
        {
            get { return _TcpClients; }
            set { _TcpClients = value;
                RaisePropertyChanged("TcpClients");
            }
        }

        private int _Port=8888;

        public int Port
        {
            get { return _Port; }
            set { _Port = value;
                RaisePropertyChanged("Port");
            }
        }

        private string _IP="127.0.0.1";

        public string IP
        {
            get { return _IP; }
            set { _IP = value;
                RaisePropertyChanged("IP");
            }
        }

        private int _SvrClientsIndex;
        /// <summary>
        /// 服务器客户端列表选中项目序号
        /// </summary>
        public int SvrClientsIndex
        {
            get { return _SvrClientsIndex; }
            set { _SvrClientsIndex = value;
                RaisePropertyChanged("SvrClientsIndex");
            }
        }


    }
    

}
