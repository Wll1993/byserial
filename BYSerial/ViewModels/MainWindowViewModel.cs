using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BYSerial.Base;

using System.Collections.ObjectModel;
using System.IO;
using System.IO.Ports;
using System.Threading;
using BYSerial.Models;
using System.Windows;
using System.Windows.Media;
using BYSerial.Util;
using BYSerial.Views;
using System.Windows.Documents;
using BYSerial.TCPHelper;
using System.Net;
using System.Net.Sockets;

namespace BYSerial.ViewModels
{
    public class MainWindowViewModel : NotificationObject
    {
        public SerialPort _serialPort;
        

        public MainWindowViewModel()
        {
            GlobalPara.GetLocSet();
            if(GlobalPara.HisCfg.his!=null)
            {
                SendTxtHistory = new ObservableCollection<string>(GlobalPara.HisCfg.his);
            }
            else
            {
                GlobalPara.HisCfg.his = new List<string>();
            }
             SerialPortList = new ObservableCollection<string>(SerialPort.GetPortNames().ToList());
            if (SerialPortList.Count > 0)
            {
                _ComPortState = SerialPortList[PortNameIndex] + " ClOSED";
            }
            else
            {
                ComPortState = "Not Detected SerialPort";
                ComPortStateColor = GlobalPara.RedBrush;
            }
           
            DataBitsIndex = 3;
            ReceivePara = GlobalPara.ReceivePara;
            SendPara = GlobalPara.SendPara;
            LogPara = GlobalPara.LogPara;
            DisplayPara = GlobalPara.DisplayPara;
            ChangeLang(GlobalPara.MyCfg.Language);

            #region 命令绑定
            OnSendCommand = new DelegateCommand();
            OnSendCommand.ExecuteAction = new Action<object>(SendCommand);
            OnLogCommand = new DelegateCommand();
            OnLogCommand.ExecuteAction = new Action<object>(OnLogClick);
            OnStartCommand = new DelegateCommand();
            OnStartCommand.ExecuteAction = new Action<object>(OnStartClick);
            OnPauseCommand = new DelegateCommand();
            OnPauseCommand.ExecuteAction = new Action<object>(OnPauseClick);
            OnStopCommand = new DelegateCommand();
            OnStopCommand.ExecuteAction = new Action<object>(OnStopClick);
            OnClearCommand = new DelegateCommand();
            OnClearCommand.ExecuteAction = new Action<object>(OnClearClick);
            OnHideLeftCommand = new DelegateCommand();
            OnHideLeftCommand.ExecuteAction = new Action<object>(OnHideLeft);
            #endregion

            #region 菜单命令
            ChangeToChCmd = new DelegateCommand();
            ChangeToChCmd.ExecuteAction = new Action<object>(ChangeToCh);
            ChangeToEnCmd = new DelegateCommand();
            ChangeToEnCmd.ExecuteAction = new Action<object>(ChangeToEn);
            ShowToolBoxCmd = new DelegateCommand();
            ShowToolBoxCmd.ExecuteAction = new Action<object>(ShowToolBox);
            ShowAsciiCmd = new DelegateCommand();
            ShowAsciiCmd.ExecuteAction = new Action<object>(ShowAscii);
            ShowOptionsCmd = new DelegateCommand();
            ShowOptionsCmd.ExecuteAction = new Action<object>(ShowOptions);
            ShowAboutCmd = new DelegateCommand();
            ShowAboutCmd.ExecuteAction = new Action<object>(ShowAbout);
            ShowDonateCmd = new DelegateCommand();
            ShowDonateCmd.ExecuteAction = new Action<object>(ShowDonate);
            #endregion
            try
            {
                BackDetectSerialPortChange();
            }
            catch { }

        }
        #region 菜单命令
        public DelegateCommand ShowToolBoxCmd { get; private set; }
        private void ShowToolBox(object para)
        {
            ToolBoxWindow window = new ToolBoxWindow();
            window.Show();
        }
        public DelegateCommand ShowAsciiCmd { get; private set; }
        private void ShowAscii(object para)
        {
            AsciiCodeWindow window = new AsciiCodeWindow();
            window.Show();
        }
        public DelegateCommand ShowOptionsCmd { get; private set; }
        private void ShowOptions(object para)
        {
            OptionsWindow window = new OptionsWindow();
            window.Show();
        }
        public DelegateCommand ShowAboutCmd { get; private set; }
        private void ShowAbout(object para)
        {
            AboutWindow window = new AboutWindow();
            window.Show();
        }
        public DelegateCommand ShowDonateCmd { get; private set; }
        private void ShowDonate(object para)
        {
            DonateWindow window = new DonateWindow();
            window.Show();
        }

