using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BYSerial.Base;


namespace BYSerial.Models
{
    public class SendPara: NotificationObject
    {
        private bool _IsText = true;

        public bool IsText
        {
            get => _IsText;
            set
            {
                _IsText = value;                
                this.RaisePropertyChanged("IsText");
            }
        }
        private bool _IsHex = false;

        public bool IsHex
        {
            get => _IsHex;
            set
            {
                _IsHex = value;
                AutoCRCEnable = value;
                if (!value) AutoCRC = false;
                this.RaisePropertyChanged("IsText");
            }
        }
        private bool _AutoCRCEnable = false;

        public bool AutoCRCEnable
        {
            get => _AutoCRCEnable;
            set
            {
                _AutoCRCEnable = value;
                this.RaisePropertyChanged("AutoCRCEnable");
            }
        }
        private bool _AutoCRC = false;

        public bool AutoCRC
        {
            get => _AutoCRC;
            set
            {
                _AutoCRC = value;
                this.RaisePropertyChanged("AutoCRC");
            }
        }
        private bool _IsLoop = false;

        public bool IsLoop
        {
            get => _IsLoop;
            set
            {
                _IsLoop = value;
                this.RaisePropertyChanged("IsLoop");
            }
        }

        private int _LoopInterval = 1000;

        public int LoopInterval
        {
            get => _LoopInterval;
            set
            {
                _LoopInterval = value;
                this.RaisePropertyChanged("LoopInterval");
            }
        }

        private bool _CommentSupport = false;

        public bool CommentSupport
        {
            get => _CommentSupport;
            set
            {
                _CommentSupport = value;
                this.RaisePropertyChanged("CommentSupport");
            }
        }
        private bool _FormatSend = false;

        public bool FormatSend
        {
            get => _FormatSend;
            set
            {
                _FormatSend = value;
                this.RaisePropertyChanged("FormatSend");
            }
        }

        private bool _FormatNewLine = true;

        public bool FormatNewLine
        {
            get => _FormatNewLine;
            set
            {
                _FormatNewLine = value;
                this.RaisePropertyChanged("FormatNewLine");
            }
        }
        private bool _FormatCarReturn = false;

        public bool FormatCarReturn
        {
            get => _FormatCarReturn;
            set
            {
                _FormatCarReturn = value;
                this.RaisePropertyChanged("FormatCarReturn");
            }
        }
        private bool _FormatNLCR = false;

        public bool FormatNLCR
        {
            get => _FormatNLCR;
            set
            {
                _FormatNLCR = value;
                this.RaisePropertyChanged("FormatNLCR");
            }
        }
        private bool _FormatCRNL = false;

        public bool FormatCRNL
        {
            get => _FormatCRNL;
            set
            {
                _FormatCRNL = value;
                this.RaisePropertyChanged("FormatCRNL");
            }
        }

    }
}
