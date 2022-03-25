using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BYSerial.Base;

namespace BYSerial.Models
{
    internal class AnalogReal : NotificationObject
    {
        private float _AnalogMax;

        public float AnalogMax
        {
            get { return _AnalogMax; }
            set { _AnalogMax = value;
                RaisePropertyChanged("AnalogMax");
            }
        }
        private float _AnalogMin;

        public float AnalogMin
        {
            get { return _AnalogMin; }
            set
            {
                _AnalogMin = value;
                RaisePropertyChanged("AnalogMin");
            }
        }

        private float _AnalogValue;

        public float AnalogValue
        {
            get { return _AnalogValue; }
            set
            {
                _AnalogValue = value;
                RaisePropertyChanged("AnalogValue");
            }
        }

        private float _RealMax;

        public float RealMax
        {
            get { return _RealMax; }
            set { _RealMax = value;
                RaisePropertyChanged("RealMax");
            }
        }
        private float _RealMin;

        public float RealMin
        {
            get { return _RealMin; }
            set
            {
                _RealMin = value;
                RaisePropertyChanged("RealMin");
            }
        }

        private float _RealValue;

        public float RealValue
        {
            get { return _RealValue; }
            set
            {
                _RealValue = value;
                RaisePropertyChanged("RealValue");
            }
        }
    }
}