        public DelegateCommand ChangeToChCmd { get; private set; }
        private void ChangeToCh(object para)
        {
            // TODO: 切换系统资源文件
            ChangeLang("zh-CN");
        }
        public DelegateCommand ChangeToEnCmd { get; private set; }
        private void ChangeToEn(object para)
        {
            ChangeLang("en-US");
        }
        private void ChangeLang(string lang)
        {
            if(lang == "en-US")
            {
                ResourceDictionary dict = new ResourceDictionary();
                dict.Source = new Uri(@"Assets\Language\en-US.xaml", UriKind.Relative);
                Application.Current.Resources.MergedDictionaries[0] = dict;
                if (GlobalPara.MyCfg != null)
                {
                    GlobalPara.MyCfg.Language = "en-US";
                }
            }
            else
            {
                ResourceDictionary dict = new ResourceDictionary();
                dict.Source = new Uri(@"Assets\Language\zh-CN.xaml", UriKind.Relative);
                Application.Current.Resources.MergedDictionaries[0] = dict;
                if (GlobalPara.MyCfg != null)
                {
                    GlobalPara.MyCfg.Language = "zh-CN";
                }
            }
        }

        #endregion

        private void BackDetectSerialPortChange()
        {
            Task task = Task.Factory.StartNew(new Action(() =>
            {
                while (true)
                {
                    if (!IsStartCan) continue;
                    Thread.Sleep(500);
                    SerialPortList = new ObservableCollection<string>(SerialPort.GetPortNames().ToList());
                    //添加TCP项
                    SerialPortList.Add("TCP");
                }
            }));
        }

        public async Task SendCmdLoop()
        {
            while (SendPara.IsLoop)
            {
                SendCommand(null);
                Thread.Sleep(SendPara.LoopInterval);
            }
        }

        private FlowDocument _ReciveFlowDoc=null;

        public DelegateCommand OnSendCommand { get; private set; }

