using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using BYSerial.Base;

namespace BYSerial.Models
{
    public class DisplayPara : NotificationObject
    {

        private bool _FormatDisColor = false;

        public bool FormatDisColor
        {
            get => _FormatDisColor;
            set
            {
                _FormatDisColor = value;
                this.RaisePropertyChanged("FormatDisColor");
            }
        }

        private string _ReceiveTxtColor = "#000000";

        public string ReceiveTxtColor
        {
            get => _ReceiveTxtColor;
            set
            {
                _ReceiveTxtColor = value;
                this.RaisePropertyChanged("ReceiveTxtColor");
            }
        }
        private string _SendTxtColor = "#000000";

        public string SendTxtColor
        {
            get => _SendTxtColor;
            set
            {
                _SendTxtColor = value;
                this.RaisePropertyChanged("SendTxtColor");
            }
        }

    }
}
