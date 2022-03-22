using BYSerial.Base;
using BYSerial.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BYSerial.ViewModels
{
    internal class ToolBoxViewModel : NotificationObject
    {
        public ToolBoxViewModel()
        {
            OnLRCCommand = new DelegateCommand();
            OnLRCCommand.ExecuteAction=new Action<object>(OnLRC);
            OnXORCommand = new DelegateCommand();
            OnXORCommand.ExecuteAction = new Action<object>(OnXOR);
            OnCheckSumCommand = new DelegateCommand();
            OnCheckSumCommand.ExecuteAction = new Action<object>(OnCheckSum);
            OnFCSCommand = new DelegateCommand();
            OnFCSCommand.ExecuteAction = new Action<object>(OnFCS);
            OnCRC16LoHiCommand = new DelegateCommand();
            OnCRC16LoHiCommand.ExecuteAction = new Action<object>(OnCRC16LoHi);
            OnDecimalTo16HexCommand = new DelegateCommand();
            OnDecimalTo16HexCommand.ExecuteAction = new Action<object>(OnDecimalTo16Hex);
            OnSingleHexToDecimalCommand = new DelegateCommand();
            OnSingleHexToDecimalCommand.ExecuteAction = new Action<object>(OnSingleHexToDecimal);
            OnDoubleHexToDecimalCommand = new DelegateCommand();
            OnDoubleHexToDecimalCommand.ExecuteAction = new Action<object>(OnDoubleHexToDecimal);
            OnIntegerTo16HexCommand = new DelegateCommand();
            OnIntegerTo16HexCommand.ExecuteAction = new Action<object>(OnIntegerTo16Hex);
            On16BitHexToIntegerCommand = new DelegateCommand();
            On16BitHexToIntegerCommand.ExecuteAction = new Action<object>(On16BitHexToInteger);
            On32BitHexToIntegerCommand = new DelegateCommand();
            On32BitHexToIntegerCommand.ExecuteAction = new Action<object>(On32BitHexToInteger);
        }

        #region command
        public DelegateCommand  OnLRCCommand { get; }

        private void OnLRC(object para)
        {
            StrLRC = CommonCheck.CheckLRC(SrcStrings);
            StrFullStr = SrcStrings + StrLRC;
        }

        public DelegateCommand OnXORCommand { get; }
        private void OnXOR(object para)
        {
            StrXOR = CommonCheck.CheckXor(SrcStrings);
            StrFullStr = SrcStrings + StrXOR;
        }
        public DelegateCommand OnCheckSumCommand { get; }
        private void OnCheckSum(object para)
        {
            StrCheckSum = CommonCheck.CheckSum(SrcStrings);
            StrFullStr = SrcStrings + StrCheckSum;
        }
        public DelegateCommand OnFCSCommand { get; }
        private void OnFCS(object para)
        {
            StrFCS = CommonCheck.CheckFCS(SrcStrings);
            StrFullStr = SrcStrings + StrFCS;
        }
        public DelegateCommand OnCRC16LoHiCommand { get; }
        private void OnCRC16LoHi(object para)
        {
            StrCRC16LoHi = CommonCheck.CheckCRC16Modbus(SrcStrings);
            StrFullStr = SrcStrings + StrCRC16LoHi;
        }
        public DelegateCommand OnDecimalTo16HexCommand { get; }
        private void OnDecimalTo16Hex(object para)
        {
            float sing = Convert.ToSingle(StrDecimal);
            StrSingleHex = DataConvertUtility.ByteArrayToHexString(BitConverter.GetBytes(sing).Reverse().ToArray());
            double dob = Convert.ToDouble(StrDecimal);
            StrDoubleHex = DataConvertUtility.ByteArrayToHexString(BitConverter.GetBytes(dob).Reverse().ToArray());
        }
        public DelegateCommand OnSingleHexToDecimalCommand { get; }
        private void OnSingleHexToDecimal(object para)
        {
            byte[] data = DataConvertUtility.HexStringToByte(StrSingleHex).Reverse().ToArray();
            StrDecimal = BitConverter.ToSingle(data, 0).ToString();
        }
        public DelegateCommand OnDoubleHexToDecimalCommand { get; }
        private void OnDoubleHexToDecimal(object para)
        {
            byte[] data = DataConvertUtility.HexStringToByte(StrDoubleHex).Reverse().ToArray();
            StrDecimal = BitConverter.ToDouble(data, 0).ToString();
        }
        public DelegateCommand OnIntegerTo16HexCommand { get; }
        private void OnIntegerTo16Hex(object para)
        {
            Int16 datashor = Convert.ToInt16(StrInteger);
            Str16BitHex = DataConvertUtility.ByteArrayToHexString(BitConverter.GetBytes(datashor).Reverse().ToArray());
            Int32 dataint = Convert.ToInt32(StrInteger);
            Str32BitHex = DataConvertUtility.ByteArrayToHexString(BitConverter.GetBytes(dataint).Reverse().ToArray());
        }
        public DelegateCommand On16BitHexToIntegerCommand { get; }
        private void On16BitHexToInteger(object para)
        {
            byte[] bt = DataConvertUtility.HexStringToByte(Str16BitHex).Reverse().ToArray();
            StrInteger = BitConverter.ToInt16(bt,0).ToString();
        }
        public DelegateCommand On32BitHexToIntegerCommand { get; }
        private void On32BitHexToInteger(object para)
        {
            byte[] bt = DataConvertUtility.HexStringToByte(Str32BitHex).Reverse().ToArray();
            StrInteger = BitConverter.ToInt32(bt,0).ToString();
        }




        #endregion

        #region Check
        private string _SrcStrings = "";

        public string SrcStrings
        {
            get => _SrcStrings;
            set
            {
                _SrcStrings = value;
                this.RaisePropertyChanged("SrcStrings");
            }
        }

        private string _StrLRC = "";

        public string StrLRC
        {
            get => _StrLRC;
            set
            {
                _StrLRC = value;
                this.RaisePropertyChanged("StrLRC");
            }
        }
        private string _StrXOR = "";

        public string StrXOR
        {
            get => _StrXOR;
            set
            {
                _StrXOR = value;
                this.RaisePropertyChanged("StrXOR");
            }
        }
        private string _StrCheckSum = "";

        public string StrCheckSum
        {
            get => _StrCheckSum;
            set
            {
                _StrCheckSum = value;
                this.RaisePropertyChanged("StrCheckSum");
            }
        }

        private string _StrFCS = "";

        public string StrFCS
        {
            get => _StrFCS;
            set
            {
                _StrFCS = value;
                this.RaisePropertyChanged("StrFCS");
            }
        }
        private string _StrCRC16LoHi = "";

        public string StrCRC16LoHi
        {
            get => _StrCRC16LoHi;
            set
            {
                _StrCRC16LoHi = value;
                this.RaisePropertyChanged("StrCRC16LoHi");
            }
        }
        private string _StrFullStr = "";
        public string StrFullStr
        {
            get => _StrFullStr;
            set
            {
                _StrFullStr = value;
                this.RaisePropertyChanged("StrFullStr");
            }
        }
        #endregion

        #region Convert
        private string _StrDecimal = "";

        public string StrDecimal
        {
            get => _StrDecimal;
            set
            {
                _StrDecimal = value;
                this.RaisePropertyChanged("StrDecimal");
            }
        }

        private string _StrSingleHex = "";

        public string StrSingleHex
        {
            get => _StrSingleHex;
            set
            {
                _StrSingleHex = value;
                this.RaisePropertyChanged("StrSingleHex");
            }
        }
        private string _StrDoubleHex = "";

        public string StrDoubleHex
        {
            get => _StrDoubleHex;
            set
            {
                _StrDoubleHex = value;
                this.RaisePropertyChanged("StrDoubleHex");
            }
        }

        private string _StrInteger = "";

        public string StrInteger
        {
            get => _StrInteger;
            set
            {
                _StrInteger = value;
                this.RaisePropertyChanged("StrInteger");
            }
        }

        private string _Str16BitHex = "";

        public string Str16BitHex
        {
            get => _Str16BitHex;
            set
            {
                _Str16BitHex = value;
                this.RaisePropertyChanged("Str16BitHex");
            }
        }
        private string _Str32BitHex = "";

        public string Str32BitHex
        {
            get => _Str32BitHex;
            set
            {
                _Str32BitHex = value;
                this.RaisePropertyChanged("Str32BitHex");
            }
        }
        #endregion
    }
}
