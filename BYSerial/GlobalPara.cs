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
        public static FastCmdsCfg FastCfg = new FastCmdsCfg();

        private static string _cfgfile = "my.cfg";
        private static string _hisfile = "his.cfg";
        private static string _cmdfile = "fastcmds.cfg";
        /// <summary>
        /// ������ʾ����
        /// </summary>
        public static ChartParas ChartParas = new ChartParas();
        /// <summary>
        /// �Ƿ�������ʾͼ��
        /// </summary>
        public static bool IsShowChart { get; set; } = false;
        public static void GetLocSet()
        {            
            try
            {
                string path = Path.Combine(System.Windows.Forms.Application.StartupPath, _cfgfile);
                
                if (File.Exists(path))
                {
                    MyCfg=JSONHelper.DeserializeJsonToObject<mycfg>(File.ReadAllText(path));                    
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
                path = Path.Combine(System.Windows.Forms.Application.StartupPath, _hisfile);
                if (File.Exists(path))
                {
                    HisCfg=JSONHelper.DeserializeJsonToObject<hiscfg>(File.ReadAllText(path));
                }
                path = Path.Combine(System.Windows.Forms.Application.StartupPath, _cmdfile);
                if (File.Exists (path))
                {
                    FastCfg=JSONHelper.DeserializeJsonToObject<FastCmdsCfg>(File.ReadAllText(path));
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
                string path=Path.Combine(System.Windows.Forms.Application.StartupPath, "bycfg");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                MyCfg.LogPath = LogPara.FileName;
                MyCfg.FormatDisColor=DisplayPara.FormatDisColor;
                MyCfg.SendColor=DisplayPara.SendColor.ToString();
                MyCfg.RecColor=DisplayPara.ReceiveColor.ToString();
                MyCfg.CheckUpdate = IsCheckUpdate;
                string txt=JSONHelper.SerializeObject(MyCfg);

                File.WriteAllText(Path.Combine(path,_cfgfile), txt);
                txt=JSONHelper.SerializeObject(HisCfg);
                File.WriteAllText(Path.Combine(path, _hisfile), txt);
                txt=JSONHelper.SerializeObject(FastCfg);
                File.WriteAllText(Path.Combine(path, _cmdfile), txt);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
