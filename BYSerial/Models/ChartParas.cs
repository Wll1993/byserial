using BYSerial.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BYSerial.Models
{
    public class ChartParas : NotificationObject
    {
		private int _HexStartIndex=0;

		public int HexStartIndex
        {
			get { return _HexStartIndex; }
			set { _HexStartIndex = value; 
			RaisePropertyChanged("HexStartIndex");
			}
		}

		private int _HexDataTypeInex=0;

		public int HexDataTypeInex
        {
			get { return _HexDataTypeInex; }
			set { _HexDataTypeInex = value;
				RaisePropertyChanged("HexDataTypeInex");
				HexDataType = (DataType)_HexDataTypeInex;
            }
		}

		private int _HexDataByteOrderInex=0;

		public int HexDataByteOrderInex
        {
			get { return _HexDataByteOrderInex; }
			set { _HexDataByteOrderInex = value;
				RaisePropertyChanged("HexDataByteOrderInex");
				HexByteOrder = (ByteOrder)_HexDataByteOrderInex;
            }
		}

		private int _AsciiStartIndex=0;

		public int AsciiStartIndex
        {
			get { return _AsciiStartIndex; }
			set { _AsciiStartIndex = value;
				RaisePropertyChanged("AsciiStartIndex");
			}
		}

		private int _AsciiLen=2;

		public int AsciiLen
        {
			get { return _AsciiLen; }
			set { _AsciiLen = value;
				RaisePropertyChanged("AsciiLen");
			}
		}


		private int _XTimeSpan=300;
		/// <summary>
		/// X轴时间跨度，单位：秒
		/// </summary>
		public int XTimeSpan
        {
			get { return _XTimeSpan; }
			set {
                
				if(value <20)
				{
                    _XTimeSpan = 300;
				}
				else
				{
                    _XTimeSpan = value;
                }
				RaisePropertyChanged();
			}
		}
		private int _XStep=50;
		/// <summary>
		/// X轴步距，单位：秒
		/// </summary>
		public int XStep
        {
			get { return _XStep; }
			set { _XStep = value;
				RaisePropertyChanged();
			}
		}

        private int _YStep=10;
        /// <summary>
        /// X轴步距，单位：秒
        /// </summary>
        public int YStep
        {
            get { return _YStep; }
            set
            {
                _YStep = value;
                RaisePropertyChanged();
            }
        }
        private double _YMax=100;
        /// <summary>
        /// Y轴最大值
        /// </summary>
        public double YMax
        {
            get { return _YMax; }
            set
            {
                _YMax = value;
                RaisePropertyChanged();
            }
        }
        private double _YMin=0;
        /// <summary>
        /// Y轴最小值
        /// </summary>
        public double YMin
        {
            get { return _YMin; }
            set
            {
                _YMin = value;
                RaisePropertyChanged();
            }
        }

        public DataType HexDataType { get; set; } = DataType.UShort;
		public ByteOrder HexByteOrder { get; set; } = ByteOrder.B12;
        /// <summary>
        /// 截取的数值的数据类型
        /// </summary>
        public enum DataType
        {
            UShort=0,
            UInt=1,
            Short=2,
            Int =3,
            Float=4,
        }
		/// <summary>
		/// 字节顺序
		/// </summary>
		public enum ByteOrder
        {
			B12=0,
			B21=1,
            B1234=2,
            B3412=3,
			B4321=4,
        }
    }
   
}
