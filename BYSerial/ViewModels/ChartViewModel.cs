using BYSerial.Base;
using BYSerial.Models;
using BYSerial.Util;
using BYSerial.Views;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BYSerial.ViewModels
{
    public class ChartViewModel : NotificationObject
    {
        private ChartViewModel()
        {

        }
        public ChartViewModel(ChartParas para)
        {
            try
            {
                //lets set how to display the X Labels
                DateTimeFormatter = value => new DateTime((long)value).ToString("HH:mm");
                XTimeSpan = para.XTimeSpan;
                XAxisStep = TimeSpan.FromSeconds(para.XStep).Ticks;
                YAxisStep = para.YStep;
                YAxisMax = para.YMax;
                YAxisMin = para.YMin;
            }
            catch (Exception ex)
            {
                FileTool.SaveFailLog(ex.Message);
            }           
            SetAxisLimits(DateTime.Now);
        }
        private ChartValues<MeasureData> _tempValues = new ChartValues<MeasureData>();
        private ChartValues<MeasureData> _Chart1Values = new ChartValues<MeasureData>();
        public ChartValues<MeasureData> Chart1Values
        {
            get => _Chart1Values;
            set
            {
                _Chart1Values = value;
                RaisePropertyChanged();  //"Chart1Values"
            }
        }

        public Func<double, string> DateTimeFormatter { get; set; }

        private double _axisMax1;
        private double _axisMin1;
        public double AxisMax1
        {
            get { return _axisMax1; }
            set
            {
                _axisMax1 = value;
                RaisePropertyChanged("AxisMax1");
            }
        }
        public double AxisMin1
        {
            get { return _axisMin1; }
            set
            {
                _axisMin1 = value;
                RaisePropertyChanged("AxisMin1");
            }
        }
        //AxisStep forces the distance between each separator in the X axis
        private double _XAxisStep = TimeSpan.FromMinutes(1).Ticks;
        public double XAxisStep { 
        
            get { return _XAxisStep; }
            set
            {
                _XAxisStep = value;
                RaisePropertyChanged();
            }
        }
        private double _YAxisStep = 5;
        public double YAxisStep
        {

            get { return _YAxisStep; }
            set
            {
                _YAxisStep = value;
                RaisePropertyChanged();
            }
        }

        private double _YAxisMin=0;

        public double YAxisMin
        {
            get { return _YAxisMin; }
            set { _YAxisMin = value; 
            RaisePropertyChanged() ;
            }
        }
        private double _YAxisMax=100;

        public double YAxisMax
        {
            get { return _YAxisMax; }
            set { _YAxisMax = value;
                RaisePropertyChanged();
            }
        }



        public void SetAxisStep(int step)
        {
            XAxisStep = TimeSpan.FromMinutes(step).Ticks;
        }
        //this is not always necessary, but it can prevent wrong labeling
        private double _AxisUnit = TimeSpan.FromMinutes(1).Ticks;
        public double AxisUnit
        {
            get { return _AxisUnit; }
            set
            {
                _AxisUnit = value;
                RaisePropertyChanged("AxisUnit");
            }
        }

        public void SetAxisUnit(int step)
        {
            AxisUnit = TimeSpan.FromMinutes(step).Ticks;
        }

        /// <summary>
        /// 时间跨度，单位秒
        /// </summary>
        public int XTimeSpan { get; set; } = 300;


        private void SetAxisLimits(DateTime now)
        {
            int start = 20;
            int stop = XTimeSpan; // XTimeSpan * 20;
            AxisMax1 = now.Ticks + TimeSpan.FromSeconds(start).Ticks; // lets force the axis to be start seconds ahead
            AxisMin1 = now.Ticks - TimeSpan.FromSeconds(stop).Ticks; // and stop seconds behind
        }
        private void AddNewData(MeasureData data)
        {
            if (Chart1Values.Count > 100) Chart1Values.RemoveAt(0);
            Chart1Values.Add(data);           
            SetAxisLimits(DateTime.Now);
        }

        private double _CurValue;

        public double CurValue
        {
            get { return _CurValue; }
            set { _CurValue = value; 
            RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="value"></param>
        public void AddData(double value)
        {
            try
            {
                CurValue=value;
                MeasureData data = new MeasureData() { DateTime = DateTime.Now, Value = value };
                //AddNewData(data);
                App.Current.Dispatcher.BeginInvoke(() =>
                {
                    AddNewData(data);
                });
            }
            catch
            {

            }
            
        }

    }
}
