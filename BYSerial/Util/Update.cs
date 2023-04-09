using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Reflection;
using BYSerial.Views;
using System.Windows;

using System.Runtime.InteropServices;

namespace BYSerial.Util
{
    public class Update
    {
        /// <summary>
        /// 判断网络状况的方法，返回true为连接，false为未连接
        /// </summary>
        /// <param name="conState"></param>
        /// <param name="reder"></param>
        /// <returns></returns>
        [DllImport("wininet.dll")]
        public extern static bool InternetGetConnectedState(out int conState, int reder);
        public static bool CheckUpdate()
        {
            try
            {
                string version = Assembly.GetExecutingAssembly().GetName().Version.ToString().Replace(".","");
                int Ver = Convert.ToInt32(version);
                WebClient wc = new WebClient();
                string remoteVer = wc.DownloadString("https://gitee.com/LvYiWuHen/byserial/raw/master/Version.txt");
                string[] vers = remoteVer.Split(',');
                int reVer=Convert.ToInt32(vers[0]);
                if(reVer > Ver)
                {
                    string tip = Encoding.UTF8.GetString(wc.DownloadData("https://gitee.com/LvYiWuHen/byserial/raw/master/UpdateTip.txt"));
                    
                    UpdateWindow uw=new UpdateWindow();
                    uw.SetUpdateMsg(tip, vers[1]);
                    uw.ShowDialog();
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("检查更新出错","更新提示");
            }
            return false;
        }
    }
}
