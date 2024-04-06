using BYSerial.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BYSerial.Views
{
    /// <summary>
    /// SelfDevWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SelfDevWindow : Window
    {
        public SelfDevWindow()
        {
            InitializeComponent();
        }

       

        private void lbl4DIDO_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string html = "https://item.taobao.com/item.htm?ft=t&id=780193987033&spm=a21dvs.23580594.0.0.4fee645exZs8w4";
            Util.FileTool.OpenUrlWithDefaultBrowser(html);
        }

        private void lbl4DO_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string html = "https://item.taobao.com/item.htm?ft=t&id=780193987033&spm=a21dvs.23580594.0.0.4fee645exZs8w4";
            Util.FileTool.OpenUrlWithDefaultBrowser(html);
        }

        private void lbl4DOWIFI_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string html = "https://item.taobao.com/item.htm?id=780531523015&skuId=5333607729555";
            Util.FileTool.OpenUrlWithDefaultBrowser(html);
        }

        private void lblElec_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string html = "https://item.taobao.com/item.htm?id=780300450343&skuId=5328327448593";
            Util.FileTool.OpenWebWithUrl(html);
        }
    }
}
