using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
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

        private SolidColorBrush _ReceiveColor=Brushes.Black;

        public SolidColorBrush ReceiveColor
        {
            get { return _ReceiveColor; }
            set
            {
                _ReceiveColor = value;
                RaisePropertyChanged("ReceiveColor");
            }
        }
        private SolidColorBrush _SendColor = Brushes.Black;

        public SolidColorBrush SendColor
        {
            get { return _SendColor; }
            set
            {
                _SendColor = value;
                RaisePropertyChanged("SendColor");
            }
        }
    }
}
