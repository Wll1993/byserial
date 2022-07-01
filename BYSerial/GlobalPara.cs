using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using BYSerial.Models;
using BYSerial.Util;

namespace BYSerial
{
    public class GlobalPara
    {
        public static string SysPlattform = "";

        public static ReceivePara ReceivePara = new ReceivePara();
        public static SendPara SendPara = new SendPara();
        public static LogPara LogPara = new LogPara();
        public static DisplayPara DisplayPara = new DisplayPara();
        public static bool IsCheckUpdate = true;
       
        public static readonly SolidColorBrush RedBrush = new SolidColorBrush(Colors.Red);
        public static readonly SolidColorBrush GreenBrush = new SolidColorBrush(Colors.Green);
        public static readonly SolidColorBrush TransparentBrush = new SolidColorBrush(Colors.Transparent);
       
        public static mycfg MyCfg = new mycfg();
        public static hiscfg HisCfg = new hiscfg();

        private static string _cfgfile = "my.cfg";
        private static string _hisfile = "his.cfg";
        

        public static void GetLocSet()
        {            
            try
            {
                if(File.Exists(_cfgfile))
                {
                    MyCfg=JSONHelper.DeserializeJsonToObject<mycfg>(File.ReadAllText(_cfgfile));                    
                    if(MyCfg!=null)
                    {
                        LogPara.FileName = MyCfg.LogPath;

                        var converter = new BrushConverter();
                        DisplayPara.FormatDisColor=MyCfg.FormatDisColor;
                        DisplayPara.SendColor = (SolidColorBrush)converter.ConvertFromString(MyCfg.SendColor);
                        DisplayPara.ReceiveColor= (SolidColorBrush)converter.ConvertFromString(MyCfg.RecColor);
                        //Console.WriteLine(Brushes.Black.ToString());
                        IsCheckUpdate = MyCfg.CheckUpdate;
                    }
                    
                }
                if(File.Exists(_hisfile))
                {
                    HisCfg=JSONHelper.DeserializeJsonToObject<hiscfg>(File.ReadAllText(_hisfile));
                }
                            
            }
            catch(Exception ex)
            {
                throw ex;
            }            
        }
       
        public static void SaveCurCfg()
        {
             try
            {
                MyCfg.LogPath = LogPara.FileName;
                MyCfg.FormatDisColor=DisplayPara.FormatDisColor;
                MyCfg.SendColor=DisplayPara.SendColor.ToString();
                MyCfg.RecColor=DisplayPara.ReceiveColor.ToString();
                MyCfg.CheckUpdate = IsCheckUpdate;
                string txt=JSONHelper.SerializeObject(MyCfg);
                File.WriteAllText(_cfgfile, txt);
                txt=JSONHelper.SerializeObject(HisCfg);
                File.WriteAllText(_hisfile, txt);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
