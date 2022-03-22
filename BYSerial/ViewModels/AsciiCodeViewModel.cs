using BYSerial.Base;
using BYSerial.Models;
using BYSerial.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Resources;

namespace BYSerial.ViewModels
{
    internal class AsciiCodeViewModel:NotificationObject
    {
        public AsciiCodeViewModel()
        {
            //string txtjson = "";
            try
            {
                Uri uri = new Uri("pack://application:,,,/BYSerial;component/Assets/ascii.json", UriKind.Absolute);
                                
                StreamResourceInfo srinfo = Application.GetResourceStream(uri);
                Stream sr = srinfo.Stream;
                byte[] bytes = new byte[sr.Length];
                sr.Read(bytes, 0, bytes.Length);
                string json= Encoding.UTF8.GetString(bytes);
                AsciiList = JSONHelper.DeserializeJsonToObject<ObservableCollection<AsciiJson>>(json);
               
                //string filename = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Assets/ascii.json";
                //txtjson = File.ReadAllText(filename);
                //AsciiList = JSONHelper.DeserializeJsonToObject<ObservableCollection<AsciiJson>>(txtjson);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);    
            }


        }
        private ObservableCollection<AsciiJson> _AsciiList = new ObservableCollection<AsciiJson>();
        public ObservableCollection<AsciiJson> AsciiList
        {
            get => _AsciiList;
            set
            {
                _AsciiList = value;
                this.RaisePropertyChanged("AsciiList");
            }

        }
    }
}
