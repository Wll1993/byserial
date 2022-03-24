using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using BYSerial.Base;
using BYSerial.Models;
using BYSerial.Views;
using Microsoft.Win32;

namespace BYSerial.ViewModels
{
    internal class OptionsViewModel : NotificationObject
    {
        public OptionsViewModel()
        {
            _DisplayPara = new DisplayPara(); 
            _ReceivePara = new ReceivePara();
            _LogPara = new LogPara();
            _SendPara = new SendPara();



            OnCancelCommand = new DelegateCommand();
            OnCancelCommand.ExecuteAction = new Action<object>(OnCancel);                
            OnOKCommand = new DelegateCommand();
            OnOKCommand.ExecuteAction = new Action<object>(OnOK);
            OnApplyCommand = new DelegateCommand();
            OnApplyCommand.ExecuteAction = new Action<object>(OnApply);
            OpenFileCommand = new DelegateCommand();
            OpenFileCommand.ExecuteAction = new Action<object>( OpenFile);
            SelectReceiveColorCmd = new DelegateCommand();
            SelectReceiveColorCmd.ExecuteAction = new Action<object>(SelectReceiveColor);
            SelectSendColorCmd = new DelegateCommand();
            SelectSendColorCmd.ExecuteAction = new Action<object>(SelectSendColor);
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

        private DisplayPara _DisplayPara;
        public DisplayPara DisplayPara
        {
            get => _DisplayPara;
            set
            {
                _DisplayPara = value;
                this.RaisePropertyChanged("DisplayPara");
            }
        }

        private int _OptionsTabSelectedIndex = 0;
        /// <summary>
        /// Options Tab当前选中项序号
        /// </summary>
        public int OptionsTabSelectedIndex
        {
            get => _OptionsTabSelectedIndex;
            set
            {
                _OptionsTabSelectedIndex = value;
                this.RaisePropertyChanged("OptionsTabSelectedIndex");
            }
        }

        public DelegateCommand OnCancelCommand { get; }
        public DelegateCommand OnOKCommand { get; }

        public DelegateCommand OnApplyCommand { get; }

        public DelegateCommand OpenFileCommand { get; }

        public DelegateCommand SelectReceiveColorCmd { get; }
        public DelegateCommand SelectSendColorCmd { get; }

        private void SelectReceiveColor(object para)
        {
            ColorPickerWin colorPicker = new ColorPickerWin();
            bool? bret= colorPicker.ShowDialog();
            if((bool)bret)
            {
                DisplayPara.ReceiveColor = colorPicker.SelectedBrush;
            }
        }
        private void SelectSendColor(object para)
        {
            ColorPickerWin colorPicker = new ColorPickerWin();
            bool? bret = colorPicker.ShowDialog();
            if ((bool)bret)
            {
                DisplayPara.SendColor = colorPicker.SelectedBrush;
            }
        }

        private void OnCancel(object para)
        {
            Window window=para as Window;
            if (window != null)
            {
                window.Close();
            }
        }
        private void OnOK(object para)
        {
            Window window = para as Window;
            GlobalPara.ReceivePara.MinimalInterval = ReceivePara.MinimalInterval;
            GlobalPara.ReceivePara.TimeFormat = ReceivePara.TimeFormat;

            GlobalPara.SendPara.FormatSend = SendPara.FormatSend;
            GlobalPara.SendPara.FormatNewLine = SendPara.FormatNewLine;
            GlobalPara.SendPara.FormatCarReturn = SendPara.FormatCarReturn;
            GlobalPara.SendPara.FormatCRNL = SendPara.FormatCRNL;
            GlobalPara.SendPara.FormatNLCR = SendPara.FormatNLCR;

            GlobalPara.LogPara.SaveLogMsg = LogPara.SaveLogMsg;
            GlobalPara.LogPara.FileName = LogPara.FileName;
            GlobalPara.LogPara.MaxFileSize = LogPara.MaxFileSize;
            GlobalPara.LogPara.EnableWriteBuf = LogPara.EnableWriteBuf;
            GlobalPara.LogPara.BufSize = LogPara.BufSize;

            GlobalPara.DisplayPara.FormatDisColor = DisplayPara.FormatDisColor;
            GlobalPara.DisplayPara.ReceiveColor = DisplayPara.ReceiveColor;
            GlobalPara.DisplayPara.SendColor = DisplayPara.SendColor;

            if (window != null)
            {
                window.Close();
            }
        }

        private void OnApply(Object para)
        {
            switch (OptionsTabSelectedIndex)
            {
                case 0:
                    GlobalPara.ReceivePara.MinimalInterval = ReceivePara.MinimalInterval;
                    GlobalPara.ReceivePara.TimeFormat = ReceivePara.TimeFormat;
                    break;
                case 1:
                    GlobalPara.SendPara.FormatSend = SendPara.FormatSend;
                    GlobalPara.SendPara.FormatNewLine = SendPara.FormatNewLine;
                    GlobalPara.SendPara.FormatCarReturn = SendPara.FormatCarReturn;
                    GlobalPara.SendPara.FormatCRNL = SendPara.FormatCRNL;
                    GlobalPara.SendPara.FormatNLCR = SendPara.FormatNLCR;
                    break;
                case 2:
                    GlobalPara.LogPara.SaveLogMsg = LogPara.SaveLogMsg;
                    GlobalPara.LogPara.FileName = LogPara.FileName;
                    GlobalPara.LogPara.MaxFileSize = LogPara.MaxFileSize;
                    GlobalPara.LogPara.EnableWriteBuf = LogPara.EnableWriteBuf;
                    GlobalPara.LogPara.BufSize = LogPara.BufSize;
                    break;
                case 3:
                    GlobalPara.DisplayPara.FormatDisColor = DisplayPara.FormatDisColor;
                    GlobalPara.DisplayPara.ReceiveTxtColor = DisplayPara.ReceiveTxtColor;
                    GlobalPara.DisplayPara.SendTxtColor = DisplayPara.SendTxtColor;
                    break;
            }
        }

        /// <summary>
        /// 选择文件
        /// </summary>
        /// <param name="wd"></param>
        /// <returns></returns>
        private void OpenFile(object para)
        {
            Window window = para as Window;
            OpenFileDialog ofd = new OpenFileDialog() { Title = "Select Exists Txt File", Multiselect= false };
            ofd.InitialDirectory = Environment.CurrentDirectory;            
            ofd.Filter="Txt|*.txt";
            if (ofd.ShowDialog() == true)
            { 
                LogPara.FileName = ofd.FileName;
            }           
        }
    }
}
