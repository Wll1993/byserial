﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BYSerial.Util
{
   public class FileTool
    {
        private static readonly object lockobj = new object();
        /// <summary>
        /// 保存错误记录
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool SaveFailLog(string str)
        {
            string logPath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Err" + DateTime.Now.ToString("yyyy-MM-dd") + ".log"; ;

            try
            {
                StreamWriter sw;
                if (!File.Exists(logPath))
                {
                    sw = File.CreateText(logPath);
                }
                else
                {
                    sw = File.AppendText(logPath);
                }
                sw.WriteLine(System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":" + str);
                sw.Flush();  //清空缓冲区
                sw.Close();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }
        /// <summary>
        /// 保存调试记录
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool SaveDebugLog(string str)
        {
            string logPath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Debug" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";

            try
            {
                StreamWriter sw;
                if (!File.Exists(logPath))
                {
                    sw = File.CreateText(logPath);
                }
                else
                {
                    sw = File.AppendText(logPath);
                }
                sw.WriteLine( str + "\r\n");
                sw.Flush();  //清空缓冲区
                sw.Close();

            }
            catch (Exception ex)
            {
                throw ex;                
            }
            return true;
        }

        public static bool SaveDebugLog(string filepath,string str)
        {   
            lock(lockobj)
            {
                try
                {
                    StreamWriter sw;
                    if (!File.Exists(filepath))
                    {
                        sw = File.CreateText(filepath);
                    }
                    else
                    {
                        sw = File.AppendText(filepath);
                    }
                    sw.WriteLine(str + "\r\n");
                    sw.Flush();  //清空缓冲区
                    sw.Close();

                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return true;
            }
        }

        /// <summary>
        /// 在Ubuntu中打开网址
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool OpenWebWithUrl(string url)
        {
            bool bret = false;
            try
            {
                var process = new Process();
                process.StartInfo.FileName ="explorer.exe";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.Arguments = url;
                process.Start();
                Thread.Sleep(1000);
            }
            catch
            {

            }
            return bret;
        }
    }
}
