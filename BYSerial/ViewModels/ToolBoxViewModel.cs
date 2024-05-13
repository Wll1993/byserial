using BYSerial.Base;
using BYSerial.Models;
using BYSerial.Util;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

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
            OnCRC32Command= new DelegateCommand();
            OnCRC32Command.ExecuteAction=new Action<object>(OnCRC32);
            OnCRC32MPEG2Command=new DelegateCommand();
            OnCRC32MPEG2Command.ExecuteAction = new Action<object>(OnCRC32MPEG2);

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

            OnUIntegerTo16HexCommand = new DelegateCommand();
            OnUIntegerTo16HexCommand.ExecuteAction = new Action<object>(OnUIntegerTo16Hex);
            OnU16BitHexToIntegerCommand = new DelegateCommand();
            OnU16BitHexToIntegerCommand.ExecuteAction = new Action<object>(OnU16BitHexToInteger);
            OnU32BitHexToIntegerCommand = new DelegateCommand();
            OnU32BitHexToIntegerCommand.ExecuteAction = new Action<object>(OnU32BitHexToInteger);

            OnCopyCommand = new DelegateCommand();
            OnCopyCommand.ExecuteAction= new Action<object>(OnCopy);
            OnSelImgCommand= new DelegateCommand();
            OnSelImgCommand.ExecuteAction=new Action<object>(OnSelImg);
            OnImgCovCommand= new DelegateCommand();
            OnImgCovCommand.ExecuteAction= new Action<object>(OnImgCov);
            OnBase64CovCommand= new DelegateCommand();
            OnBase64CovCommand.ExecuteAction = new Action<object>(OnBase64Cov);
            AnlogRealCalCmd=new DelegateCommand();
            AnlogRealCalCmd.ExecuteAction= new Action<object>(AnlogRealCal);
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

        public DelegateCommand OnCRC32Command { get; }

        private void OnCRC32(object para)
        {
            StrCRC32 = CommonCheck.CalculateCRC32(SrcStrings);            
        }
        public DelegateCommand OnCRC32MPEG2Command { get; }

        private void OnCRC32MPEG2(object para)
        {
            StrCRC32MPEG2 = CommonCheck.CalculateCRC32MPEG2(SrcStrings);
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
        #region 有符号整数转换
        public DelegateCommand OnIntegerTo16HexCommand { get; }
        private void OnIntegerTo16Hex(object para)
        {
            try
            {
                
                Int16 datashor = 0;
                bool bret= Int16.TryParse(StrInteger, out datashor);
                if(bret)
                {
                    Str16BitHex = DataConvertUtility.ByteArrayToHexString(BitConverter.GetBytes(datashor).Reverse().ToArray());
                }
                else
                {
                    Str16BitHex = "非16位整数";
                }
                
                Int32 dataint =0;
                bret= Int32.TryParse(StrInteger, out dataint);
                if (bret)
                {
                    Str32BitHex = DataConvertUtility.ByteArrayToHexString(BitConverter.GetBytes(dataint).Reverse().ToArray());
                }
                else
                {
                    Str32BitHex = "非32位整数";
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
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

        #region 无符号整数转换
        public DelegateCommand OnUIntegerTo16HexCommand { get; }
        private void OnUIntegerTo16Hex(object para)
        {
            try
            {

                UInt16 datashor = 0;
                bool bret = UInt16.TryParse(StrUInteger, out datashor);
                if (bret)
                {
                    StrU16BitHex = DataConvertUtility.ByteArrayToHexString(BitConverter.GetBytes(datashor).Reverse().ToArray());
                }
                else
                {
                    StrU16BitHex = "非16位整数";
                }

                UInt32 dataint = 0;
                bret = UInt32.TryParse(StrUInteger, out dataint);
                if (bret)
                {
                    StrU32BitHex = DataConvertUtility.ByteArrayToHexString(BitConverter.GetBytes(dataint).Reverse().ToArray());
                }
                else
                {
                    StrU32BitHex = "非32位整数";
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        public DelegateCommand OnU16BitHexToIntegerCommand { get; }
        private void OnU16BitHexToInteger(object para)
        {
            byte[] bt = DataConvertUtility.HexStringToByte(StrU16BitHex).Reverse().ToArray();
            StrUInteger = BitConverter.ToUInt16(bt, 0).ToString();
        }
        public DelegateCommand OnU32BitHexToIntegerCommand { get; }
        private void OnU32BitHexToInteger(object para)
        {
            byte[] bt = DataConvertUtility.HexStringToByte(StrU32BitHex).Reverse().ToArray();
            StrUInteger = BitConverter.ToUInt32(bt, 0).ToString();
        }

        #endregion

        public DelegateCommand OnCopyCommand { get; }
        private void OnCopy(object para)
        {            
            System.Windows.Clipboard.SetText(StrFullStr);
        }

        public DelegateCommand OnSelImgCommand { get; }
        private void OnSelImg(object para)
        {
            OpenFileDialog ofd = new OpenFileDialog() { Title = "Select Exists Txt File", Multiselect = false };
            ofd.InitialDirectory = Environment.CurrentDirectory;
            ofd.Filter = "Png|*.png|Jpg|*.jpg|Jpeg|*.jpeg|Gif|*.gif|Bmp|*.bmp";
            if (ofd.ShowDialog() == true)
            {
                ImgName = ofd.FileName;                
            }
        }
        public DelegateCommand OnImgCovCommand { get; }
        private void OnImgCov(object para)
        {
            try
            {
                ImgSel = BitmapWPF.GetBitmapSource(ImgName);
                Base64Str =BitmapWPF.GetBase64String(ImgName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public DelegateCommand OnBase64CovCommand { get; }
        private void OnBase64Cov(object para)
        {
            try
            {
                ImgSel = null;
                Bitmap bitmap = BitmapWPF.GetBitmapFromBase64String(Base64Str);
                BitmapSource bmpsrc;
                BitmapWPF.BitmapToBitmapSource(bitmap,out bmpsrc);
                ImgSel = bmpsrc;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
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

        private string _StrCRC32="";

        public string StrCRC32
        {
            get { return _StrCRC32; }
            set { _StrCRC32 = value;
                RaisePropertyChanged("StrCRC32");
            }
        }

        private string _StrCRC32MPEG2= "";

        public string StrCRC32MPEG2
        {
            get { return _StrCRC32MPEG2; }
            set
            {
                _StrCRC32MPEG2 = value;
                RaisePropertyChanged("StrCRC32MPEG2");
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

        private string _StrUInteger = "";

        public string StrUInteger
        {
            get => _StrUInteger;
            set
            {
                _StrUInteger = value;
                this.RaisePropertyChanged("StrUInteger");
            }
        }

        private string _StrU16BitHex = "";

        public string StrU16BitHex
        {
            get => _StrU16BitHex;
            set
            {
                _StrU16BitHex = value;
                this.RaisePropertyChanged("StrU16BitHex");
            }
        }
        private string _StrU32BitHex = "";

        public string StrU32BitHex
        {
            get => _StrU32BitHex;
            set
            {
                _StrU32BitHex = value;
                this.RaisePropertyChanged("StrU32BitHex");
            }
        }
        #endregion

        #region Img
        private string _ImgName;

        public string ImgName
        {
            get { return _ImgName; }
            set 
            { 
                _ImgName = value;
                RaisePropertyChanged("ImgName");
            }
        }
        private BitmapSource _ImgSel;
        public BitmapSource ImgSel
        {
            get => _ImgSel;
            set
            {
                _ImgSel = value;
                RaisePropertyChanged("ImgSel");
            }
        }

        private string _Base64Str;

        public string Base64Str
        {
            get { return _Base64Str; }
            set 
            { 
                _Base64Str = value;
                RaisePropertyChanged("Base64Str");
            }
        }


        #endregion

        #region AnalogReal

        private AnalogReal _Analog=new AnalogReal();

        public AnalogReal Analog
        {
            get { return _Analog; }
            set { _Analog = value;
                RaisePropertyChanged("Analog");
            }
        }
        
        public DelegateCommand AnlogRealCalCmd { get; }
        private void AnlogRealCal(object para)
        {
            try
            {
                Analog.RealValue = (Analog.AnalogValue-Analog.AnalogMin)*(Analog.RealMax-Analog.RealMin)/(Analog.AnalogMax-Analog.AnalogMin) +Analog.RealMin;
            }
            catch 
            {
                MessageBox.Show("请检查输入数值是否有误，分母不可为零","错误提示");
            }
        }

        #endregion
    }
}
