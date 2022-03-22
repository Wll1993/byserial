using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using BYSerial.Models;


namespace BYSerial
{
    public class GlobalPara
    {
        public static string SysPlattform = "";

        public static ReceivePara ReceivePara = new ReceivePara();
        public static SendPara SendPara = new SendPara();
        public static LogPara LogPara = new LogPara();
        public static DisplayPara DisplayPara = new DisplayPara();
       
        public static readonly SolidColorBrush RedBrush = new SolidColorBrush(Colors.Red);
        public static readonly SolidColorBrush GreenBrush = new SolidColorBrush(Colors.Green);
        public static readonly SolidColorBrush TransparentBrush = new SolidColorBrush(Colors.Transparent);
        public static LocSet LocSet=new LocSet();

        private static string _LangFileName = "Language.cfg"; 

        public static void GetLocSet()
        {            
            try
            {
                if(File.Exists(_LangFileName))
                {
                    string lang = File.ReadAllText(_LangFileName).Trim().Substring(0,5);
                    LocSet.Language = lang;
                }
                else
                {
                    File.WriteAllText(_LangFileName, "en-US");
                }              
            }
            catch(Exception ex)
            {
                throw ex;
            }            
        }
       
        public static void SaveCurLanguage(string lang)
        {
             try
            {
                File.WriteAllText(_LangFileName, lang);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
