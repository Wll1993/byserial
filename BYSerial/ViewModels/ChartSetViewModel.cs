using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BYSerial.Models;
using BYSerial.Base;

namespace BYSerial.ViewModels
{
    public class ChartSetViewModel: NotificationObject
    {
				
		public ChartSetViewModel()
		{
            ChartPara=GlobalPara.ChartParas;
            ApplyChartSetCmd = new DelegateCommand();
			ApplyChartSetCmd.ExecuteAction = new Action<object>(ApplyChartSet);
        }

		private ChartParas _ChartPara=new ChartParas();

		public ChartParas ChartPara
        {
			get { return _ChartPara; }
			set { _ChartPara = value;
				RaisePropertyChanged();
			}
		}

        public DelegateCommand ApplyChartSetCmd { get; }
		private void ApplyChartSet(object para)
		{
			GlobalPara.ChartParas = ChartPara;
		}

    }
}
