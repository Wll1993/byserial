using BYSerial.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BYSerial.Models
{
    public class UDPPara : NotificationObject
    {
        public UDPPara() { }
		private bool _IsClient=true;

		public bool IsClient
        {
			get { return _IsClient; }
			set { _IsClient = value;
				RaisePropertyChanged();
				if(value)
				{
					ServerVisual = Visibility.Collapsed;
				}
				else { ServerVisual = Visibility.Visible; }
			}
		}

		private int _UDPModeIndex=0;

		public int UDPModeIndex
        {
			get { return _UDPModeIndex; }
			set { _UDPModeIndex = value;
			RaisePropertyChanged() ;
			}
		}

		private string _IP= "127.0.0.1";

		public string IP
        {
			get { return _IP; }
			set { _IP = value;
				RaisePropertyChanged();
			}
		}

		private int _Port=8888;

		public int Port
		{
			get { return _Port; }
			set { _Port = value;
				RaisePropertyChanged();
			}
		}

		private Visibility _ServerVisual=Visibility.Collapsed;

		public Visibility ServerVisual
        {
			get { return _ServerVisual; }
			set { _ServerVisual = value;
				RaisePropertyChanged();
			}
		}
        private ObservableCollection<string> _ClientsName = new ObservableCollection<string>();

        public ObservableCollection<string> ClientsName
        {
            get { return _ClientsName; }
            set
            {
                _ClientsName = value;
                RaisePropertyChanged();
            }
        }

		private int _SvrClientsIndex=0;

		public int SvrClientsIndex
        {
			get { return _SvrClientsIndex; }
			set { _SvrClientsIndex = value; 
				RaisePropertyChanged();
			}
		}

	}
}
