using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BYSerial.Views
{
    /// <summary>
    /// UpdateWindow.xaml 的交互逻辑
    /// </summary>
    public partial class UpdateWindow : Window
    {
        public UpdateWindow()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 更新提示
        /// </summary>
        public string UpdateTip { get; set; }

        public string Url { get; set; }


        public void SetUpdateMsg(string tip,string url)
        {
            try
            {
                txtUpdate.Text = tip;
                Run linkText = new Run(url);
                Hyperlink link = new Hyperlink(linkText);
                link.NavigateUri = new Uri(url);
                link.RequestNavigate += new RequestNavigateEventHandler(delegate (object sender, RequestNavigateEventArgs e) {
                    //Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
                    //https://gitee.com/LvYiWuHen/byserial/attach_files/1097700/download/BYSerial_V1.4.4.zip
                    //https://gitee.com/LvYiWuHen/byserial/attach_files/1081068/download/BYSerial_V1.4.3.zip
                    string folder = System.IO.Path.Combine(Environment.CurrentDirectory, "update");
                    if(!Directory.Exists(folder)) Directory.CreateDirectory(folder);  
                    string[] tmp=url.Split('/');
                    string fileName = tmp[tmp.Length-1];//客户端保存的文件名
                    string filePath = System.IO.Path.Combine(folder,fileName);//完整路径 
                    downTip.Text = "下载中...";
                    //以字符流的形式下载文件
                    WebClient wc = new WebClient();
                    byte[] data = wc.DownloadData(url);
                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    // byte[] bytes = new byte[(int)fs.Length];
                    fs.Write(data, 0, data.Length);
                    fs.Flush();
                    fs.Close();
                    downTip.Text = "下载完成";
                    e.Handled = true;
                });
                linklbl.Content = link;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }

       
    }
}
