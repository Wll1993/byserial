using BYSerial.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BYSerial.ViewModels;
namespace BYSerial.Models
{
    public class FastCmdModel : NotificationObject
    {
		public FastCmdModel()
		{
			SendFastCmdCmd = new DelegateCommand();
			SendFastCmdCmd.ExecuteAction = new Action<object>(SendFastCmd);
        }
        [Newtonsoft.Json.JsonIgnore()]
        public MainWindowViewModel.SendFastCmd SendCmd;

		private int _CmdNum=0;
		/// <summary>
		/// 命令编号
		/// </summary>
		public int CmdNum
        {
			get { return _CmdNum; }
			set { _CmdNum = value;
				RaisePropertyChanged("CmdNum");
                CmdNumStr=SetCmdNumStr(value);
            }
		}
		private string SetCmdNumStr(int cmdnum)
		{
            string str = cmdnum.ToString();
            if (str.Length == 1)
            {
                return "0" + str;
            }
            else
            {
                return str;
            }
        }
        private string _CmdNumStr = "";
        /// <summary>
        /// 命令编号字符串
        /// </summary>
        [Newtonsoft.Json.JsonIgnore()]
        public string CmdNumStr
        {
            get { 	
				_CmdNumStr= SetCmdNumStr(CmdNum);
				return _CmdNumStr; 
			}
            set
            {
                _CmdNumStr = value;
                RaisePropertyChanged("CmdNumStr");
            }
        }

		private int _RowID=0;
		/// <summary>
		/// 命令所在行ID
		/// </summary>
        [Newtonsoft.Json.JsonIgnore()]
        public int RowID
        {
			get { return _RowID; }
			set { _RowID = value;
				RaisePropertyChanged("RowID");
			}
		}


		private int _DelayTime=50;

		public int DelayTime
        {
			get { return _DelayTime; }
			set { _DelayTime = value;
				RaisePropertyChanged("DelayTime");
			}
		}

		private string _CmdString="";

		public string CmdString
        {
			get { return _CmdString; }
			set { _CmdString = value;
				RaisePropertyChanged("CmdString");
			}
		}

		private string _Remark="";
		/// <summary>
		/// 备注
		/// </summary>
		public string Remark
        {
			get { return _Remark; }
			set { _Remark = value;
				RaisePropertyChanged("Remark");
			}
		}

		private bool _IsSelected=false;
		/// <summary>
		/// 是否被选中
		/// </summary>
		public bool IsSelected
        {
			get { return _IsSelected; }
			set { _IsSelected = value;
			RaisePropertyChanged("IsSelected");
			}
		}

        [Newtonsoft.Json.JsonIgnore()]
        public DelegateCommand SendFastCmdCmd { get; private set; }

		public void SendFastCmd(object para)
		{
			if(SendCmd!=null)
			{
				try
				{
                    SendCmd(CmdString);
                }
				catch
				{

				}
				
			}
		}


    }
}
