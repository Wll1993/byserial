using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace IoTClientDeskTop
{
    /// <summary>
    /// AboutWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AboutWindow : Window
    {
        
        public AboutWindow()
        {
            InitializeComponent();            
        }

        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void lblGit_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                string txt = lblGit.Content.ToString();
                int len = txt.Length;
                string html = txt.Substring(7, len - 7);
                Process.Start(html);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void lblBlog_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                string txt = lblBlog.Content.ToString();
                int len =txt.Length;
                string html = txt.Substring(6, len - 6);
                Process.Start(html);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtVer.Text = "Version:" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            txtInfo.Text= "Email:xuyuanbaoxyb@163.com\r\nWeChat/Phone:18694923164\r\nQQ:416315797\r\nQQGroup:750923887";
        }
    }
}