        private void SendCommand(object para)
        {
            try
            {
                // string cmd = SendTxt.Replace("", " ");
                if (SendTxt.Trim() == "") return;
                string txtsend = SendTxt.Trim().Replace(" ", "").ToUpper();
                if (SendPara.FormatSend)
                {
                    if (SendPara.FormatCRNL)
                    {
                        txtsend += "\r\n";
                    }
                    else if (SendPara.FormatNLCR)
                    {
                        txtsend += "\n\r";
                    }
                    else if (SendPara.FormatNewLine)
                    {
                        txtsend += "\r";
                    }
                    else if (SendPara.FormatCarReturn)
                    {
                        txtsend += "\n";
                    }
                }
                int byteNum = 0;

                if (SendPara.IsHex)
                {
                    if(txtsend.Length%2!=0)
                    {
                        MessageBox.Show("输入字符长度为奇数，命令不可发送；请检查命令是否有错！\r\n一个字节至少2个字符，不足请补零","错误提示");
                        return;
                    }
                    byte[] btSend = new byte[1];
                    if (SendPara.AutoCRC)
                    {
                        string strcrc = CommonCheck.CheckCRC16Modbus(txtsend);
                        btSend = Util.DataConvertUtility.HexStringToByte(strcrc);
                    }
                    else
                    {
                        btSend = Util.DataConvertUtility.HexStringToByte(txtsend);
                    }
                    if (IsSerialTest == Visibility.Visible)
                    {
                        _serialPort.Write(btSend, 0, btSend.Length);
                    }
                    else
                    {
                        if (TcpPara.bIsTcpClient)
                        {
                            _TcpClient.SendAsync(btSend);
                        }
                        else if(TcpPara.bIsTcpServer)
                        {
                            _TcpServer.SendAsync(TcpPara.TcpClients[TcpPara.SvrClientsIndex],btSend);
                        }
                    }
                    byteNum = btSend.Length;

                }
                else if (SendPara.IsText)
                {
                    if (IsSerialTest == Visibility.Visible)
                    {
                        _serialPort.Write(txtsend);
                    }
                    else
                    {
                        if (TcpPara.bIsTcpClient)
                        {
                            _TcpClient.SendAsync(txtsend);
                        }
                        else if(TcpPara.bIsTcpServer)
                        {
                            _TcpServer.SendAsync(TcpPara.TcpClients[TcpPara.SvrClientsIndex], txtsend);
                        }
                    }
                    byteNum = Encoding.ASCII.GetBytes(txtsend).Length;
                }

                if (ReceivePara.DisplaySend)
                {
                    if (ReceivePara.AutoFeed)
                    {
                        if (SendPara.IsHex)
                        {
                            txtsend = Util.DataConvertUtility.InsertFormat(txtsend, 2, " ") + "\r\n";
                        }
                        else
                        {
                            txtsend += "\r\n";
                        }
                    }
                    if (ReceivePara.DisplayTime)
                    {
                        txtsend =DateTime.Now.ToString(ReceivePara.TimeFormat) + txtsend;
                    }
                    txtsend = "[SEND]" + txtsend;                   
                    App.Current.Dispatcher.BeginInvoke(new Action(() => {
                        Paragraph pg = new Paragraph();
                        pg.Margin = new Thickness(1);
                        pg.Padding = new Thickness(0);
                        pg.Inlines.Add(new Run(txtsend));
                        if (DisplayPara.FormatDisColor)
                        {
                            pg.Foreground = DisplayPara.SendColor;
                        }
                        _ReciveFlowDoc.Blocks.Add(pg);
                    }));

                    if (LogPara.SaveLogMsg)
                    {
                        SaveLogAsync(txtsend);
                    }
                }
                _SendedBytesNum += byteNum;
                SendBytesStr = "Tx: " + _SendedBytesNum + " Bytes";
                AddSendHistory(SendTxt);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }


        #region Toolbar command

        public DelegateCommand OnLogCommand { get; }

        private void OnLogClick(object parameter)
        {
            LogPara.SaveLogMsg = !LogPara.SaveLogMsg;
            if (LogPara.SaveLogMsg)
            {
                LogBtnBackColor = GlobalPara.RedBrush;
            }
            else
            {
                LogBtnBackColor = GlobalPara.TransparentBrush;
            }
        }

        private SolidColorBrush _LogBtnBackColor = new SolidColorBrush(Colors.Transparent);

        public SolidColorBrush LogBtnBackColor
        {
            get => _LogBtnBackColor;
            set
            {
                _LogBtnBackColor = value;
                this.RaisePropertyChanged("LogBtnBackColor");
            }
        }

        private bool _IsStartCan = true;

        public bool IsStartCan
        {
            get => _IsStartCan;
            set
            {
                _IsStartCan = value;
                this.RaisePropertyChanged("IsStartCan");
            }
        }

        private bool _IsStopCan = false;
        public bool IsStopCan
        {
            get => _IsStopCan;
            set
            {
                _IsStopCan = value;
                this.RaisePropertyChanged("IsStopCan");
            }
        }

        public DelegateCommand OnStartCommand { get; }

        private void OnStartClick(object parameter)
        {
            try
            {
                //将显示收发信息的流文档传输过来。
                _ReciveFlowDoc = parameter as FlowDocument;

                if (_serialPort != null)
                {
                    if (_serialPort.IsOpen) _serialPort.Close();
                }


                bool bdtr = false; bool brts = false;

                if (FlowType == 1)
                {
                    brts = true;
                }
                else if (FlowType == 2)
                {
                    bdtr = true;
                }
                if(PortNameIndex==SerialPortList.Count-1)
                {
                    if(TcpPara.bIsTcpClient)
                    {
                        if((TcpPara.IP=="")||(TcpPara.Port==0))
                        {
                            MessageBox.Show("请填写正确的IP或Port", "错误提示");
                            return;
                        }
                        _TcpClient = new ClientAsync();
                        _TcpClient.Completed += _TcpClient_Completed;
                        _TcpClient.Received += _TcpClient_Received;
                        _TcpClient.ConnectAsync(TcpPara.IP, TcpPara.Port);
                        
                    }
                    else if(TcpPara.bIsTcpServer)
                    {
                        if ((TcpPara.IP == "") || (TcpPara.Port == 0))
                        {
                            MessageBox.Show("请填写正确的IP或Port", "错误提示");
                            return;
                        }
                        _TcpServer = new ServerAsync();
                        _TcpServer.Completed += _TcpServer_Completed;
                        _TcpServer.Received += _TcpServer_Received;
                        _TcpServer.StartAsync(TcpPara.IP,TcpPara.Port);
                        IsStartCan = false;
                    }
                }
                else
                {
                    _serialPort = new SerialPort();
                    _serialPort.PortName = SerialPortList[PortNameIndex];
                    _serialPort.BaudRate = BaudRateList[BaudRateIndex];
                    _serialPort.Parity = (Parity)this.Parity;
                    _serialPort.DataBits = DataBitsList[DataBitsIndex];
                    _serialPort.StopBits = (StopBits)this.StopBits;
                    _serialPort.DtrEnable = bdtr;
                    _serialPort.RtsEnable = brts;
                    _serialPort.DataReceived += _serialPort_DataReceived;
                    _serialPort.Open();
                    ComPortState = SerialPortList[PortNameIndex] + " Opened";

                    ComPortStateColor = GlobalPara.GreenBrush;
                    IsStartCan = false;
                    IsStopCan = true;
                    IsPauseCan = true;
                    SendCmdIsEnable = true;
                    PauseBtnBackColor = GlobalPara.TransparentBrush;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                ComPortState = SerialPortList[PortNameIndex] + " Can't open!";
                ComPortStateColor = GlobalPara.RedBrush;
            }
        }

        

        public DelegateCommand OnHideLeftCommand { private set; get; }
        private void OnHideLeft(Object parameter)
        {
            if (LeftSetingVisual == Visibility.Visible)
            {
                LeftSetingVisual = Visibility.Collapsed;
            }
            else
            {
                LeftSetingVisual = Visibility.Visible;
            }
        }

        private Visibility _LeftSetingVisual = Visibility.Visible;
        public Visibility LeftSetingVisual
        {
            get => _LeftSetingVisual;
            set
            {
                _LeftSetingVisual = value;
                RaisePropertyChanged("LeftSetingVisual");
            }
        }


        private SolidColorBrush _PauseBtnBackColor = new SolidColorBrush(Colors.Transparent);
        public SolidColorBrush PauseBtnBackColor
        {
            get => _PauseBtnBackColor;
            set
            {
                _PauseBtnBackColor = value;
                this.RaisePropertyChanged("PauseBtnBackColor");
            }
        }


        public DelegateCommand OnPauseCommand { get; }

        private bool _IsPauseCan = false;

        public bool IsPauseCan
        {
            get => _IsPauseCan;
            set
            {
                _IsPauseCan = value;
                this.RaisePropertyChanged("IsPauseCan");
            }
        }

        private void OnPauseClick(object parameter)
        {

            if (PauseBtnBackColor == GlobalPara.GreenBrush)
            {
                PauseBtnBackColor = GlobalPara.RedBrush;
            }
            else
            {
                PauseBtnBackColor = GlobalPara.GreenBrush;
            }
        }


        public DelegateCommand OnStopCommand { get; }

        private void OnStopClick(object parameter)
        {
            IsStopCan = false;
            IsStartCan = true;
            IsPauseCan = false;
            SendCmdIsEnable = false;
            PauseBtnBackColor = GlobalPara.TransparentBrush;
            if(IsSerialTest==Visibility.Visible)
            {
                if (_serialPort != null)
                {
                    if (_serialPort.IsOpen) _serialPort.Close();
                    ComPortState = SerialPortList[PortNameIndex] + " Closed";
                    ComPortStateColor = new SolidColorBrush(Colors.Green);
                }
            }
            else if(TcpPara.bIsTcpClient)
            {
                _TcpClient.Close();
            }
            else if(TcpPara.bIsTcpServer)
            {
                _TcpServer.Close();
                _TcpPara.TcpClients.Clear();
            }
        }
        public DelegateCommand OnClearCommand { get; }

        private void OnClearClick(object parameter)
        {
            App.Current.Dispatcher.BeginInvoke(new Action(() => {
                _ReciveFlowDoc.Blocks.Clear();
            }));
            _SendedBytesNum = 0;
            _ReceivedBytesNum = 0;
            SendBytesStr = "Tx: 0 Bytes";
            ReceiveBytesStr = "Rx: 0 Bytes";
        }

        #endregion

        #region Port Operate
        private void _serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (!_IsPauseCan) return;
                Thread.Sleep(50);
                int nums = _serialPort.BytesToRead;
                byte[] bytes = new byte[nums];
                _serialPort.Read(bytes, 0, nums);
                string receivestr = "";
                if (ReceivePara.IsHex)
                {
                    receivestr = DataConvertUtility.BytesToHexStrWithSpace(bytes);
                }
                else
                {
                    receivestr = Encoding.UTF8.GetString(bytes);
                }

                if (ReceivePara.AutoFeed)
                {
                    receivestr += "\r\n";
                }
                if (ReceivePara.DisplayTime)
                {
                    receivestr = DateTime.Now.ToString(ReceivePara.TimeFormat) + receivestr;
                }
                receivestr = "[REC]" + receivestr;
                ////20220324切换为RichTextBox,此处暂丢弃
                // ReceiveTxt +=receivestr;                
                App.Current.Dispatcher.BeginInvoke(new Action(() => {
                    try
                    {
                        Paragraph pg = new Paragraph();
                        pg.Margin = new Thickness(1);
                        pg.Padding= new Thickness(0);
                        pg.Inlines.Add(new Run(receivestr));
                        if (DisplayPara.FormatDisColor)
                        {
                            pg.Foreground = DisplayPara.ReceiveColor;
                        }
                        _ReciveFlowDoc.Blocks.Add(pg);
                    }
                    catch
                    {
                        ComPortState = SerialPortList[PortNameIndex] + " Recive Process Error!";
                        ComPortStateColor = GlobalPara.RedBrush;
                        return;
                    }
                }));
                _ReceivedBytesNum += bytes.Length;
                ReceiveBytesStr = "Rx: " + _ReceivedBytesNum + " Bytes";
                if (LogPara.SaveLogMsg)
                {
                    SaveLogAsync(receivestr);
                }
            }
            catch
            {
                ComPortState = SerialPortList[PortNameIndex] + " Recive Process Error!";
                ComPortStateColor = GlobalPara.RedBrush;
            }
        }
        #endregion



