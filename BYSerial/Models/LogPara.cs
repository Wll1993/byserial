﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using BYSerial.Base;

namespace BYSerial.Models
{
    public class LogPara : NotificationObject
    {
        private bool _SaveLogMsg = false;

        public bool SaveLogMsg
        {
            get => _SaveLogMsg;
            set
            {
                _SaveLogMsg = value;
                this.RaisePropertyChanged("SaveLogMsg");
            }
        }

        private bool _EnableWriteBuf = false;

        public bool EnableWriteBuf
        {
            get => _EnableWriteBuf;
            set
            {
                _EnableWriteBuf = value;
                this.RaisePropertyChanged("EnableWriteBuf");
            }
        }
        private string _FileName =GlobalPara.LogFolder+ "\\"+DateTime.Now.ToString("yyyyMMddHHmmssfff")+".txt";

        public string FileName
        {
            get => _FileName;
            set
            {
                _FileName = value;
                this.RaisePropertyChanged("FileName");
            }
        }

        private int _MaxFileSize = 1024;  //单位字节，1024字节1MB

        public int MaxFileSize
        {
            get => _MaxFileSize;
            set
            {
                _MaxFileSize = value;
                this.RaisePropertyChanged("MaxFileSize");
            }
        }

        private int _BufSize = 256;  //KB

        public int BufSize
        {
            get => _BufSize;
            set
            {
                _BufSize = value;
                this.RaisePropertyChanged("BufSize");
            }
        }

    }
}
