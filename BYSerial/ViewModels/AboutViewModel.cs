using BYSerial.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BYSerial.ViewModels
{
    internal class AboutViewModel : NotificationObject
    {
      
        private string _Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();// "Version:1.1.1";
        public string Version
        {
            get => _Version;
            set
            {
                _Version = value;
                this.RaisePropertyChanged("Version");
            }
        }

        private string _Information = "Email:xuyuanbaoxyb@163.com\r\nWeChat/Phone:18694923164\r\nQQ:416315797\r\n";
        public string Information
        {
            get => _Information;
            set
            {
                _Information = value;
                RaisePropertyChanged(nameof(Information));
            }
        }
       
    }
}