        #region PortParaGetSet
        private ObservableCollection<string> _SerialPortList = new ObservableCollection<string>();

        public ObservableCollection<string> SerialPortList
        {
            get => _SerialPortList;
            set
            {
                _SerialPortList = value;
                this.RaisePropertyChanged("SerialPortList");
            }
        }

        private int _PortNameIndex = 0;

        public int PortNameIndex
        {
            get => _PortNameIndex;
            set
            {
                _PortNameIndex = value;
                this.RaisePropertyChanged("PortNameIndex");
            }
        }



        private ObservableCollection<int> _BaudRateList = new ObservableCollection<int>() { 9600, 19200, 38400, 57600, 115200, 1200, 2400, 4800 };

        public ObservableCollection<int> BaudRateList
        {
            get => _BaudRateList;
            set
            {
                _BaudRateList = value;
                this.RaisePropertyChanged("BaudRateList");
            }
        }

        private int _BaudRateIndex = 0;

        public int BaudRateIndex
        {
            get => _BaudRateIndex;
            set
            {
                _BaudRateIndex = value;
                this.RaisePropertyChanged("BaudRateIndex");
            }
        }

        private ObservableCollection<int> _DataBitsList = new ObservableCollection<int>() { 5, 6, 7, 8 };

        public ObservableCollection<int> DataBitsList
        {
            get => _DataBitsList;
            set
            {
                _DataBitsList = value;
                this.RaisePropertyChanged("DataBitsList");
            }
        }

