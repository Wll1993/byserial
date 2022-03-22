using BYSerial.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BYSerial.Models
{
    public class AsciiJson: NotificationObject
    {
        private string _Binary = "";
        public string Binary
        {
            get { return _Binary; }
            set 
            { _Binary = value; 
            RaisePropertyChanged(nameof(Binary));
            }
        }
        private string _Decimal = "";
        public string Decimal
        {
            get { return _Decimal; }
            set { _Decimal = value; RaisePropertyChanged(nameof(Decimal)); }
        }
        private string _Hex = "";
        public string Hex
        {
            get { return _Hex; }
            set { _Hex = value; RaisePropertyChanged(nameof(Hex)); }
        }

        private string _AbbrGlyph = "";
        public string AbbrGlyph
        {
            get { return _AbbrGlyph; }
            set { _AbbrGlyph = value; RaisePropertyChanged(nameof(AbbrGlyph)); }
        }
        private string _Discription = "";
        public string Discription
        {
            get { return _Discription; }
            set { _Discription = value; RaisePropertyChanged(nameof(Discription)); }
        }
    }    
}