        private int _DataBitsIndex = 0;
        public int DataBitsIndex
        {
            get => _DataBitsIndex;
            set
            {
                _DataBitsIndex = value;
                this.RaisePropertyChanged("DataBitsIndex");
            }
        }

        private int _Parity = 0;
        public int Parity
        {
            get => _Parity;
            set
            {
                _Parity = value;
                this.RaisePropertyChanged("Parity");
            }
        }

        private int _StopBits = 1;
        public int StopBits
        {
            get => _StopBits;
            set
            {
                _StopBits = value;
                this.RaisePropertyChanged("StopBits");
            }
        }

        private int _FlowType = 0;
        public int FlowType
        {
            get => _FlowType;
            set
            {
                _FlowType = value;
                this.RaisePropertyChanged("FlowType");
            }
        }
        #endregion

        #region PortSendReceiveState
        private string _ComPortState = "";

        public string ComPortState
        {
            get => _ComPortState;
            set
            {
                _ComPortState = value;
                this.RaisePropertyChanged("ComPortState");
            }
        }
        private SolidColorBrush _ComPortStateColor = new SolidColorBrush(Colors.Red);
        public SolidColorBrush ComPortStateColor
        {
            get => _ComPortStateColor;
            set
            {
                _ComPortStateColor = value;
                this.RaisePropertyChanged("ComPortStateColor");
            }
        }
        /// <summary>
        /// �Ѿ����͵��ֽ���
        /// </summary>
        private long _SendedBytesNum = 0;

        private string _SendBytesStr = "Tx:0 Bytes";
        public string SendBytesStr
        {
            get => _SendBytesStr;
            set
            {
                _SendBytesStr = value;
                this.RaisePropertyChanged("SendBytesStr");
            }
        }

        private SolidColorBrush _SendBytesColor = new SolidColorBrush(Colors.Green);
        public SolidColorBrush SendBytesColor
        {
            get => _SendBytesColor;
            set
            {
                _SendBytesColor = value;
                this.RaisePropertyChanged("SendBytesColor");
            }
        }
        /// <summary>
        /// �Ѿ����յ��ֽ���
        /// </summary>
        private long _ReceivedBytesNum = 0;
        private string _ReceiveBytesStr = "Rx:0 Bytes";
        public string ReceiveBytesStr
        {
            get => _ReceiveBytesStr;
            set
            {
                _ReceiveBytesStr = value;
                this.RaisePropertyChanged("ReceiveBytesStr");
            }
        }
        private SolidColorBrush _ReceiveBytesColor = new SolidColorBrush(Colors.BlueViolet);
        public SolidColorBrush ReceiveBytesColor
        {
            get => _ReceiveBytesColor;
            set
            {
                _ReceiveBytesColor = value;
                this.RaisePropertyChanged("ReceiveBytesColor");
            }
        }
        #endregion

        #region 绑定属性
        private Visibility _IsSerialTest = Visibility.Visible;
        /// <summary>
        /// 是否是串口测试
        /// </summary>
        public Visibility IsSerialTest
        {
            get { return _IsSerialTest; }
            set
            {
                _IsSerialTest = value;
                if (_IsSerialTest != Visibility.Visible)
                {
                    TcpPara.IsTcpTest=Visibility.Visible;
                }
                else
                {
                    TcpPara.IsTcpTest=Visibility.Collapsed;
                }
                RaisePropertyChanged("IsSerialTest");
            }
        }
       
        private DisplayPara _DisplayPara;
        public DisplayPara DisplayPara
        {
            get => _DisplayPara;
            set
            {
                _DisplayPara = value;
                RaisePropertyChanged("DisplayPara");
            }
        }

        private LogPara _LogPara;

        public LogPara LogPara
        {
            get => _LogPara;
            set
            {
                _LogPara = value;
                this.RaisePropertyChanged("LogPara");
            }
        }


        private ReceivePara _ReceivePara;
        public ReceivePara ReceivePara
        {
            get => _ReceivePara;
            set
            {
                _ReceivePara = value;
                this.RaisePropertyChanged("ReceivePara");
            }

        }

        private SendPara _SendPara;
        public SendPara SendPara
        {
            get => _SendPara;
            set
            {
                _SendPara = value;
                this.RaisePropertyChanged("SendPara");
            }

        }

        private string _SendTxt = "";

        public string SendTxt
        {
            get => _SendTxt;
            set
            {
                _SendTxt = value;
                this.RaisePropertyChanged("SendTxt");
            }
        }
        private ObservableCollection<string> _SendTxtHistory = new ObservableCollection<string>();

        public ObservableCollection<string> SendTxtHistory
        {
            get => _SendTxtHistory;
            set
            {
                _SendTxtHistory = value;
                this.RaisePropertyChanged("SendTxtHistory");
            }
        }

        private void AddSendHistory(string str)
        {
            if (SendTxtHistory.Count > 20)
            {
                SendTxtHistory.RemoveAt(0);
            }
            if (SendTxtHistory.Contains(str)) return;
            SendTxtHistory.Add(str);
            GlobalPara.HisCfg.his.Add(str);
        }

        private int _SendTxtHisSelIndex = 0;
        public int SendTxtHisSelIndex
        {
            get => _SendTxtHisSelIndex;
            set
            {
                _SendTxtHisSelIndex = value;
                this.RaisePropertyChanged("SendTxtHisSelIndex");
            }
        }

        private bool _SendCmdIsEnable = false;

        public bool SendCmdIsEnable
        {
            get => _SendCmdIsEnable;
            set
            {
                _SendCmdIsEnable = value;
                this.RaisePropertyChanged("SendCmdIsEnable");
            }
        }
        private string _SerialPortSettings = "";
        public string SerialPortSettings
        {
            get => _SerialPortSettings;
            set
            {
                _SerialPortSettings = value;
                this.RaisePropertyChanged("SerialPortSettings");
            }
        }

        #endregion

        #region Log

        private List<string> _LogList = new List<string>();

        private async Task<bool> SaveLogAsync(string strlog)
        {
            bool bret = false;
            if (LogPara.EnableWriteBuf)
            {
                long len = 0;
                long listlen = _LogList.Count;
                if (listlen > 0)
                {
                    for (int i = 0; i < listlen; i++)
                    {
                        len += System.Text.Encoding.Default.GetBytes(_LogList[i]).Length;
                    }
                }
                if (len >= LogPara.BufSize)
                {
                    for (int i = 0; i < listlen; i++)
                    {
                        await SaveLogBaseAsync(_LogList[i]);
                    }
                    _LogList.Clear();
                }
                else
                {
                    _LogList.Add(strlog);
                }
            }
            else
            {
                await SaveLogBaseAsync(strlog);
            }

            return bret;
        }

        private async Task<bool> SaveLogBaseAsync(string strlog)
        {
            bool bret = false;
            try
            {
                if (!LogPara.SaveLogMsg) return true;

                System.IO.FileInfo fileInfo = null;
                try
                {
                    fileInfo = new System.IO.FileInfo(LogPara.FileName);
                }
                catch (Exception ex)
                {
                    Util.FileTool.SaveFailLog(ex.Message);
                    return false;
                }
                
                if (fileInfo != null && fileInfo.Exists)
                {
                    System.Diagnostics.FileVersionInfo info = System.Diagnostics.FileVersionInfo.GetVersionInfo(LogPara.FileName);
                    long filsizeMB = fileInfo.Length;
                    if (filsizeMB >= LogPara.MaxFileSize)
                    {
                        string filepath = Path.GetDirectoryName(LogPara.FileName);
                        if (filepath == null || filepath == "")
                        {
                            filepath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                        }
                        LogPara.FileName = Path.Combine(filepath, "Log_" + DateTime.Now.ToString("yyyyMMdd") + ".txt");
                    }
                }

                bret = Util.FileTool.SaveDebugLog(LogPara.FileName, strlog);
            }
            catch (Exception ex)
            {
                Util.FileTool.SaveFailLog(ex.Message);
                ComPortState = "Save Log Err";
                ComPortStateColor = GlobalPara.RedBrush;
            }
            return bret;
        }

        #endregion

        #region TCP 
        private TCPPara _TcpPara = new TCPPara();
        
        public TCPPara TcpPara
        {
            get { return _TcpPara; }
            set
            {
                _TcpPara = value;
                RaisePropertyChanged("TcpPara");
            }
        }
        #region TCP Client
        private ClientAsync _TcpClient;
        private void _TcpClient_Completed(System.Net.Sockets.TcpClient client, EnSocketAction arg2)
        {
            try
            {
                IPEndPoint iep = client.Client.RemoteEndPoint as IPEndPoint;
                string key = string.Format("{0}:{1}", iep.Address.ToString(), iep.Port);
                switch (arg2)
                {
                    case EnSocketAction.Connect:
                        ComPortState = client.Client.RemoteEndPoint.ToString() + " Connected";
                        ComPortStateColor = GlobalPara.GreenBrush;
                        IsStartCan = false;
                        IsStopCan = true;
                        IsPauseCan = true;
                        SendCmdIsEnable = true;
                        PauseBtnBackColor = GlobalPara.TransparentBrush;
                        break;
                    case EnSocketAction.SendMsg:
                        Console.WriteLine("{0}：向{1}发送了一条消息", DateTime.Now, key);
                    
                        break;
                    case EnSocketAction.Close:
                        Console.WriteLine("服务端连接关闭");
                        ComPortState = client.Client.RemoteEndPoint.ToString() + " Closed";
                        ComPortStateColor = GlobalPara.RedBrush;
                        IsStartCan = true;
                        IsStopCan = false;
                        IsPauseCan = false;
                        SendCmdIsEnable = false;
                        PauseBtnBackColor = GlobalPara.TransparentBrush;
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private void _TcpClient_Received(TcpClient client, byte[] msg)
        {
            try
            {
                if (!_IsPauseCan) return;
                Thread.Sleep(10);              
                string receivestr = "";
                if (ReceivePara.IsHex)
                {
                    receivestr = DataConvertUtility.BytesToHexStrWithSpace(msg);
                }
                else
                {
                    receivestr = Encoding.UTF8.GetString(msg);
                }

                if (ReceivePara.AutoFeed)
                {
                    receivestr += "\r\n";
                }
                if (ReceivePara.DisplayTime)
                {
                    receivestr = DateTime.Now.ToString(ReceivePara.TimeFormat) + receivestr;
                }
                receivestr = "[REC]" + receivestr;                         
                App.Current.Dispatcher.BeginInvoke(new Action(() => {
                    try
                    {
                        Paragraph pg = new Paragraph();
                        pg.Margin = new Thickness(1);
                        pg.Padding = new Thickness(0);
                        pg.Inlines.Add(new Run(receivestr));
                        if (DisplayPara.FormatDisColor)
                        {
                            pg.Foreground = DisplayPara.ReceiveColor;
                        }
                        _ReciveFlowDoc.Blocks.Add(pg);
                    }
                    catch
                    {

                        ComPortState = client.Client.RemoteEndPoint.ToString() + " Recive Error!";
                        ComPortStateColor = GlobalPara.RedBrush;
                        return;
                    }
                }));
                _ReceivedBytesNum += msg.Length;
                ReceiveBytesStr = "Rx: " + _ReceivedBytesNum + " Bytes";
                if (LogPara.SaveLogMsg)
                {
                    SaveLogAsync(receivestr);
                }
            }
            catch
            {
                ComPortState = client.Client.RemoteEndPoint.ToString() + " Recive Process Error!";
                ComPortStateColor = GlobalPara.RedBrush;
            }
        }
        #endregion

        #region TCP Server
        private ServerAsync _TcpServer;

        private ObservableCollection<TcpClient> _TcpClientLst;
        /// <summary>
        /// TCP服务器接收到的客户端集合
        /// </summary>
        public ObservableCollection<TcpClient> TcpClientLst
        {
            get { return _TcpClientLst; }
            set { _TcpClientLst = value;
                RaisePropertyChanged("TcpClientLst");
            }
        }
        /// <summary>
        /// TCPServer 的客户端返回的信息
        /// </summary>
        /// <param name="client"></param>
        /// <param name="msg"></param>
        private void _TcpServer_Received(TcpClient client, byte[] msg)
        {
            _TcpClient_Received(client, msg);
        }

        private void _TcpServer_Completed(TcpClient client, EnSocketAction arg2)
        {
            try
            {
                IPEndPoint iep = client.Client.RemoteEndPoint as IPEndPoint;
                string key = string.Format("{0}:{1}", iep.Address.ToString(), iep.Port);
                switch (arg2)
                {
                    case EnSocketAction.Connect:
                        App.Current.Dispatcher.BeginInvoke(new Action(() => {
                            if (TcpPara.TcpClients.Contains(key))
                            {
                                TcpPara.TcpClients.Remove(key);
                            }
                            TcpPara.TcpClients.Add(key);
                            ComPortState = key + " Connected";
                            if (!IsStartCan)
                            {
                                ComPortStateColor = GlobalPara.GreenBrush;
                                IsStartCan = false;
                                IsStopCan = true;
                                IsPauseCan = true;
                                SendCmdIsEnable = true;
                                PauseBtnBackColor = GlobalPara.TransparentBrush;
                            }
                        }));
                        break;
                    case EnSocketAction.SendMsg:
                       // Console.WriteLine("{0}：向{1}发送了一条消息", DateTime.Now, key);

                        break;
                    case EnSocketAction.Close:
                        App.Current.Dispatcher.BeginInvoke(new Action(() => {
                            if (TcpPara.TcpClients.Contains(key))
                            {
                                TcpPara.TcpClients.Remove(key);
                                ComPortState = key + " Closed";
                            }                        
                            if (TcpPara.TcpClients.Count == 0)
                            {
                                ComPortState = "No Clients";
                                ComPortStateColor = GlobalPara.RedBrush; 
                                SendCmdIsEnable = false;
                                PauseBtnBackColor = GlobalPara.TransparentBrush;
                            }
                            }));
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        #endregion

        #endregion

    }
}
